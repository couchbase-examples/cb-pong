using UnityEngine;

public class Ball : BaseBall
{
    
    public ReplayManager replayManager;
    private int _i=0;
    public void ResetBall(){
        _ballRBD.position = Vector3.zero;
        _ballRBD.linearVelocity = Vector3.zero;

        AddStartingForce();
    }

    private void Start(){
        ResetBall();
    }

    private void FixedUpdate(){
        replayManager.ballDetails.Add(new Vector2(this.transform.position.x, this.transform.position.y));
        _i++;
    }

    private void AddStartingForce(){
        float x = (Random.value < 0.5f)?1.0f:-1.0f;
        float y = (Random.value < 0.5f)?Random.Range(-1.0f, -0.5f) : Random.Range(0.5f, 1.0f);

        Vector2 initialDir = new Vector2(x,y);
        _ballRBD.AddForce(initialDir * this.speed);
    }
    
}
