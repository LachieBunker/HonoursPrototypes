using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CartMover : CharacterClass {
    

    // Use this for initialization
    void Start ()
    {
        pRole = PlayerRoles.CartMover;
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if(isLocalPlayer)//Make the local player blue, and child the camera to them
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
        if (objectTag == "Cart")
        {
            if (!hasObject)
            {
                PickUpObject(interactionObject);
            }
            else
            {
                Debug.Log("Drop object");
                DropObject();
            }
        }
        else if (hasObject)
        {
            DropObject();
        }
    }

    //Attach the cart to the player
    public override void PickUpObject(GameObject _object)
    {
        if (isClient)
        {
            hasObject = true;
            currentObject = _object;
            currentObject.transform.parent = transform;
            currentObject.transform.localPosition = holdingObjectPos;
            currentObject.transform.rotation = transform.rotation;
            CmdPickUpObject(_object);
        }
        
    }

    //Attach the cart to the player on the server
    [Command]
    public override void CmdPickUpObject(GameObject _object)
    {
        if(isServer)
        {
            hasObject = true;
            currentObject = _object;
            currentObject.transform.parent = transform;
            currentObject.transform.localPosition = holdingObjectPos;
            currentObject.transform.rotation = transform.rotation;
        }
    }

    //Dettach the cart from the player
    public override void DropObject()
    {
        if (isClient)
        {
            hasObject = false;
            currentObject.transform.parent = null;
            currentObject = null;
            CmdDropObject();
        }
        
    }

    //Dettach the cart from the player on the server
    [Command]
    public override void CmdDropObject()
    {
        if(isServer)
        {
            hasObject = false;
            currentObject.transform.parent = null;
            currentObject = null;
        }
    }
}