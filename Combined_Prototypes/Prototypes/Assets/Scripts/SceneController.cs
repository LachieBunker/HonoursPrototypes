using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SceneController : NetworkBehaviour {

    public int playersReady;
    public string nextLevelName;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void PlayerReady(string _pFeedback)
    {
        if (isServer)
        {
            Debug.Log(_pFeedback);
            playersReady++;
            NewNetworkManager nManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NewNetworkManager>();
            nManager.SubmitPlayerFeedback(_pFeedback);
            if (playersReady == 2)//base on number of players?
            {
                NextScene();
            }
        }
    }

    public void PlayerReady()
    {
        if (isServer)
        {
            playersReady++;
            if (playersReady == 2)//base on number of players?
            {
                NextScene();
            }
        }
    }

    public void NextScene()
    {
        NewNetworkManager nManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NewNetworkManager>();
        nManager.ServerChangeScene(nextLevelName);
        
    }
}
