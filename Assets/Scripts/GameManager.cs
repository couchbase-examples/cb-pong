using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class GameManager : MonoBehaviour
{
    public ReplayManager replayManager;
    private int _p1Score;
    private int _p2Score;

    private string _uniqueID;

    public Paddle paddle1;
    public Paddle paddle2;
    public Ball ball;

    public TextMeshProUGUI P1ScoreUI;
    public TextMeshProUGUI P2ScoreUI;

    private void Awake(){
        // FOr testing
        _p1Score = 0;
        _p2Score = 0;
        _uniqueID = Guid.NewGuid().ToString().Substring(0, 6);
        Debug.Log(_uniqueID);
    }

    private void EndGame(){
        replayManager.EndRecording(_p1Score, _p2Score);
        Debug.Log("Game Done");
        SceneManager.LoadScene("AfterGame");
    }   

    public void Player1Scores(){
        _p1Score++;
        P1ScoreUI.text = _p1Score.ToString();
        Debug.Log(_p1Score);
        this.paddle1.ResetSpeed();
        this.paddle2.ResetSpeed();
        this.ball.ResetBall();    
        if(_p1Score >=7) EndGame();
    }

    public void Player2Scores(){
        _p2Score++;
        P2ScoreUI.text = _p2Score.ToString();
        Debug.Log(_p2Score);
        this.paddle1.ResetSpeed();
        this.paddle2.ResetSpeed();
        this.ball.ResetBall();
        if(_p2Score >=7) EndGame();
    }

    public string GetID(){
        return _uniqueID;
    }

}
