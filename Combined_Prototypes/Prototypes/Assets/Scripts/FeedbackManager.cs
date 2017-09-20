using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedbackManager : NetworkBehaviour {

    public List<Slider> feedbackSliders;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1.0f;
        if(!isLocalPlayer)
        {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        if (isLocalPlayer)
        {
            Debug.Log("LocalPlayer");
            gameObject.SetActive(false);
            CmdStartGame();
        }
    }

    [Command]
    public void CmdStartGame()
    {
        SceneController sManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SceneController>();
        sManager.PlayerReady();
    }

    public string GetPlayerFeedback()
    {
        string _feedback = "";
        for(int i = 0; i < feedbackSliders.Count; i++)
        {
            _feedback = _feedback + feedbackSliders[i].value.ToString("f2");
            if(i < feedbackSliders.Count -1)
            {
                _feedback = _feedback + ",";
            }
        }
        return _feedback;
    }

    public void PlayerReady()
    {
        if(isLocalPlayer)
        {
            Debug.Log("LocalPlayer");
            string temp = GetPlayerFeedback();
            CmdPlayerReady(temp);
            gameObject.SetActive(false);
        }
        else if(isClient)
        {
            Debug.Log("Client");
        }
        
    }

    [Command]
    public void CmdPlayerReady(string _feedback)
    {
        string _fback = connectionToClient.connectionId.ToString() + ":" + _feedback;
        SceneController sManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SceneController>();
        sManager.PlayerReady(_fback);
    }
}
