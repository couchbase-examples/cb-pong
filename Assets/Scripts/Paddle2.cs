using UnityEngine;

public class Paddle2 : Paddle
{
    public Rigidbody2D _ballRBD;

    private void FixedUpdate(){
        if(_ballRBD.linearVelocity.x > 0.0f){
            if(_ballRBD.position.y > this.transform.position.y){
                _rbd.AddForce(Vector2.up * this.speed);
            }
            else{
                _rbd.AddForce(Vector2.down * this.speed);
            }
        }
        else{
            if(this.transform.position.y > 0.0f){
                _rbd.AddForce(Vector2.down * this.speed);
            }
            else{
                _rbd.AddForce(Vector2.up * this.speed);
            }
        }
    }
}
