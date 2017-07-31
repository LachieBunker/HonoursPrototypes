using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterClass : NetworkBehaviour {

    public int playerNum;
    public PlayerRoles pRole;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode interactKey;
    public float moveSpeed;
    public float rotSpeed;
    public float interactionRange;
    public GameManager gManager;
    public Transform camPos;
    public bool hasObject;
    public GameObject currentObject;
    public Vector3 holdingObjectPos;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }
    /*
    //Set the player's role
    public void SetRole(PlayerRoles _role)
    {
        pRole = _role;
    }

    //Set the players owner number
    public void SetOwner(int _playerNum)
    {
        playerNum = _playerNum;
    }*/

    //Interact with an object
    public virtual void Interact(GameObject interactionObject, string objectTag)
    {

    }

    //Pick up an object
    public virtual void PickUpObject(GameObject _object)
    {

    }

    //Drop the currentObject
    public virtual void DropObject()
    {

    }

    //Call CmdDespositObject on server
    public virtual void DepositObject(GameObject _deposit)
    {

    }

    //Pick up an object on the server
    [Command]
    public virtual void CmdPickUpObject(GameObject _object)
    {

    }

    //Drop the currentObject on the server
    [Command]
    public virtual void CmdDropObject()
    {

    }

    //Give the currentObject to the interacted _deposit on the server
    [Command]
    public virtual void CmdDepositObject(GameObject _deposit)
    {

    }

    //Respawn the player
    public void Respawn()
    {
        gManager.RespawnPlayer(gameObject);
    }

}

public enum PlayerRoles { CartMover, ObjectCollector }
