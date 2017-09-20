using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    
    public GameObject feedbackCanvas;
    public GameObject preGameCanvas;
    public int playersReady;

    public int localPlayerNum;
    public bool playing;
    public GameObject pRole1Prefab;
    public GameObject pRole2Prefab;
    public bool gameOver;
    public float gameTimer;
    public float gameTimeMax;
    public int objectsCollected;
    public int objectGoal;
    public GameObject respawnPoint;
    public int numObjectsToSpawn;
    public GameObject[] objectPrefabs;
    public Vector3 objectSpawnRange;
    public int objectSpawnMode;
    public Text scoreText;
    public Text timerText;
    public Canvas gOverCanvas;
    public GameObject gameWonPanel;
    public GameObject gameLostPanel;
    public Text gOText;

    public string nextLevelName;

	// Use this for initialization
	void Start ()
    {
        if (isClient)
        {
            //Debug.Log("Client");
            //CmdAddPlayer();
            //playing = true;
        }
        if (isServer)
        {
            //RpcSetClientPNum(2);
            GameSetup();
            UpdateUI();
            StartCoroutine(DelayStart(3));
            playersReady = 0;
        }
	}
    /*
    public override void OnStartServer()
    {
        //Debug.Log("server start");
        //numPlayers = 0;
        //RpcSetClientPNum(2);
    }

    public override void OnStartLocalPlayer()
    {
        //Debug.Log("Local player start");
        //AddPlayer();
    }

    [Command]
    void CmdAddPlayer()
    {
        Debug.Log("CMD add player");
        numPlayers++;
        RpcSetClientPNum(numPlayers);
    }

    [ClientRpc]
    void RpcSetClientPNum(int _pNum)
    {
        localPlayerNum = _pNum;
        Debug.Log(localPlayerNum);
    }
    */
    // Update is called once per frame
    void Update ()
    {
        if(isServer)
        {
            if (playing)//Decrease the timer and sync with client
            {
                gameTimer -= Time.deltaTime;
                timerText.text = gameTimer.ToString("f2");
                RpcSyncTimer(gameTimer);
                if (gameTimer <= 0)
                {
                    GameOver("Lost");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Y) && gameOver)
            {
                Menu();
            }
            if (Input.GetKeyDown(KeyCode.H) && !playing)
            {
                StartGame();
            }
            if (Input.GetKeyDown(KeyCode.J) && !playing)
            {
                StartCoroutine(DelayStart(2));
            }
            if(Input.GetKeyDown(KeyCode.G) && playing)
            {
                GameOver("Won");
            }
        }
        
    }

    //Sync the client timers with the server timer
    [ClientRpc]
    void RpcSyncTimer(float _timer)
    {
        if(isClient)
        {
            gameTimer = _timer;
            timerText.text = gameTimer.ToString("f2");
        }
    }

    //Set up the game before starting
    public void GameSetup()
    {
        if (isServer)
        {
            Time.timeScale = 1.0f;
            playing = false;
            RpcPauseTime();
            SpawnObjects(objectSpawnMode);
        }
        
    }

    //Pause the clients game time
    [ClientRpc]
    void RpcPauseTime()
    {
        if(isClient)
        {
            Time.timeScale = 1.0f;
            playing = false;
        }
    }

    //Start the game after a delay
    public IEnumerator DelayStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartGame();
    }

    //Start the game immediately
    public void StartGame()
    {
        if (isServer)
        {
            playing = true;
            Time.timeScale = 1.0f;
            gameTimer = gameTimeMax;
            RpcStartGame();
        }
    }

    //Start the client game time
    [ClientRpc]
    void RpcStartGame()
    {
        if (isClient)
        {
            playing = true;
            Time.timeScale = 1.0f;
        }
    }

    //Spawn the Object around the area
    public virtual void SpawnObjects(int spawnMode = 0)
    {
        if (spawnMode == 0)//Spawn the first object
        {
            for (int i = 0; i < numObjectsToSpawn; i++)
            {
                float xPos = Random.Range(-objectSpawnRange.x, objectSpawnRange.x);
                float zPos = Random.Range(-objectSpawnRange.z, objectSpawnRange.z);
                NetworkServer.Spawn(Instantiate(objectPrefabs[0], new Vector3(xPos, objectSpawnRange.y, zPos), Quaternion.identity));
            }
        }
        else if (spawnMode == 1)//Spawn each object
        {
            for (int j = 0; j < objectPrefabs.Length; j++)
            {
                for (int i = 0; i < numObjectsToSpawn; i++)
                {
                    float xPos = Random.Range(-objectSpawnRange.x, objectSpawnRange.x);
                    float zPos = Random.Range(-objectSpawnRange.z, objectSpawnRange.z);
                    NetworkServer.Spawn(Instantiate(objectPrefabs[j], new Vector3(xPos, objectSpawnRange.y, zPos), Quaternion.identity));
                }
            }
        }
        else if (spawnMode == 2)//Spawn each object, split between two different zones
        {
            for (int j = 0; j < objectPrefabs.Length; j++)
            {
                int spawnZone = 0;
                ShapeBin objectBin = GameObject.FindGameObjectWithTag("Object" + (j + 1) + "Bin").GetComponent<ShapeBin>();
                for (int i = 0; i < numObjectsToSpawn; i++)
                {
                    if(i > numObjectsToSpawn/2)
                    {
                        spawnZone = 1;
                    }
                    float xPos = Random.Range((objectSpawnRange.x * spawnZone) - objectSpawnRange.x, (objectSpawnRange.x * spawnZone));
                    float zPos = Random.Range(-objectSpawnRange.z, objectSpawnRange.z);
                    NetworkServer.Spawn(Instantiate(objectPrefabs[j], new Vector3(xPos, objectSpawnRange.y, zPos), Quaternion.identity));
                    objectBin.numObjectsToCollect++;
                    if(i == numObjectsToSpawn - 1)
                    {
                        objectBin.UpdateObjectTexts();
                    }
                }
            }
        }
        //Add third spawn mode that spawns random object from prefabs

    }

    //Display the results of the game
    public void DisplayScore()
    {
        if (isServer)
        {

        }
    }

    //Update the UI score
    public void UpdateUI()
    {
        if(isServer)
        {
            scoreText.text = objectsCollected.ToString() + "/" + objectGoal.ToString();
            RpcUpdateUI();
        }
    }

    //Update the clients UI score
    [ClientRpc]
    void RpcUpdateUI()
    {
        if (isClient)
        {
            scoreText.text = objectsCollected.ToString() + "/" + objectGoal.ToString();
        }
    }
    
    /*
    public void PlayerReady()
    {
        if(isLocalPlayer)
        {
            Debug.Log("PRLocal");
            //feedbackCanvas.SetActive(false);
            //CmdPlayerReady();
        }
        else if(isServer)
        {
            Debug.Log("PRServer");
            //feedbackCanvas.SetActive(false);
            playersReady++;
            if (playersReady == 1)
            {
                preGameCanvas.SetActive(false);
                StartGame();
            }
        }
        else if(isClient)
        {
            Debug.Log("PRClient");
            //feedbackCanvas.SetActive(false);
            //CmdPlayerReady();
        }
    }

    [Command]
    public void CmdPlayerReady()
    {
        if(isServer)
        {
            Debug.Log("CPRLocal");
            feedbackCanvas.SetActive(false);
            playersReady++;
            if (playersReady == 1)
            {
                preGameCanvas.SetActive(false);
                StartGame();
            }
        }
        
    }*/

    //Increase the score
    public void AddObjectToCount(int numObject)
    {
        if (isServer)
        {
            objectsCollected += numObject;
            RpcAddObjectToCount(numObject);
            UpdateUI();
            if (objectsCollected >= objectGoal)
            {
                GameOver("Won");
            }
        }
    }

    //Increase the score on the clients
    [ClientRpc]
    void RpcAddObjectToCount(int numObject)
    {
        if (isClient)
        {
            objectsCollected += numObject;
        }
    }

    //Move the player back to the spawn position
    public void RespawnPlayer(GameObject _player)
    {
        Debug.Log("respawn player");
        _player.transform.position = respawnPoint.transform.position;
        RpcRespawnPlayer(_player);
    }

    //Move the player back to the spawn position on the clients
    [ClientRpc]
    void RpcRespawnPlayer(GameObject _player)
    {
        if (isClient)
        {
            _player.transform.position = respawnPoint.transform.position;
        }
    }

    //Stop the game and show the game over screen
    public void GameOver(string gOCon)
    {
        if (gOCon == "Won")
        {
            playing = false;
            gameOver = true;
            Time.timeScale = 1.0f;
            gOverCanvas.gameObject.SetActive(true);
            gameWonPanel.SetActive(true);
            Debug.Log(gOText);
            Debug.Log(gameTimer);
            gOText.text = "Timer: " + gameTimer.ToString("f2");
        }
        else if (gOCon == "Lost")
        {
            playing = false;
            gameOver = true;
            Time.timeScale = 1.0f;
            gOverCanvas.gameObject.SetActive(true);
            gameLostPanel.SetActive(true);
            gOText.text = "Score: " + objectsCollected.ToString();
        }
        RpcGameOver(gOCon);
        StartCoroutine(DelayLoadLevel());
    }

    //Pause the game and show the game over screen on the clients
    [ClientRpc]
    void RpcGameOver(string gOCon)
    {
        if (isClient)
        {
            if (gOCon == "Won")
            {
                playing = false;
                gameOver = true;
                Time.timeScale = 1.0f;
                gOverCanvas.gameObject.SetActive(true);
                gameWonPanel.SetActive(true);
                gOText.text = "Timer: " + gameTimer.ToString("f2");
            }
            else if (gOCon == "Lost")
            {
                playing = false;
                gameOver = true;
                Time.timeScale = 1.0f;
                gOverCanvas.gameObject.SetActive(true);
                gameLostPanel.SetActive(true);
                gOText.text = "Score: " + objectsCollected.ToString();
            }
        }
    }

    //Go to the menu
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel()
    {
        NewNetworkManager nManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NewNetworkManager>();
        nManager.ServerChangeScene(nextLevelName);
    }

    public IEnumerator DelayLoadLevel()
    {
        yield return new WaitForSeconds(3);
        LoadLevel();
    }
}
