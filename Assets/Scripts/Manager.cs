using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase.Auth;
using Facebook.Unity;

public enum GameState { Title, Play, GameOver, Pause}

public class Manager : Singleton<Manager> {
    [SerializeField]
    private Player _player = null;

    [SerializeField]
    private Floor _floor = null;
    
    public AudioSource _bgSound = null;

    [SerializeField]
    private float _speed = 0.01f;
    [SerializeField]
    private float _createTime = 1.0f;

    [SerializeField]
    private int floorNumber = 0;

    public GameOverPopup _gameOverPopup = null;

    public GameObject QuitAlarm = null;

    private float _currentTime = 0.0f;

    private Floor floor = null;

    private List<Floor> _floors = new List<Floor>();

    private int _score = 0;
    private int _bestScore = 0;
    private bool _isBestScore = false;

    public bool isVibMuted = false;

    private GameState gameState;

    private MemoryPool pool;

    public float Speed { get { return _speed; } }
    public int FloorNumber { get { return floorNumber; } }
    
    public GameState GameState
    {
        get { return gameState; }
        set
        {
            gameState = value;
            switch (gameState)
            {
                case GameState.Title:
                    _player.State = PlayerState.Twinkle;
                    break;
                case GameState.Play:
                    _player.State = PlayerState.Play;
                    break;
                case GameState.GameOver:
                    Vibrate();
                    _player.State = PlayerState.Stop;
                    UIManager.Instance.InvokeGameOver();
                    break;
            }
        }
    }

    public void FbShare()
    {
        FB.ShareLink(
            new System.Uri("https://developers.facebook.com/"),
            callback: ShareCallback
        );
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !System.String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!System.String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");

            Heart.Instance.CurrentHeart += 2;
        }
    }

    public int Score { get { return _score; } set { _score = value; } }
    public int BestScore { get { return _bestScore; } }

    public bool IsBestScore
    {
        get
        {
            return _isBestScore;
        }

        set
        {
            _isBestScore = value;
        }
    }

    private void Start()
    {
        pool = new MemoryPool(_floor, 5, 50);
        if (!PlayerPrefs.HasKey("soundSetting"))
        {
            PlayerPrefs.SetInt("soundSetting", 1);
        }
        if (!PlayerPrefs.HasKey("vibSetting"))
        {
            PlayerPrefs.SetInt("vibSetting", 1);
        }

        GameState = GameState.Title;
        Init();
        UIManager.Instance.ShowTitle();

        _bestScore = PlayerPrefs.GetInt("_bestScore");

    }

    private void Init()
    {
        _isBestScore = false;
        _score = 0;
        _currentTime = 0.0f;
        floorNumber = 0;
        _player.Init();
        _floors.ToArray().ToList().ForEach(x => Remove(x));

        UIManager.Instance.Init();
    }

    public void GenerateStartingFloor()
    {
        float posY = -0.114f;
        floor = pool.TakeItem();
        floor.Init();
        floor.SetPositionY(posY);
        floor.FloorNumber = floorNumber;
        _floors.Add(floor);

        while (posY > -1.5f)
        {
            posY -= 0.75f;
            floorNumber++;
            floor = pool.TakeItem();
            floor.Init();
            floor.SetPositionX(Random.Range(-0.388f, 0.381f));
            floor.SetPositionY(posY);
            floor.FloorNumber = floorNumber;
            _floors.Add(floor); 
        }
    }

    public void Replay()
    {
        if (!Heart.Instance.TryDecreaseHeart())
            return;

        _gameOverPopup.gameObject.SetActive(false);

        Init();
        GenerateStartingFloor();
        GameState = GameState.Play;
        

        UIManager.Instance.ShowScore();
    }

    public void Pause()
    {
        GameState = GameState.Pause;
        _player.State = PlayerState.Pause;
    }
    
    public void Resume(GameState state)
    {
        GameState = state;
        _player.State = PlayerState.Resume;
    }

    void FixedUpdate()
    {
        switch(gameState)
        {
            case GameState.Play:
            {
                _currentTime += Time.deltaTime;
                if (_createTime < _currentTime)
                {
                    _currentTime = 0;


                    floorNumber++;
                    if (floorNumber == Constants.levelTwo)
                    {
                        _createTime = 2.0f;
                    }
                    floor = pool.TakeItem();
                    floor.Init();
                    floor.SetPositionX(Random.Range(-0.388f, 0.381f));
                    floor.gameObject.SetActive(true);
                    floor.FloorNumber = floorNumber;
                    _floors.Add(floor);

                }

                _player.GameUpdate();

                _floors.ForEach((x) =>
                {
                    x.GameUpdate();
                    if (x.IsNeedInvokeScoreCheck(_player.transform.position))
                    {
                    //InvokeScore();
                    }
                });
                break;
            }
            case GameState.GameOver:
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    Replay();
                }
                break;
            }
        }
    }

    private void Update()
    {
        //back키 누르면 종료
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.A))
        {
            if (QuitAlarm.activeSelf)
            {
                Application.Quit();
            }
            else
            {
                QuitAlarm.SetActive(true);
                Invoke("InactiveQuitAlarm", 3.0f);
            }
        }
    }

    public void Remove(Floor target)
    {
        _floors.Remove(target);
        pool.PutItem(target);
    }

    public void UpdateScore(int score)
    {
        _score = score;

        CheckAchievement(score);

        if (_bestScore < _score)
        {
            _bestScore = _score;
            _isBestScore = true;

            PlayerPrefs.SetInt("_bestScore", _bestScore);
            PlayerPrefs.Save();
        }
        
        UIManager.Instance.Score = _score;
    }

    public void CheckAchievement(int score)
    {
        if (PlayerPrefs.GetInt("Ach1") != 1 && score >= 5)
        {
            PlayerPrefs.SetInt("Ach1", 1);
            PlayerPrefs.Save();
            UIManager.Instance.testText.text = "업적1 달성";
        }
    }
    public void InactiveQuitAlarm()
    {
        QuitAlarm.SetActive(false);
    }

    public void Vibrate()
    {
#if UNITY_ANDROID
        if (!isVibMuted)
            Handheld.Vibrate();
#endif
    }

    void OnApplicationQuit()
    {
        StopAllCoroutines();
        //FirebaseAuth.DefaultInstance.SignOut();
        Heart.Instance.StopAllCoroutines();
        pool.Dispose(); //메모리 풀 삭제
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
