# Pong with Couchbase Lite Integration

A basic implementation of the classic Pong game built with Unity, with a replay system powered by Couchbase Lite for database functionality. This project showcases how Unity and Couchbase Lite can work together to create a seamless gaming experience with offline-first data capabilities and synchronization.


## About Couchbase Lite

Couchbase Lite is an embedded NoSQL database that provides offline-first capabilities to mobile and edge applications. It supports full CRUD operations, rich queries using SQL++, and bidirectional synchronization with Couchbase Server through Sync Gateway.

Learn more [here](https://www.couchbase.com/products/lite)

## Features

### Core Gameplay
- Classic Pong gameplay with two paddles and a ball
- Dynamic difficulty progression (paddles and ball speed up after each hit)
- First player to reach 7 points wins the game

### Database-Powered Replay System
- Complete game state recording using Couchbase Lite
- View the replay any previously played game with exact paddle and ball positions
- Browse replay history with timestamps and scores
- Database-backed persistence allowing games to be viewed across sessions

## Technical Architecture

### Unity Implementation
The game is built with Unity's 2D physics system and consists of several key components:

#### Game Core
- **GameManager**: Controls game flow, scoring, and win conditions
- **Ball**: Manages ball physics, collision detection, and initial random trajectory
- **Paddle/Paddle2**: Handle player and computer paddle movement and physics
- **ScoringArea**: Detects when a point is scored and notifies the GameManager
- **BouncySurface**: Implements physics for wall collisions

#### Replay Framework
- **ReplayManager**: Records game state at each physics frame
- **ReplayMenu**: UI for browsing and selecting saved game replays
- **ReplayPlayButton**: Handles replay selection and scene loading
- **ReplayScripts/**: Contains specialized components for replay playback
  - **ReGameManager**: Simplified GameManager for replay mode
  - **ReplayBall**: Reconstructs ball movement from database
  - **ReplayPlayerPaddle**: Reconstructs paddle movement from database

### Couchbase Lite Integration

#### Database Architecture
- **DatabaseManager** singleton that manages all database operations
- Local database named "pong_db" with a collection called "replays"
- Document structure for each game:
  - Unique ID as document ID (generated via GUID)
  - Arrays for paddle positions (`paddleInfo`, `paddleInfo2`)
  - Array for ball positions (`ballInfo`)
  - Game metadata (timestamp, final scores)

#### Database Operations
1. **Data Capture**:
   ```csharp
   public void SaveData(string id, List<Vector2> frames, List<Vector2> frames2, List<Vector2> ballInfo, int p1Score, int p2Score)
   ```
   Converts Unity Vector2 position data to Couchbase Lite MutableDictionary objects and stores them as arrays within documents.

2. **Query and Retrieval**:
   ```csharp
   public IResultSet GetLatestTenReplays()
   ...
   public IResultSet GetAllReplays()
   ...
   public Document GetEntireDoc(string doc_id)
   ...
   public List<Vector2> GetBallPositions(string doc_id)
   public List<Vector2> GetPaddlePositions(string doc_id, string trackKey)
   ```
   Uses Couchbase Lite's QueryBuilder to retrieve recent games ordered by timestamp. `GetBallPositions`/`GetPaddlePositions` decode a replay's position arrays back into Unity `Vector2` frames, so the replay document schema (`ballInfo`, `paddleInfo`, `paddleX`, ...) is defined only in `DatabaseManager`.

3. **Sync Replication**:
   ```csharp
   var remoteURL = new URLEndpoint(new Uri("<url>"));
   var config = new ReplicatorConfiguration(remoteURL);
   // Configuration settings...
   var thisReplicator = new Replicator(config);
   ```
   Configures and starts a replicator to sync local game data to a remote Couchbase Sync Gateway.

4. **Data Lifecycle Management**:
   ```csharp
   public void KeepLatestDocuments(int countToKeep = 10, string timestampField = "timestamp")
   ```
   Manages database size by purging older replays while keeping the most recent ones.

## Data Flow

### Game Recording Process
1. **Initialization**: When a new game starts, a unique ID is generated
2. During gameplay:
   - Ball positions are captured every physics frame in `ReplayManager.ballDetails`
   - Paddle positions for both players are stored in `positions1` and `positions2`
3. **Game Completion**: When a player reaches 7 points, `EndRecording()` is called
4. **Database Storage**: Position data is transformed into Couchbase Lite document format and saved

### Replay Playback Process
1. **Selection**: User selects a replay from the UI list populated by `GetLatestTenReplays()`
2. **Loading**: Selected replay ID is passed to the replay scene via `DatabaseManager.SetId()`
3. **Reconstruction**:
   - `ReplayBall` retrieves ball positions via `DatabaseManager.GetBallPositions()` and replays movement frame by frame
   - `ReplayPlayerPaddle` components do the same for both paddles via `GetPaddlePositions()`
   - Score display is updated based on stored values

### Database Synchronization
1. **Local First**: All game data is stored locally first in the Couchbase Lite database
2. **Background Sync**: Data is automatically pushed to a Couchbase Sync Gateway in the background

## Implementation Details

### Couchbase Lite Setup
```csharp
_db = new Database("pong_db");
_col = _db.CreateCollection("replays");
```

### Document Structure Example
```json
{
  "id": "5eb9f1",
  "paddleInfo": [
    { "paddleX": -7.5, "paddleY": 0.0 },
    { "paddleX": -7.5, "paddleY": 0.2 },
    // Additional frame data...
  ],
  "paddleInfo2": [
    { "paddleX": 7.5, "paddleY": 0.0 },
    // Additional frame data...
  ],
  "ballInfo": [
    { "ballX": 0.0, "ballY": 0.0 },
    { "ballX": 0.1, "ballY": 0.05 },
    // Additional frame data...
  ],
  "timestamp": "2023-05-12T14:30:22.123Z",
  "p1Score": 7,
  "p2Score": 5
}
```

### Query Example (GetLatestTenReplays)
```csharp
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
```

## Benefits of Couchbase Lite Integration

1. **Offline-First Architecture**: Games can be played and replayed without network connectivity
2. **Efficient Data Storage**: Position data is stored as compact dictionary objects
3. **Query Capabilities**: SQL++ query language for flexible data retrieval
4. **Sync Protocol**: Automatic background synchronization with conflict resolution
## Getting Started

### Prerequisites
- Unity 6000.0.44f1 or newer
- [Couchbase Lite for .NET](https://docs.couchbase.com/couchbase-lite/current/csharp/gs-install.html) (The repo already contains the dlls and dylibs used to build this project)

### Trying it out
1. Clone the repository
2. Open the project in Unity
4. Configure the replication URL and authentication in `DatabaseManager.cs` to point to your own Sync Gateway
5. Build and run the game

### Configuration
To set up your own Couchbase Sync Gateway:
1. [Deploy Couchbase Server and Sync Gateway](https://docs.couchbase.com/sync-gateway/current/introduction.html) or [Use App Services with a Capella Cluster](https://docs.couchbase.com/cloud/app-services/index.html)
2. Make sure to have a database named "pong-data"
4. Update the `remoteURL` and credentials in `DatabaseManager.cs` to point to your gateway

## Performance Considerations

- **Fixed Time Interval Capture Rate**: Position data is captured every FixedUpdate frame
- **Document Size Management**: The game stores thousands of position points per match
- **Memory Usage**: Long games generate larger documents, managed via the `KeepLatestDocuments` function, keeping the local database size in check.
- **Synchronization Timing**: Sync occurs in the background to avoid impact on gameplay

## Future Enhancements

- **Replay Controls**: Add pause, fast-forward, and rewind capabilities
- **Online Multiplayer**: Leverage Couchbase sync for real-time multiplayer
