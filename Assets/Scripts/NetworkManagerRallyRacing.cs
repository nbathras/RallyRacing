using Mirror;
using System.Collections.Generic;
using UnityEngine;

// Custom NetworkManager that simply assigns the correct racket positions when
// spawning players. The built in RoundRobin spawn method wouldn't work after
// someone reconnects (both players would be on the same side).
[AddComponentMenu("")]
public class NetworkManagerRallyRacing : NetworkManager {
    public List<Transform> playerSpawnPositionList;

    public override void OnServerAddPlayer(NetworkConnection conn) {
        // add player at correct spawn position
        Transform spawnPosition = playerSpawnPositionList[numPlayers];
        GameObject player = Instantiate(playerPrefab, spawnPosition.position, spawnPosition.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        // spawn ball if two players
        /*
        if (numPlayers == 2) {
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            NetworkServer.Spawn(ball);
        }
        */
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        // destroy ball
        /*
        if (ball != null) {
            NetworkServer.Destroy(ball);
        }
        */

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
