using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

public class User
{
    public string username;
    public string email;
    public string UserId;
    public int heart;
    public int bestScore;
    public string achievements; 

    public User()
    {
    }

    public User(string username, string email)
    {
        this.username = username;
        this.email = email;
    }

    public User(string username, string email, string userId)
    {
        this.username = username;
        this.email = email;
        UserId = userId;
        heart = Constants.MAX_HEART;
        bestScore = 0;
    }
}

public class Achievements
{
    public bool ach1;
    public bool ach2;

    public Achievements()
    {
        ach1 = true;
        ach2 = false;
    }
}

public class Ach
{
    public string subject;
    public string name;
    public int reqValue;
    public int currentValue;
    public int reward;
    public bool isClear;

    public Ach()
    {
        subject = "계단";
        name = "100계단 달성";
        reqValue = 5;
        currentValue = 0;
        reward = 1000;
        isClear = false;
    }

    public Ach(string s, string n, int reqV, int curV, int rw)
    {
        subject = s;
        name = n;
        reqValue = reqV;
        currentValue = curV;
        reward = rw;
        isClear = false;
    }
}

public class FireBaseDBManager : Singleton<FireBaseDBManager>
{
    private DatabaseReference databaseReference;
    private FirebaseUser user;
    public UnityEngine.UI.Text text;


    // Use this for initialization
    void Start () {
        // Set these values before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://down-d257e.firebaseio.com/");
        FirebaseApp.DefaultInstance.SetEditorP12FileName("downy-test-74f7c5fd86e9.p12");
        FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("test-748@down-d257e.iam.gserviceaccount.com");
        FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
        //// Get the root reference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
#if UNITY_EDITOR
        CreateUser();
#endif
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        //WriteNewUser(user.UserId, user.DisplayName, user.Email);
    }

    private void WriteNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);

        Debug.Log("업데이트 완료");
    }

    void CreateUser()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            Firebase.Auth.FirebaseUser newUser = task.Result;
            User user = new User(newUser.DisplayName, newUser.Email, newUser.UserId);
            string json = JsonUtility.ToJson(user);
            //string json = "{\"username\":\"\",\"email\":\"\",\"UserId\":\"OEJUVPVABO\",\"heart\":10,\"bestScore\":0,\"achievements\":{\"ach1\":true,\"ach2\":false}\"}";
            Debug.Log(json);
            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(user.UserId).SetRawJsonValueAsync(json);
            Achievements achievements = new Achievements();
            string jsonAch = JsonUtility.ToJson(achievements);
            Debug.Log(jsonAch);
            databaseReference.Child("users").Child(user.UserId).Child("achievements").SetRawJsonValueAsync(jsonAch);

            databaseReference.Child("users").Child(user.UserId).Child("achievements").Child("ach1").SetRawJsonValueAsync(JsonUtility.ToJson(new Ach()));
            Ach ach2 = new Ach("게임플레이", "게임플레이 3회", 3, 0, 2000);
            databaseReference.Child("users").Child(user.UserId).Child("achievements").Child("ach2").SetRawJsonValueAsync(JsonUtility.ToJson(ach2));


            Heart.Instance.Init();
            AchievementsManager.Instance.Init();

        });
        
    }
}
