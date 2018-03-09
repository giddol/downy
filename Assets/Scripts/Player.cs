using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState { Twinkle, Play, Pause, Resume, Stop }
public class Player : MonoBehaviour, IGameObject {
    [SerializeField]
    private Rigidbody2D _rigidbody = null;

    [SerializeField]
    private Animator animator;
    
    private Vector2 prevVelocity;
    private PlayerState prevState;
    private Vector2 _startPosition = Vector2.zero;
    private PlayerState state;

    
    private float h = 0.0f;
    private float moveHorizontal = 0.0f;
    private float moveForce;

    public PlayerState State
    {
        get
        {
            return state;
        }

        set
        {
            switch (value)
            {
                case PlayerState.Twinkle: 
                    animator.SetBool("isTitle", true);
                    _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                    break;
                case PlayerState.Play:
                    if (animator.GetBool("isTitle"))
                        animator.SetBool("isTitle", false);
                    _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    break;
                case PlayerState.Pause:
                    prevState = state;
                    prevVelocity = _rigidbody.velocity;
                    _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                    if(state == PlayerState.Twinkle)
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                case PlayerState.Resume:
                    state = prevState;
                    if (prevState == PlayerState.Twinkle)
                        gameObject.SetActive(true);
                    else if (prevState == PlayerState.Play)
                    {
                        _rigidbody.velocity = prevVelocity;
                        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                    Debug.Log(prevState);
                    return;
                case PlayerState.Stop:
                    _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                    _rigidbody.velocity = Vector2.zero;
                    break;
            }

            state = value;
        }
    }

    private void Awake()
    {
        _startPosition = transform.position;
    }

    public void Init()
    {
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
        _rigidbody.velocity = Vector2.zero;
    }

    // Use this for initialization
    void Start ()
    {
        Init();
        State = PlayerState.Twinkle;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
	}

    // Update is called once per frame
    public void GameUpdate() {

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

        if (_rigidbody.velocity.x == 0)
        {
            moveForce += moveDir.x;
        }
        //_rigidbody.velocity = new Vector2(h * Time.deltaTime*10, _rigidbody.velocity.y);
        //_rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
        _rigidbody.AddForce(moveDir.normalized * Time.deltaTime * 100);

        if (this.transform.position.x < -0.482f)
        {
            this.transform.position = new Vector2(-0.482f, this.transform.position.y);
            _rigidbody.velocity = new Vector2(0.0f, _rigidbody.velocity.y);
        }
        else if (this.transform.position.x > 0.482f)
        {
            this.transform.position = new Vector2(0.482f, this.transform.position.y);
            _rigidbody.velocity = new Vector2(0.0f, _rigidbody.velocity.y);
        }

        if(this.transform.position.y > 1.08 || this.transform.position.y < -1.08)
        {
            Manager.Instance.GameState = GameState.GameOver;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Floor":
                int score = collision.gameObject.GetComponent<Floor>().FloorNumber;
                Manager.Instance.UpdateScore(score);
                break;
        }
    }
}
