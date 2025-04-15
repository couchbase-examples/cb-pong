using UnityEngine;

using Couchbase.Lite;
using Couchbase.Lite.Query;

using System;
using System.IO;
using System.Collections.Generic;
public class ReplayPlayerPaddle : Paddle
{
    private DatabaseManager dbm;
    private string _id = "5eb9f1";

    public string _key = "paddleInfo";

    // private int _stateChanges = 0; 
    private ArrayObject frameInfo;
    
    private int _i = 0;
    private void Start(){
        dbm = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        _id = dbm.GetId();
        var doc = dbm.GetEntireDoc(_id);
        frameInfo = doc.GetArray(_key);
        Debug.Log(frameInfo.Count);
    }
    private void FixedUpdate(){
        if(_i < frameInfo.Count){
            
            this.transform.position = new Vector3(frameInfo.GetDictionary(_i).GetFloat("paddleX"), frameInfo.GetDictionary(_i).GetFloat("paddleY"), 0);
            
        }
        _i++;
    }
}
