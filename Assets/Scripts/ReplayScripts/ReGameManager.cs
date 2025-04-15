using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class ReGameManager : MonoBehaviour
{
    private int _p1Score;
    private int _p2Score;

    private string _uniqueID;

    public Paddle paddle1;
    public Paddle paddle2;
    public ReplayBall ball;

    public TextMeshProUGUI P1ScoreUI;
    public TextMeshProUGUI P2ScoreUI;

    private void Awake(){
        // FOr testing
        _p1Score = 0;
        _p2Score = 0;
    }

    private void EndGame(){
        Debug.Log("Game Done");
        SceneManager.LoadScene("AfterReplay");
    }   

    public void Player1Scores(){
        _p1Score++;
        P1ScoreUI.text = _p1Score.ToString();
        Debug.Log(_p1Score);
        if(_p1Score >=7) EndGame();
    }

    public void Player2Scores(){
        _p2Score++;
        P2ScoreUI.text = _p2Score.ToString();
        Debug.Log(_p2Score);
        if(_p2Score >=7) EndGame();
    }


}
