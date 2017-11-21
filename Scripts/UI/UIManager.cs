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
    private NumbersRenderer _score = null;

    [SerializeField]
    private GameOverPopup _gameOverPopup = null;

    [SerializeField]
    private GameObject _newBestScore = null;

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
