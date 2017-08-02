using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ToyCollector : CharacterClass {

    public GameObject object1Prefab;
    public GameObject object2Prefab;

    // Use this for initialization
    void Start ()
    {
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isLocalPlayer)//Child the camera to the local player
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.parent = gameObject.transform;
            cam.transform.position = camPos.position;
            cam.transform.rotation = camPos.rotation;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(isLocalPlayer)
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
                        Debug.Log("interacted" + hit.transform.gameObject + " with tag: " + hit.transform.tag);
                        Interact(hit.transform.gameObject, hit.transform.tag);
                    }
                    else
                    {
                        if(hasObject)
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
        Debug.Log("InteractObject: " + interactionObject + " with tag: " + objectTag);
        if (objectTag.Contains("Toy") && !hasObject)//If the interacted object is a toy and the player doesn't already have a toy
        {
            if (objectTag.Contains(playerNum.ToString()))//If the toy number is the same as the player number
            {
                PickUpObject(interactionObject);
            }
        }
        else if (objectTag == "Converter" && hasObject)//If the interacted object is the converter, and the player has a toy
        {
            if (!interactionObject.GetComponent<Converter>().busy)//If the converter isn't busy
            {
                DepositObject(interactionObject);
            }

        }
        else if (objectTag == "Conveyor" && hasObject)//If the interacted object is the conveyor and the player has a toy
        {
            if (interactionObject.GetComponent<Conveyor>().GetPlayerQueueLength(playerNum) < interactionObject.GetComponent<Conveyor>().queueLimit)//If the queue for the selected toy isn't full
            {
                DepositObject(interactionObject);
            }

        }
        else if (hasObject)
        {
            DropObject();
        }
    }

    //Pick up interacted toy
    public override void PickUpObject(GameObject _object)
    {
        hasObject = true;
        currentObject = _object;
        _object.transform.parent = gameObject.transform;
        _object.transform.localPosition = holdingObjectPos;
        _object.transform.localRotation = Quaternion.Euler(0, 0, 0);
        CmdPickUpObject(_object);
    }

    //Drop currentObject
    public override void DropObject()
    {
        currentObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        RemoveObject(true);
        CmdDropObject();
    }

    //Call CmdDepositObject on server
    public override void DepositObject(GameObject _deposit)
    {
        if(isClient)
        {
            CmdDepositObject(_deposit);
            if (_deposit.tag == "Converter")//If _deposit is the converter, remove currentObject
            {
                RemoveObject(true);
            }
            else if (_deposit.tag == "Conveyor")
            {
                //RemoveObject();
            }
            //Destroy(currentObject);
        }
    }

    //Pick up interacted toy on server
    [Command]
    public override void CmdPickUpObject(GameObject _object)
    {
        if(isServer)
        {
            hasObject = true;
            currentObject = _object;
            _object.transform.parent = gameObject.transform;
            _object.transform.localPosition = holdingObjectPos;
            _object.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //Drop currentObject on server
    [Command]
    public override void CmdDropObject()
    {
        if(isServer)
        {
            currentObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            RemoveObject(true);
        }
    }

    [Command]
    public override void CmdDepositObject(GameObject _deposit)
    {
        if (isServer)
        {
            if (_deposit.tag == "Converter")//If _deposit is the converter, find converter on server, and deposit currentObject into it
            {
                Debug.Log(_deposit + " , " + currentObject);
                _deposit = GameObject.FindGameObjectWithTag("Converter");
                Debug.Log(_deposit + " , " + currentObject);
                _deposit.GetComponent<Converter>().DepositObject(currentObject, playerNum);
                RemoveObject(true);
            }
            else if (_deposit.tag == "Conveyor")//If _deposit is conveyor
            {
                _deposit = GameObject.FindGameObjectWithTag("Conveyor");//Find conveyor on server
                if(_deposit.GetComponent<Conveyor>().GetPlayerQueueLength(playerNum) < _deposit.GetComponent<Conveyor>().queueLimit)//If the queue for the selected toy isn't full
                {
                    //Destory the currentObject, and re-instantiate it on the server
                    if (currentObject.tag.Contains("1"))
                    {
                        Destroy(currentObject);
                        GameObject newOb = (GameObject)Instantiate(object1Prefab, holdingObjectPos, Quaternion.identity);
                        NetworkServer.Spawn(newOb);
                        currentObject = newOb;
                    }
                    else if (currentObject.tag.Contains("2"))
                    {
                        Destroy(currentObject);
                        GameObject newOb = (GameObject)Instantiate(object2Prefab, holdingObjectPos, Quaternion.identity);
                        NetworkServer.Spawn(newOb);
                        currentObject = newOb;
                    }
                    
                    _deposit.GetComponent<Conveyor>().DepositObject(currentObject, playerNum);//Deposit currentObject into conveyor
                    RemoveObject();
                    RpcRemoveObject(false);
                }
                
                
            }
            //Destroy(currentObject);
            
        }
        
    }

    //Remvoes all references to the current object
    public void RemoveObject(bool deParent = false)
    {
        hasObject = false;
        if (currentObject != null)
        {
            if(deParent)
            {
                currentObject.transform.parent = null;
            }
            currentObject = null;
        }
    }

    //Remove the currentObject from the player on the client
    [ClientRpc]
    void RpcRemoveObject(bool deParent)
    {
        if(isClient)
        {
            hasObject = false;
            if (currentObject != null)
            {
                if (deParent)
                {
                    currentObject.transform.parent = null;
                }
                currentObject = null;
            }
        }
    }

    [Command]
    public void CmdLog(string text)
    {
        if(isServer)
        {
            Debug.Log("From Client: " + text);
        }
    }

    [ClientRpc]
    public void RpcLog(string text)
    {
        if(isClient)
        {
            Debug.Log("From Server: " + text);
        }
    }
}
