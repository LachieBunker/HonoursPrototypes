using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tute_GameManager : MonoBehaviour {

    public TuteMode tuteMode;
    public bool playing;
    public GameObject objectCollecterPrefab;
    public GameObject cartMoverPrefab;
    public float gameTimer;
    public float gameTimeMax;
    public int fruitCollected;
    public int fruitGoal;
    public GameObject respawnPoint;
    public int numObjectToSpawn;
    public Vector3 objectSpawnRange;
    public GameObject fruitPrefab;
    public GameObject cartPrefab;
    public Text scoreText;
    public Canvas gOverCanvas;

    // Use this for initialization
    void Start ()
    {
        GameSetup();
        UpdateUI();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (tuteMode == TuteMode.CartMover)//set tute mode
        {
            gameTimer -= Time.deltaTime;
            scoreText.text = gameTimer.ToString("f2");
            if (gameTimer <= 0)
            {
                GameOver();
            }
        }
    }

    public void GameSetup()
    {
        Time.timeScale = 1.0f;
        playing = true;
        gameTimer = gameTimeMax;
        SpawnPlayer();
        SpawnFruit();
        Instantiate(cartPrefab, new Vector3(2, 0, 0), Quaternion.identity);

    }

    public void SpawnPlayer()
    {
        if (tuteMode == TuteMode.CartMover)
        {
            Instantiate(cartMoverPrefab, respawnPoint.transform.position, Quaternion.identity);
        }
        else if(tuteMode == TuteMode.ObjectCollector)
        {
            Instantiate(objectCollecterPrefab, respawnPoint.transform.position, Quaternion.identity);
        }
    }

    public void SpawnFruit()
    {
        for (int i = 0; i < numObjectToSpawn; i++)
        {
            float xPos = Random.Range(-objectSpawnRange.x, objectSpawnRange.x);
            float zPos = Random.Range(-objectSpawnRange.z, objectSpawnRange.z);
            Instantiate(fruitPrefab, new Vector3(xPos, 0.5f, zPos), Quaternion.identity);
        }
    }

    public void UpdateUI()
    {
        if (tuteMode == TuteMode.ObjectCollector)
        {
            scoreText.text = fruitCollected.ToString() + "/" + fruitGoal.ToString();
        }
    }

    public void AddFruitToCount(int numFruit)
    {
        fruitCollected += numFruit;
        UpdateUI();
        if (fruitCollected >= fruitGoal)
        {
            GameOver();
        }
    }

    public void RespawnPlayer(GameObject _player)
    {
        Debug.Log("respawn player");
        _player.transform.position = respawnPoint.transform.position;
    }

    public void GameOver()
    {
        playing = false;
        Time.timeScale = 0.0f;
        gOverCanvas.gameObject.SetActive(true);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}

public enum TuteMode { ObjectCollector, CartMover }
