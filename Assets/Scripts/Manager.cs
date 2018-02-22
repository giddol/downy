using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : Singleton<Manager> {
    [SerializeField]
    private Bird _bird = null;
    [SerializeField]
    private Ground _ground = null;
    [SerializeField]
    private Pipe _pipe = null;
    [SerializeField]
    private Floor _floor = null;

    [SerializeField]
    private float _speed = 0.01f;
    [SerializeField]
    private float _createTime = 2.0f;
    [SerializeField]
    private float _pipeRandomHeight = 0.4f;
    [SerializeField]
    private float _pipeRandomPostionY = 0.5f;

    public GameObject QuitAlarm = null;

    private float _currentTime = 0.0f;
    private int floorNumber = 0;

    private Floor floor = null;
    private float h = 0.0f;
    private float moveHorizontal = 0.0f;
    private float moveForce;

    private List<Pipe> _pipeList = new List<Pipe>();
    private List<Floor> _floors = new List<Floor>();
    public float Speed { get { return _speed; } }
    public int FloorNumber { get { return floorNumber; } }
    public bool IsPlay
    {
        get { return _isPlay; }
        set
        {
            _isPlay = value;

            if (!_isPlay)
            {
                UIManager.Instance.InvokeGameOver();
            }
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

    private bool _isPlay = false;
    private int _score = 0;
    private int _bestScore = 0;
    private bool _isBestScore = false;

    private void Start()
    {
        Init();
        UIManager.Instance.ShowTitle();
        _bestScore = PlayerPrefs.GetInt("_bestScore");

        for (int i = 0; i < 10; i++)
            Debug.Log(Random.Range(0, 100));
    }

    private void Init()
    {
        _isBestScore = false;
        _isPlay = false;
        _score = 0;
        _currentTime = 0.0f;
        floorNumber = 0;
        _bird.Init();
        _ground.Init();
        _pipeList.ToArray().ToList().ForEach(x => Remove(x));
        _floors.ToArray().ToList().ForEach(x => Remove(x));

        floor = GameObject.Instantiate(_floor);
        floor.SetPositionY(-0.2f);
        _floors.Add(floor);

        UIManager.Instance.Init();
    }
    public void Replay()
    {
        Init();
        _isPlay = true;
        UIManager.Instance.ShowScore();
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        _bird.FreezePositionY(!_isPlay);
        if (_isPlay)
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

            if (_bird.GetComponent<Rigidbody2D>().velocity.x == 0)
            {
                moveForce += moveDir.x;
            }

            

            //_bird.GetComponent<Rigidbody2D>().velocity = new Vector2(h * Time.deltaTime*10, _bird.GetComponent<Rigidbody2D>().velocity.y);
            //_bird.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, _bird.GetComponent<Rigidbody2D>().velocity.y);
            _bird.GetComponent<Rigidbody2D>().AddForce(moveDir.normalized * Time.deltaTime * 100);

            //Debug.Log(moveForce);

            _currentTime += Time.deltaTime;
            if(_createTime<_currentTime)
            {
                _currentTime = 0;

                //_pipe.SetHeight(Random.Range(0.0f, _pipeRandomHeight));
                //_pipe.SetPositionY(Random.Range(0.0f, _pipeRandomPostionY));
                //_pipeList.Add(GameObject.Instantiate(_pipe));
                floorNumber++;
                if (floorNumber > 5)
                {
                    floor = GameObject.Instantiate(Resources.Load<Floor>("Floor2"));
                    floor.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                }
                else
                {
                    floor = GameObject.Instantiate(Resources.Load<Floor>("Floor"));
                }
                floor.SetPositionX(Random.Range(-0.388f, 0.381f));
                floor.gameObject.SetActive(true);
                _floors.Add(floor);
            }

            _bird.GameUpdate();
            _ground.GameUpdate();

            _floors.ForEach((x) =>
            {
                x.GameUpdate();
                if (x.IsNeedInvokeScoreCheck(_bird.transform.position))
                {
                    //InvokeScore();
                }
            });
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

    public void Remove(Pipe target)
    {
        _pipeList.Remove(target);
        Destroy(target.gameObject);
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
}
