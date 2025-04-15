using UnityEngine;

public class Paddle : MonoBehaviour
{

    public float speed = 10.0f;
    protected Rigidbody2D _rbd;
    private float initialSpeed;

    private void Awake(){
        initialSpeed = speed;
        _rbd = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision){
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if(ball != null){
            this.speed += 0.5f;
        }
    }

    public void ResetSpeed(){
        this.speed = initialSpeed;
    }
}
