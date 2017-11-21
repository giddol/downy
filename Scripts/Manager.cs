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
    private float _speed = 0.01f;
    [SerializeField]
    private float _createTime = 2.0f;
    [SerializeField]
    private float _pipeRandomHeight = 0.4f;
    [SerializeField]
    private float _pipeRandomPostionY = 0.5f;

    private float _currentTime = 0.0f;

    private List<Pipe> _pipeList = new List<Pipe>();
    public float Speed { get { return _speed; } }
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

    public int Score { get { return _score; } }
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
    }

    private void Init()
    {
        _isBestScore = false;
        _isPlay = false;
        _score = 0;
        _currentTime = 0.0f;
        _bird.Init();
        _ground.Init();
        _pipeList.ToArray().ToList().ForEach(x => Remove(x));
        UIManager.Instance.Init();
    }
    public void Replay()
    {
        Init();
        _isPlay = true;
        UIManager.Instance.ShowScore();
    }
    // Update is called once per frame
    void Update ()
    {
        _bird.FreezePositionY(!_isPlay);
        if (_isPlay)
        {
            _currentTime += Time.deltaTime;
            if(_createTime<_currentTime)
            {
                _currentTime = 0;

                _pipe.SetHeight(Random.Range(0.0f, _pipeRandomHeight));
                _pipe.SetPositionY(Random.Range(0.0f, _pipeRandomPostionY));
                _pipeList.Add(GameObject.Instantiate(_pipe));
            }

            _bird.GameUpdate();
            _ground.GameUpdate();
            _pipeList.ForEach((x) =>
            {
                x.GameUpdate();
                if (x.IsNeedInvokeScoreCheck(_bird.transform.position))
                {
                    InvokeScore();
                }

            });

        }
    }

    public void Remove(Pipe target)
    {
        _pipeList.Remove(target);
        Destroy(target.gameObject);
    }

    private void InvokeScore()
    {
        _score++;

        if (_bestScore < _score)
        {
            _bestScore = _score;
            _isBestScore = true;

            PlayerPrefs.SetInt("_bestScore", _bestScore);
            PlayerPrefs.Save();
        }
        
        UIManager.Instance.Score = _score;
        Debug.Log(_score);
    }
}
