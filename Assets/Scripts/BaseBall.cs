using UnityEngine;

public class BaseBall : MonoBehaviour
{

    public float speed = 200.0f;
    protected Rigidbody2D _ballRBD;
    
    private void Awake(){
        _ballRBD = GetComponent<Rigidbody2D>();
    }
    
    public void AddForce(Vector2 force){
        _ballRBD.AddForce(force);
    }

    public Vector2 GetVelocity(){
        return _ballRBD.linearVelocity;
    }
}
