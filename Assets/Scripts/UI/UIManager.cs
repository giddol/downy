using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private GameObject _title = null;

    [SerializeField]
    private Button _startButton = null;

    [SerializeField]
    private Text _score = null;

    [SerializeField]
    private GameOverPopup _gameOverPopup = null;

    [SerializeField]
    private GameObject _newBestScore = null;

    [SerializeField]
    private GameObject _settings = null;

    [SerializeField]
    private GameObject _settingsButton = null;

    [SerializeField]
    private Toggle _vibToggle = null;

    [SerializeField]
    private Toggle _soundToggle = null;

    private GameObject prevScreen = null;
    private GameState prevGameState;

    public Text testText = null;

    public int Score
    {
        set
        {
           _newBestScore.SetActive(Manager.Instance.IsBestScore);
            _score.text = value.ToString();
        }
    }


    public void Init()
    {
        if (PlayerPrefs.GetInt("soundSetting") == 1)
            _soundToggle.isOn = true;
        else
            _soundToggle.isOn = false;

        _title.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);
        _score.gameObject.SetActive(false);
        _newBestScore.gameObject.SetActive(false);
        _gameOverPopup.gameObject.SetActive(false);
        _settings.gameObject.SetActive(false);
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
        Manager.Instance.GenerateStartingFloor();
        Manager.Instance.GameState = GameState.Play;
        _title.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);

        Social.ShowAchievementsUI();
    }
	
    public void ShowScore()
    {
        _score.text = "0";
        _score.gameObject.SetActive(true);
        _newBestScore.SetActive(false);
    }

    public void InvokeGameOver()
    {
        _gameOverPopup.Show();
        _score.gameObject.SetActive(false);
        _newBestScore.SetActive(false);
    }

    public void SettingsButton()
    {
        if (_title.activeSelf)
        {
            prevScreen = _title;
            prevGameState = GameState.Title;
            Manager.Instance.Player.gameObject.SetActive(false);
        }
        else if (_gameOverPopup.gameObject.activeSelf)
        {
            prevGameState = GameState.GameOver;
            prevScreen = _gameOverPopup.gameObject;
        }
        else if(Manager.Instance.GameState == GameState.Play)
        {
            prevGameState = GameState.Play;
            prevScreen = _score.gameObject;
        }
        Manager.Instance.Pause();
        _settings.gameObject.SetActive(true);
        _settingsButton.gameObject.SetActive(false);
        prevScreen.gameObject.SetActive(false);
    }

    public void BackButton()
    {
        _settings.gameObject.SetActive(false);
        _settingsButton.gameObject.SetActive(true);
        prevScreen.gameObject.SetActive(true);
        Manager.Instance.Resume(prevGameState);
    }

    public void VibToggle()
    {
        if (_vibToggle.isOn)
        {
            Manager.Instance.isVibMuted = false;
            PlayerPrefs.SetInt("vibSetting", 1);
        }
        else
        {
            Manager.Instance.isVibMuted = true;
            PlayerPrefs.SetInt("vibSetting", 0);
        }
    }

    public void SoundToggle()
    {
        if (_soundToggle.isOn)
        {
            Manager.Instance._bgSound.mute = false;
            PlayerPrefs.SetInt("soundSetting", 1);
        }
        else
        {
            Manager.Instance._bgSound.mute = true;
            PlayerPrefs.SetInt("soundSetting", 0);
        }
    }
}
