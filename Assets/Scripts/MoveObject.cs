using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour,IGameObject {

    public float _startPositionY = -1.0f;
    public float _endPositionY = 1.5f;

    // Update is called once per frame
    virtual public void GameUpdate()
    {
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

    virtual protected void FinishEndPositionY()
    {
        transform.position = new Vector2(transform.position.x, _startPositionY);
    }
}
