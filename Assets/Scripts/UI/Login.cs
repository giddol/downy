using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class Login : MonoBehaviour
{

    [SerializeField] string email;
    [SerializeField] string password;

    public InputField inputTextEmail;
    public InputField inputTextPassword;
    public Text loginResult;

    FirebaseAuth auth;
    FirebaseUser user;

    void Awake()
    {
        // 초기화
        loginResult.text = "before auth";
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        loginResult.text = "after auth";

        //페북 init
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }


    // 버튼이 눌리면 실행할 함수.
    public void JoinBtnOnClick()
    {
        email = inputTextEmail.text;
        password = inputTextPassword.text;

        Debug.Log("email: " + email + ", password: " + password);

        CreateUser();
    }


    void CreateUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                loginResult.text = "회원가입 실패";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                loginResult.text = "회원가입 실패";
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            loginResult.text = "회원가입 굿럭";
        });
    }

    public void LoginBtnOnClick()
    {
        email = inputTextEmail.text;
        password = inputTextPassword.text;

        Debug.Log("email: " + email + ", password: " + password);

        LoginUser();
    }

    void LoginUser()
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                loginResult.text = "로그인 실패";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                loginResult.text = "로그인 실패";
                return;
            }
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                loginResult.text = user.UserId + " 로그인 굿럭";
                OnSuccesLogin();
            }
        }
    }

    public void FacebookLoginBtnOnClick()
    {
        loginResult.text = "페북 로그인 중";
        

        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);

        
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(aToken.TokenString);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void GoogleLoginBtnOnClick()
    {
        loginResult.text = "구글 로그인 중";
        GooglePlayServiceInitialize();

        Social.localUser.Authenticate(success =>
        {
            if (success == false) return;

            StartCoroutine(coGoogleLogin());
        });

    }

    void GooglePlayServiceInitialize()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();
    }

    IEnumerator coGoogleLogin()
    {
        while (System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string accessToken = null;



        Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            loginResult.text = " 구글 로그인 굿럭";

        });
    }

    //public bool bLogin
    //{
    //    get;
    //    set;
    //}
    //void Start()
    //{
    //    InitGPGS();
    //    LoginGPGS();
    //}
    //public void InitGPGS()
    //{
    //    bLogin = false;
    //    PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
    //    PlayGamesPlatform.InitializeInstance(config);
    //    PlayGamesPlatform.DebugLogEnabled = false;

    //    PlayGamesPlatform.Activate();
    //}
    //public void LoginGPGS()
    //{
    //    if (!Social.localUser.authenticated)
    //    {
    //        Social.localUser.Authenticate(LoginCallbackGPGS);
    //    }
    //}
    //public void LoginCallbackGPGS(bool result)
    //{
    //    bLogin = result;
    //    loginResult.text = bLogin.ToString();
    //}

    public void OnSuccesLogin()
    {
        SceneManager.LoadScene("Main");
    }
}