using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MoveObject
{
    [SerializeField]
    private GameObject _topPipe = null;

    private float _defaultTopPositionY = 0.0f;
    private float _defaultBasePositionY = 0.0f;

    private bool _isCheck = false;

    private void Start()
    {
        _defaultTopPositionY = _topPipe.transform.localPosition.y;
        _defaultBasePositionY = transform.position.y;
    }

    public void SetHeight(float value)
    {
        Vector2 result = _topPipe.transform.localPosition;
        result.y = value + _defaultTopPositionY;

        _topPipe.transform.localPosition = result;
    }

    public void SetPositionY(float value)
    {
        Vector2 result = transform.position;
        result.y = value + _defaultBasePositionY;
        transform.position = result;
    }

    override public void GameUpdate()
    {
        base.GameUpdate();
    }

    protected override void FinishEndPosition()
    {
        Manager.Instance.Remove(this);
    }

    //Bird와 위치를 검사하는 처리
    public bool IsNeedInvokeScoreCheck(Vector3 target)
    {
        if(!_isCheck)
        {
            if(transform.position.x <= target.x)
            {
                _isCheck = true;
                return true;
            }
        }
        return false;
    }
}
