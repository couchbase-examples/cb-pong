using UnityEngine;

using System.Collections.Generic;
public class ReplayPlayerPaddle : Paddle
{
    private DatabaseManager dbm;
    private string _id;

    // Which paddle track to replay: DatabaseManager.Paddle1TrackKey or Paddle2TrackKey (set per paddle in the scene).
    public string _key = DatabaseManager.Paddle1TrackKey;

    private List<Vector2> frameInfo;
    
    private int _i = 0;
    private void Start(){
        dbm = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        _id = dbm.GetId();
        frameInfo = dbm.GetPaddlePositions(_id, _key);
        Debug.Log(frameInfo.Count);
    }
    private void FixedUpdate(){
        if(_i < frameInfo.Count){
            
            this.transform.position = frameInfo[_i];
            
        }
        _i++;
    }
}
