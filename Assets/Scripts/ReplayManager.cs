using UnityEngine;

using System.IO;
using System.Collections.Generic;

public class FrameInfo{
    public int frameNumber{get; set;}
    public float time{get; set;}
    public Vector2 pos{get; set;}
}

public class ReplayManager : MonoBehaviour
{
    private DatabaseManager databaseManager;
    public GameManager gameManager;
    private bool _record;
    private int _i;
    public Ball ball;
    public Transform paddleTransform;
    public Transform computerTransform;

    private float startTime; 

    private string _id;
    public List<Vector2> ballDetails;

    public List<Vector2> positions1;
    public List<Vector2> positions2;
    private void Awake(){
        _record = true;
        _i = 0;
        startTime = Time.time;
        _id = gameManager.GetID();
        ballDetails = new List<Vector2>();
        positions1 = new List<Vector2>();
        positions2 = new List<Vector2>();
    }

    private void Start(){
        databaseManager = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
    }

    private void FixedUpdate(){
        if(_record){
            positions1.Add(new Vector2(paddleTransform.position.x, paddleTransform.position.y));
            positions2.Add(new Vector2(computerTransform.position.x, computerTransform.position.y));
        }
        _i++;
        
    }
    public void EndRecording(int p1Score, int p2Score){
        _record = false;
        Debug.Log(positions1.Count);
        Debug.Log(positions2.Count);
        Debug.Log(ballDetails.Count);
        databaseManager.SaveData(_id, positions1, positions2, ballDetails, p1Score, p2Score);
    }
}
