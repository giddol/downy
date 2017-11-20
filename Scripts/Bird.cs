using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour, IGameObject {
    [SerializeField]
    private Rigidbody2D _rigidbody = null;
    
    [SerializeField]
    private float _jumpValue = 1.0f;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    public void GameUpdate() {
		if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _rigidbody.AddForce(new Vector2(0, _jumpValue));
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Enemy":
                Manager.Instance.IsPlay = false;
                break;

        }
    }
}
