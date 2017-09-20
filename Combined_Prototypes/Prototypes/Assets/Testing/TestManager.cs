using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour {

    public GameObject pRole1Prefab;
    public GameObject pRole2Prefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLevel()
    {
        NewNetworkManager nManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NewNetworkManager>();
        nManager.ServerChangeScene("TestScene2");
    }
}
