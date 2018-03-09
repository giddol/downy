using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MoveObject
{
    [SerializeField]
    private GameObject _topPipe = null;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private PhysicsMaterial2D physicsMaterial2D;

    private float _defaultTopPositionY = 0.0f;
    private float _defaultBasePositionY = 0.0f;
    private int floorNumber = 0;

    private bool _isCheck = false;

    public int FloorNumber
    {
        get
        {
            return floorNumber;
        }

        set
        {
            floorNumber = value;
            if(floorNumber > Constants.levelTwo)
            {
                spriteRenderer.color = new Color(1, 0, 0);
                boxCollider2D.sharedMaterial = physicsMaterial2D;
            }
        }
    }

    private void Start()
    {
        _defaultTopPositionY = _topPipe.transform.localPosition.y;
        _defaultBasePositionY = transform.position.y;
    }

    public void Init()
    {
        transform.position = new Vector2(0.0f,-1.5f);
    }
    public void SetHeight(float value)
    {
        Vector2 result = _topPipe.transform.localPosition;
        result.y = value + _defaultTopPositionY;

        _topPipe.transform.localPosition = result;
    }

    public void SetPositionX(float value)
    {
        Vector2 result = transform.position;
        result.x = value;
        transform.position = result;
    }

    public void SetPositionY(float value)
    {
        Vector2 result = transform.position;
        result.y = value;
        transform.position = result;
    }

    override public void GameUpdate()
    {
        base.MoveY();
    }

    protected override void FinishEndPositionY()
    {
        Manager.Instance.Remove(this);
    }

    //Player와 위치를 검사하는 처리
    public bool IsNeedInvokeScoreCheck(Vector3 target)
    {
        if (!_isCheck)
        {
            if (transform.position.x <= target.x)
            {
                _isCheck = true;
                return true;
            }
        }
        return false;
    }
}
