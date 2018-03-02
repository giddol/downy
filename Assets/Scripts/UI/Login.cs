using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    [SerializeField] string email;
    [SerializeField] string password;

    public InputField inputTextEmail;
    public InputField inputTextPassword;
    public Text loginResult;

    public bool bLogin
    {
        get;
        set;
    }
    void Start()
    {
        InitGPGS();
        LoginGPGS();
    }
    public void InitGPGS()
    {
        bLogin = false;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;

        PlayGamesPlatform.Activate();
    }
    public void LoginGPGS()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(LoginCallbackGPGS);
        }
    }
    public void LoginCallbackGPGS(bool result)
    {
        bLogin = result;
        loginResult.text = bLogin.ToString();
    }
}