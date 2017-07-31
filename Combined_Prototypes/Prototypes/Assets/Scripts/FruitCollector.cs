using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FruitCollector : CharacterClass {
    

	// Use this for initialization
	void Start ()
    {
        pRole = PlayerRoles.ObjectCollector;
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isLocalPlayer)//Make the local player blue, and child the camera to them
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.parent = gameObject.transform;
            cam.transform.position = camPos.position;
            cam.transform.rotation = camPos.rotation;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isLocalPlayer)
        {
            if (gManager.playing)//Player movement
            {
                if (Input.GetKey(leftKey))
                {
                    transform.Rotate(0, -rotSpeed, 0);
                }
                else if (Input.GetKey(rightKey))
                {
                    transform.Rotate(0, rotSpeed, 0);
                }
                else if (Input.GetKey(upKey))
                {
                    transform.Translate(0, 0, moveSpeed, Space.Self);
                }
                else if (Input.GetKey(downKey))
                {
                    transform.Translate(0, 0, -moveSpeed, Space.Self);
                }
                if (Input.GetKeyDown(interactKey))
                {
                    RaycastHit hit;
                    Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                    if (Physics.Raycast(pos, transform.forward, out hit, interactionRange))
                    {
                        Debug.DrawLine(pos, hit.transform.position);
                        Debug.Log("interacted" + hit.transform.gameObject);
                        Interact(hit.transform.gameObject, hit.transform.tag);
                    }
                    else
                    {
                        if (hasObject)
                        {
                            DropObject();
                        }
                    }
                }
            }
        }
        
        

        if (transform.position.y < -1)
        {
            Respawn();
        }
    }

    //Interact with an object
    public override void Interact(GameObject interactionObject, string objectTag)
    {
        if (objectTag == "PickUp")
        {
            if (!hasObject)
            {
                PickUpObject(interactionObject);
            }
        }
        else if (objectTag == "Cart")
        {
            if (hasObject)
            {
                DepositObject(interactionObject);
            }
        }
        else
        {
            DropObject();
        }
    }

    //Pick up interacted object
    public override void PickUpObject(GameObject _object)
    {
        hasObject = true;
        currentObject = _object;
        currentObject.transform.parent = transform;
        currentObject.transform.localPosition = holdingObjectPos;
        CmdPickUpObject(_object);
    }

    //Pick up interacted fruit object on the server
    [Command]
    public override void CmdPickUpObject(GameObject _object)
    {
        if (isServer)
        {
            hasObject = true;
            currentObject = _object;
            currentObject.transform.parent = transform;
            currentObject.transform.localPosition = holdingObjectPos;
        }
    }

    //Drop current fruit object
    public override void DropObject()
    {
        currentObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        CmdDropObject();
        RemoveObject();
    }

    //Drop current fruit object on the server
    [Command]
    public override void CmdDropObject()
    {
        if(isServer)
        {
            currentObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            RemoveObject();
        }
    }

    //Destroy current fruit object
    public override void DepositObject(GameObject _deposit)
    {
        if(isClient)
        {
            Debug.Log("Deposit on client");
            CmdDepositObject(currentObject);
            Destroy(currentObject);
            RemoveObject();
        }
        
    }

    //Destroy current fruit object and add fruit to cart on the server
    [Command]
    public override void CmdDepositObject(GameObject _deposit)
    {
        Debug.Log("Deposit");
        if (isServer)
        {
            Debug.Log("Deposit on server" + _deposit);
            _deposit = GameObject.FindGameObjectWithTag("Cart");
            Debug.Log(_deposit);
            _deposit.GetComponent<CartScript>().DepositObject(_deposit, 1);
            Destroy(currentObject);
            RemoveObject();
        }
    }

    //Dettach current fruit object from player
    public void RemoveObject()
    {
        hasObject = false;
        if (currentObject != null)
        {
            currentObject.transform.parent = null;
            currentObject = null;
        }
    }
}
