using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase.Auth;
using Facebook.Unity;

public enum GameState { Title, Play, GameOver, Pause }



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
    private float h = 0.0f;
    private float moveHorizontal = 0.0f;
    private float moveForce;

    private List<Floor> _floors = new List<Floor>();

    private int _score = 0;
    private int _bestScore = 0;
    private bool _isBestScore = false;

    public bool isVibMuted = false;

    private Vector2 playerVelocity;

    private GameState gameState;

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
                        _player.GetComponent<Animator>().SetBool("isTitle", true);
                    break;
                case GameState.Play:
                    if (_player.GetComponent<Animator>().GetBool("isTitle"))
                        _player.GetComponent<Animator>().SetBool("isTitle", false);
                    break;
                case GameState.GameOver:
                    UIManager.Instance.InvokeGameOver();
                    FB.ShareLink(
                        new System.Uri("https://developers.facebook.com/"),
                        callback: ShareCallback
                    );
#if UNITY_ANDROID
                    Vibrate();
#endif
                    break;
            }
        }
    }

    private void writeNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        //FirebaseAuth.DefaultInstance.reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
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

    public Player Player
    {
        get
        {
            return _player;
        }

        set
        {
            _player = value;
        }
    }

    

    private void Start()
    {
        if (!PlayerPrefs.HasKey("soundSetting"))
        {
            PlayerPrefs.SetInt("soundSetting", 1);
        }
        if (!PlayerPrefs.HasKey("vibSetting"))
        {
            PlayerPrefs.SetInt("vibSetting", 1);
        }

        Init();
        UIManager.Instance.ShowTitle();
        _bestScore = PlayerPrefs.GetInt("_bestScore");

    }

    private void Init()
    {
        GameState = GameState.Title;
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
        floor = GameObject.Instantiate(_floor);
        floor.SetPositionY(-0.114f);
        _floors.Add(floor);

        floorNumber++;
        floor = GameObject.Instantiate(_floor);
        floor.SetPositionX(Random.Range(-0.388f, 0.381f));
        floor.SetPositionY(-0.75f);
        floor.FloorNumber = floorNumber;
        _floors.Add(floor);

        floorNumber++;
        floor = GameObject.Instantiate(_floor);
        floor.SetPositionX(Random.Range(-0.388f, 0.381f));
        floor.SetPositionY(-1.5f);
        floor.FloorNumber = floorNumber;
        _floors.Add(floor);
    }

    public void Replay()
    {
        Init();
        GenerateStartingFloor();

        gameState = GameState.Play;
        UIManager.Instance.ShowScore();
    }

    public void Pause()
    {
        GameState = GameState.Pause;
        playerVelocity = _player.GetComponent<Rigidbody2D>().velocity;
    }
    
    public void Resume()
    {
        GameState = GameState.Play;
        _player.GetComponent<Rigidbody2D>().velocity = playerVelocity;
    }
    public void Resume(GameState state)
    {
        GameState = state;
        if(state == GameState.Title)
        {
            Player.gameObject.SetActive(true);
        }
        if (state == GameState.Play)
        {
            _player.GetComponent<Rigidbody2D>().velocity = playerVelocity;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _player.FreezePositionY(gameState != GameState.Play);
        switch(gameState)
        {
            case GameState.Play:
            {
                //플레이어 터치 이동
                if (Input.touchCount > 0)
                {
                    Vector2 touchPosition = Input.GetTouch(0).position;
                    if (touchPosition.x > Screen.width / 2)
                    {
                        moveHorizontal = 1;
                        if (Input.GetTouch(0).phase == TouchPhase.Ended)
                        {
                            moveHorizontal = 0f;
                        }
                    }
                    else
                    {
                        moveHorizontal = -1;
                        if (Input.GetTouch(0).phase == TouchPhase.Ended)
                        {
                            moveHorizontal = 0f;
                        }
                    }
                }


                Vector2 moveDir = (Vector2.right * moveHorizontal);

#if UNITY_EDITOR
                h = Input.GetAxis("Horizontal");
                moveDir = (Vector2.right * h);
#endif

                if (_player.GetComponent<Rigidbody2D>().velocity.x == 0)
                {
                    moveForce += moveDir.x;
                }



                //_player.GetComponent<Rigidbody2D>().velocity = new Vector2(h * Time.deltaTime*10, _player.GetComponent<Rigidbody2D>().velocity.y);
                //_player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, _player.GetComponent<Rigidbody2D>().velocity.y);
                _player.GetComponent<Rigidbody2D>().AddForce(moveDir.normalized * Time.deltaTime * 100);

                _currentTime += Time.deltaTime;
                if (_createTime < _currentTime)
                {
                    _currentTime = 0;


                    floorNumber++;
                    if (floorNumber > 30)
                    {
                        _createTime = 2.0f;
                        floor = GameObject.Instantiate(Resources.Load<Floor>("Floor2"));
                        floor.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    }
                    else
                    {
                        floor = GameObject.Instantiate(Resources.Load<Floor>("Floor"));
                    }
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

                    _gameOverPopup.gameObject.SetActive(false);
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
                FirebaseAuth.DefaultInstance.SignOut();
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
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
        Destroy(target.gameObject);
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
    void InactiveQuitAlarm()
    {
        QuitAlarm.SetActive(false);
    }

    void Vibrate()
    {
#if UNITY_ANDROID
        if (!isVibMuted)
            Handheld.Vibrate();
#endif
    }
}
