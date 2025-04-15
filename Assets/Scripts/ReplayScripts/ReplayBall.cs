using UnityEngine;

using Couchbase.Lite;
using Couchbase.Lite.Query;


public class ReplayBall : BaseBall
{
    private DatabaseManager dbm;
    private string _id;

    private ArrayObject ballInfo;
    private int _i=0;

    private void Start(){
        dbm = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        _id = dbm.GetId();
        var doc = dbm.GetEntireDoc(_id);
        ballInfo = doc.GetArray("ballInfo");
    }

    private void FixedUpdate(){
        if(_i < ballInfo.Count){
            this.transform.position = new Vector3(ballInfo.GetDictionary(_i).GetFloat("ballX"), ballInfo.GetDictionary(_i).GetFloat("ballY"), 0);
            
        }

        _i++;
    }

    
}
