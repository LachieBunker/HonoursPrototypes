using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NewNetworkManager : NetworkManager {

    public GameObject playerRole1Prefab;
    public GameObject playerRole2Prefab;
    public Vector3 playerSpawnPos;
    public int numCurrentPlayers;
    private int pRole1Num;
    private int pRole2Num;

	// Use this for initialization
	void Start () {
        numCurrentPlayers = 0;
        SetPlayerRoles();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Set which player will have which role
    public void SetPlayerRoles()
    {
        int p1Role = Random.Range(0, 2);
        if (p1Role == 0)//mover;
        {
            pRole1Num = 1;
            pRole2Num = 2;
        }
        else if (p1Role == 1)//collector
        {
            pRole2Num = 1; ;
            pRole1Num = 2;
        }
    }

    //When a player joins, spawn the corresponding player character, as set above
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        numCurrentPlayers++;
        GameObject prefabToSpawn = new GameObject();
        if (numCurrentPlayers == pRole1Num)
        {
            prefabToSpawn = playerRole1Prefab;
        }
        else if (numCurrentPlayers == pRole2Num)
        {
            prefabToSpawn = playerRole2Prefab;
        }
        
        //prefabToSpawn = moverPrefab;
        var player = (GameObject)GameObject.Instantiate(prefabToSpawn, new Vector3(-6 + (4 * numCurrentPlayers), 0, 0), Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        
    }

    //When a player leaves, reduce the number of current players by one, and remove their character from the server
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        numCurrentPlayers--;
        base.OnServerRemovePlayer(conn, player);
    }
}
