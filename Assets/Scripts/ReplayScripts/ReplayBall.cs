using UnityEngine;

using System.Collections.Generic;


public class ReplayBall : BaseBall
{
    private DatabaseManager dbm;
    private string _id;

    private List<Vector2> ballInfo;
    private int _i=0;

    private void Start(){
        dbm = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        _id = dbm.GetId();
        ballInfo = dbm.GetBallPositions(_id);
    }

    private void FixedUpdate(){
        if(_i < ballInfo.Count){
            this.transform.position = ballInfo[_i];
            
        }

        _i++;
    }

    
}
