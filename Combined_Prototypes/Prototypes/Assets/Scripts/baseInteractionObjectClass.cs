using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class baseInteractionObjectClass : NetworkBehaviour {

    public Text[] objectTexts;
    public GameManager gManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Give an object to the interaction object
    public virtual void DepositObject(GameObject _object, int obNum)
    {

    }

    //Update the UI texts of the object
    public virtual void UpdateObjectTexts(string _text)
    {

    }

    //Sync the client with the server after recieving an object
    [ClientRpc]
    public virtual void RpcDepositObject(GameObject _object, int obNum)
    {

    }

    //Sync the UI texts on the client with the server
    [ClientRpc]
    public virtual void RpcUpdateObjectTexts(string _text)
    {

    }
}
