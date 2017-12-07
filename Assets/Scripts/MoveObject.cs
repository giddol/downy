using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour,IGameObject {

    [SerializeField]
    private float _startPositionX = 0.05f;
    [SerializeField]
    private float _endPositionX = -0.014f;

    public float _startPositionY = -1.0f;
    public float _endPositionY = 1.0f;

    // Update is called once per frame
    virtual public void GameUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 position = transform.position;
        position.x -= Manager.Instance.Speed;
        if (position.x < _endPositionX)
        {
            FinishEndPosition();
        }
        else
        {
            transform.position = position;
        }
    }

    public void MoveY()
    {
        Vector2 position = transform.position;
        position.y += Manager.Instance.Speed;
        if (position.y > _endPositionY)
        {
            FinishEndPositionY();
        }
        else
        {
            transform.position = position;
        }
    }

    virtual protected void FinishEndPosition()
    {
        transform.position = new Vector2(_startPositionX, transform.position.y);
    }

    virtual protected void FinishEndPositionY()
    {
        transform.position = new Vector2(transform.position.x, _startPositionY);
    }
}
