using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NewNetworkManager : NetworkManager {

    public int levelNum;
    public GameObject[] playerRole1Prefab;
    public GameObject[] playerRole2Prefab;
    public Vector3 playerSpawnPos;
    public int numCurrentPlayers;
    private int pRole1Num;
    private int pRole2Num;
    public bool isClient;

    public List<string> player1Feedback = new List<string>();
    public List<string> player2Feedback = new List<string>();

	// Use this for initialization
	void Start () {
        numCurrentPlayers = 0;
        levelNum = 0;
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
        //GameManager gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //Debug.Log("server add player" + gManager.pRole1Prefab);
        numCurrentPlayers++;
        GameObject prefabToSpawn = new GameObject();
        if (numCurrentPlayers == pRole1Num)
        {
            prefabToSpawn = playerRole1Prefab[levelNum];
        }
        else if (numCurrentPlayers == pRole2Num)
        {
            prefabToSpawn = playerRole2Prefab[levelNum];
        }
        
        //prefabToSpawn = moverPrefab;
        var player = (GameObject)GameObject.Instantiate(prefabToSpawn, new Vector3(-6 + (4 * numCurrentPlayers), 0, 0), Quaternion.identity);
        Debug.Log("Player: " + player);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        
    }

    //When a player leaves, reduce the number of current players by one, and remove their character from the server
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        numCurrentPlayers--;
        base.OnServerRemovePlayer(conn, player);
    }

    private void OnLevelWasLoaded(int level)
    {
        levelNum++;
        numCurrentPlayers = 0;
        SetPlayerRoles();
        //StartCoroutine(DelaySetPRoles());
        if (isClient)
        {
            ClientScene.Ready(ClientScene.readyConnection);
        }
    }

    IEnumerator DelaySetPRoles()
    {
        yield return new WaitForEndOfFrame();//(0.001f);
        GameManager gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Debug.Log("Level loaded" + gManager);
        //playerRole1Prefab = gManager.pRole1Prefab;
        //playerRole2Prefab = gManager.pRole2Prefab;
        SetPlayerRoles();
    }

    public void SubmitPlayerFeedback(string _feedback)
    {
        Debug.Log("NM recieved: " + _feedback);
        string[] chars = _feedback.Split(':');
        if(chars[0] == "1")
        {
            player1Feedback.Add(_feedback);
        }
        else if(chars[0] == "2")
        {
            player2Feedback.Add(_feedback);
        }
        else
        {
            Debug.Log("Player num: " + chars[0]);
        }
    }
}
