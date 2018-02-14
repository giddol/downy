using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour, IGameObject {
    [SerializeField]
    private Rigidbody2D _rigidbody = null;
    
    [SerializeField]
    private float _jumpValue = 1.0f;

    private Vector3 _startPosition = Vector3.zero;
    private Quaternion _startRotation = Quaternion.identity;
     
    private void Awake()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void Init()
    {
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
    }

    // Use this for initialization
    void Start ()
    {
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
	}

    // Update is called once per frame
    public void GameUpdate() {
		//if(Input.GetKeyDown(KeyCode.Mouse0))
  //      {
  //          _rigidbody.AddForce(new Vector2(0, _jumpValue));
  //      }


        if (this.transform.position.x < -0.635f)
        {
            this.transform.position = new Vector2(-0.635f, this.transform.position.y);
            _rigidbody.velocity = new Vector2(0.0f, _rigidbody.velocity.y);
        }
        else if (this.transform.position.x > 0.635f)
        {
            this.transform.position = new Vector2(0.635f, this.transform.position.y);
            _rigidbody.velocity = new Vector2(0.0f, _rigidbody.velocity.y);
        }

    }

    public void FreezePositionY(bool value)
    {
        //_rigidbody.constraints = value ? RigidbodyConstraints2D.FreezePositionY : RigidbodyConstraints2D.None;
        _rigidbody.constraints = value ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Enemy":
                Manager.Instance.IsPlay = false;
                break;
            case "Floor":
                int score = collision.gameObject.GetComponent<Floor>().FloorNumber;
                Manager.Instance.UpdateScore(score);
                break;
        }
    }
}
