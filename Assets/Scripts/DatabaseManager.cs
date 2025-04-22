using Couchbase.Lite;
using Couchbase.Lite.DI;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;

using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

public class DatabaseManager : MonoBehaviour
{

    public static DatabaseManager Instance;
    private Database _db;
    private Collection _col;

    private string _id;

    private void Awake(){

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _db = new Database("pong_db");
        _col = _db.CreateCollection("replays");

        var remoteURL = new URLEndpoint(new Uri("wss://bxwx4gt0huaibgy.apps.cloud.couchbase.com:4984/pong-data"));
        var config = new ReplicatorConfiguration(remoteURL);
        config.AddCollection(_col);
        config.ReplicatorType = ReplicatorType.PushAndPull;
        config.EnableAutoPurge = false;
        config.Continuous = true;
        config.Authenticator = new BasicAuthenticator("<username>", "<password>");
        
        var thisReplicator = new Replicator(config);

        var token = thisReplicator.AddChangeListener((sender, args) => {
                if (args.Status.Activity == ReplicatorActivityLevel.Stopped) {
                    Debug.Log("Replication stopped");
                }
                if (args.Status.Activity == ReplicatorActivityLevel.Idle) {
                    Debug.Log("Replication idle");
                }
                if (args.Status.Activity == ReplicatorActivityLevel.Connecting) {
                    Debug.Log("Replication Connecting");
                }
                if (args.Status.Activity == ReplicatorActivityLevel.Offline) {
                    Debug.Log("Replication Offline");
                }
                if (args.Status.Activity == ReplicatorActivityLevel.Busy) {
                    Debug.Log("Replication busy");
                }
            } 
        );

        thisReplicator.Start();
    }

    public IResultSet GetAllReplays(){
        using var query = QueryBuilder.Select(
            SelectResult.Expression(Meta.ID),
            SelectResult.Property("timestamp"),
            SelectResult.Property("p1Score"),
            SelectResult.Property("p2Score")
        )
        .From(DataSource.Collection(_col));
        var result = query.Execute();
        return result;
    }

    public IResultSet GetLatestTenReplays(){
        using var query = QueryBuilder.Select(
            SelectResult.Expression(Meta.ID),
            SelectResult.Property("timestamp"),
            SelectResult.Property("p1Score"),
            SelectResult.Property("p2Score")
        )
        .From(DataSource.Collection(_col))
        .OrderBy(Ordering.Property("timestamp").Descending())
        .Limit(Expression.Int(10));
        var result = query.Execute();
        return result;
    }

    public Document GetEntireDoc(string doc_id){
        var doc = _col.GetDocument(doc_id);

        return doc;
    }


    public void SaveData(string id, List<Vector2> frames, List<Vector2> frames2, List<Vector2> ballInfo, int p1Score, int p2Score){
        Debug.Log(frames.Count);
        var doc = new MutableDocument(id);
        var frameArray = new MutableArrayObject();
        foreach(var frame in frames){
            var frameDict = new MutableDictionaryObject();
            frameDict.SetFloat("paddleX", frame.x);
            frameDict.SetFloat("paddleY", frame.y);
            frameArray.AddDictionary(frameDict);
        }
        var frameArray2 = new MutableArrayObject();
        foreach(var frame in frames2){
            var frameDict2 = new MutableDictionaryObject();
            frameDict2.SetFloat("paddleX", frame.x);
            frameDict2.SetFloat("paddleY", frame.y);
            frameArray2.AddDictionary(frameDict2);
        }
        var ballPositionArray = new MutableArrayObject();
        foreach(var ballPos in ballInfo){
            var ballDict = new MutableDictionaryObject();
            ballDict.SetFloat("ballX", ballPos.x);
            ballDict.SetFloat("ballY", ballPos.y);
            ballPositionArray.AddDictionary(ballDict);
        }

        doc.SetArray("ballInfo", ballPositionArray);
        doc.SetArray("paddleInfo", frameArray);
        doc.SetArray("paddleInfo2", frameArray2);
        doc.SetDate("timestamp", DateTime.Now);
        doc.SetInt("p1Score", p1Score);
        doc.SetInt("p2Score", p2Score);
        _col.Save(doc);
    }

    public void SetId(string id){
        _id = id;
    }

    public string GetId(){
        return _id;
    }

}
