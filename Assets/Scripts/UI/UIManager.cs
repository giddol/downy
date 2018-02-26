using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private GameObject _title = null;

    [SerializeField]
    private Button _startButton = null;

    [SerializeField]
    private NumbersRenderer _score = null;

    [SerializeField]
    private GameOverPopup _gameOverPopup = null;

    [SerializeField]
    private GameObject _newBestScore = null;

    public Text testText = null;

    public int Score
    {
        set
        {
           _newBestScore.SetActive(Manager.Instance.IsBestScore);
            _score.Value = value;
        }
    }


    public void Init()
    {
        _title.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);
        _score.gameObject.SetActive(false);
        _newBestScore.gameObject.SetActive(false);
        _gameOverPopup.gameObject.SetActive(false);
    }

    private void Start()
    {
		/*
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();

        


        SignIn();
        */
    }
	/*
    public void SignIn()
    {
        testText.text = "왜";
        PlayGamesPlatform.Instance.Authenticate((bool success) =>
        {
            if (success)
            {
                StartButton();
                testText.text = "success";
                Debug.Log("success");
            }
            else
            {
                ShowScore();
                testText.text = "fail";
                Debug.Log("fail");
            }
        });
        testText.text = "안될까";
    }
	*/
    public void ShowTitle()
    {
        _title.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(true);
    }

    public void StartButton()
    {
        ShowScore();
        Manager.Instance.IsPlay = true;
        _title.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);

        Social.ShowAchievementsUI();
    }
	
    public void ShowScore()
    {
        _score.Value = 0;
        _score.gameObject.SetActive(true);
        _newBestScore.SetActive(false);
    }

    public void InvokeGameOver()
    {
        _gameOverPopup.Show();
        _score.gameObject.SetActive(false);
        _newBestScore.SetActive(false);
    }
}
