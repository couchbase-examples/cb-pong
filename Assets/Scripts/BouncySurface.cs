using UnityEngine;

public class BouncySurface : MonoBehaviour
{
    public float bounceStrength;

    private void OnCollisionEnter2D(Collision2D collision){
        BaseBall ball = collision.gameObject.GetComponent<BaseBall>();
        if(ball != null){
            Vector2 vel = ball.GetVelocity().normalized;
            vel.x *= 1.5f;
            ball.AddForce(vel * bounceStrength);
        }
    }
}
