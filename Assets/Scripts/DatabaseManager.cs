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

    // Replay document schema. The writer (SaveData) and the readers
    // (GetBallPositions / GetPaddlePositions) share these names, so replay
    // components never need to know how frames are stored.
    public const string BallTrackKey = "ballInfo";
    public const string Paddle1TrackKey = "paddleInfo";
    public const string Paddle2TrackKey = "paddleInfo2";

    public List<Vector2> GetBallPositions(string doc_id){
        return ReadTrack(doc_id, BallTrackKey, "ballX", "ballY");
    }

    public List<Vector2> GetPaddlePositions(string doc_id, string trackKey){
        return ReadTrack(doc_id, trackKey, "paddleX", "paddleY");
    }

    public void SaveData(string id, List<Vector2> frames, List<Vector2> frames2, List<Vector2> ballInfo, int p1Score, int p2Score){
        Debug.Log(frames.Count);
        var doc = new MutableDocument(id);

        doc.SetArray(BallTrackKey, ToTrackArray(ballInfo, "ballX", "ballY"));
        doc.SetArray(Paddle1TrackKey, ToTrackArray(frames, "paddleX", "paddleY"));
        doc.SetArray(Paddle2TrackKey, ToTrackArray(frames2, "paddleX", "paddleY"));
        doc.SetDate("timestamp", DateTime.Now);
        doc.SetInt("p1Score", p1Score);
        doc.SetInt("p2Score", p2Score);
        _col.Save(doc);
    }

    private static MutableArrayObject ToTrackArray(List<Vector2> positions, string xKey, string yKey){
        var array = new MutableArrayObject();
        foreach(var pos in positions){
            var dict = new MutableDictionaryObject();
            dict.SetFloat(xKey, pos.x);
            dict.SetFloat(yKey, pos.y);
            array.AddDictionary(dict);
        }
        return array;
    }

    private List<Vector2> ReadTrack(string doc_id, string trackKey, string xKey, string yKey){
        var positions = new List<Vector2>();
        var array = GetEntireDoc(doc_id)?.GetArray(trackKey);
        if(array == null){
            Debug.LogWarning($"Replay {doc_id} has no '{trackKey}' track");
            return positions;
        }
        for(int i = 0; i < array.Count; i++){
            var dict = array.GetDictionary(i);
            positions.Add(new Vector2(dict.GetFloat(xKey), dict.GetFloat(yKey)));
        }
        return positions;
    }

    public void SetId(string id){
        _id = id;
    }

    public string GetId(){
        return _id;
    }

}
