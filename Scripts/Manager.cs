using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    private static Manager _intance = null;
    public static Manager Instance { get { return _intance; } }

    

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
    public bool IsPlay { get { return _isPlay; } set { _isPlay = value; } }


    private bool _isPlay = true;
    private int _score = 0;

	// Use this for initialization
	void Awake () {
        _intance = this;
	}
	
	// Update is called once per frame
	void Update () {
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

        Debug.Log(_score);
    }
}
