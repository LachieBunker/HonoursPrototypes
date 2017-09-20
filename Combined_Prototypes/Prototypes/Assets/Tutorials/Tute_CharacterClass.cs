using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tute_CharacterClass : CharacterClass {

    public GameObject object1Prefab;
    public GameObject object2Prefab;

    public Vector3 playerBounds;

    // Use this for initialization
    void Start()
    {
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isLocalPlayer)//Child the camera to the local player
        {
            Debug.Log(gManager);
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.parent = gameObject.transform;
            cam.transform.position = camPos.position;
            cam.transform.rotation = camPos.rotation;
        }
    }

    // Update is called once per frame
    void Update()
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
                        Debug.Log("interacted" + hit.transform.gameObject + " with tag: " + hit.transform.tag);
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

        if (transform.position.x < -playerBounds.x)
        {
            transform.position = new Vector3(-playerBounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.x > playerBounds.y)
        {
            transform.position = new Vector3(playerBounds.y, transform.position.y, transform.position.z);
        }
        if (transform.position.z < -playerBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -playerBounds.z);
        }
        if (transform.position.z > playerBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerBounds.z);
        }
    }

    //Interact with an object
    public override void Interact(GameObject interactionObject, string objectTag)
    {
        Debug.Log("InteractObject: " + interactionObject + " with tag: " + objectTag);
        if (objectTag.Contains("Toy") && !hasObject)//If the interacted object is a toy and the player doesn't already have a toy
        {
            PickUpObject(interactionObject);
        }
        else if ((objectTag == "Object1" || objectTag == "Object2") && !hasObject)//If the interacted object is a toy and the player doesn't already have a toy
        {
            PickUpObject(interactionObject);
        }
        else if (objectTag == "PickUp" && !hasObject)
        {
            PickUpObject(interactionObject);
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
            DepositObject(interactionObject);

        }
        else if (objectTag.Contains("Bin") && hasObject)//If the interacted object is the converter, and the player has a toy
        {
            if (objectTag.Contains(currentObject.tag))//If the converter isn't busy
            {
                DepositObject(interactionObject);
            }

        }
        else if (objectTag == "Cart" && hasObject)
        {
            DepositObject(interactionObject);
        }
        else if(objectTag == "Cart" && !hasObject)
        {
            PickUpObject(interactionObject);
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
        if (currentObject.tag != "Cart")
        {
            _object.transform.localPosition = holdingObjectPos;
        }
        _object.transform.localRotation = Quaternion.Euler(0, 0, 0);
        CmdPickUpObject(_object);
    }

    //Drop currentObject
    public override void DropObject()
    {
        if (currentObject.tag == "Object1" || currentObject.tag == "Object2")
        {
            currentObject.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        }
        else if (currentObject.tag != "Cart")
        {
            currentObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        }
        RemoveObject(true);
        CmdDropObject();
    }

    //Call CmdDepositObject on server
    public override void DepositObject(GameObject _deposit)
    {
        if (isClient)
        {
            CmdDepositObject(_deposit);
            if (currentObject.tag.Contains("Toy"))
            {
                if (_deposit.tag == "Converter")//If _deposit is the converter, remove currentObject
                {
                    RemoveObject(true);
                }
                else if (_deposit.tag == "Conveyor")
                {
                    //RemoveObject();
                }
            }
            else if(currentObject.tag == "Object1" || currentObject.tag == "Object2")
            {
                if (_deposit.tag.Contains("Bin"))
                {
                    Destroy(currentObject);
                    RemoveObject(true);
                }
            }
            else if(currentObject.tag == "PickUp")
            {
                if(_deposit.tag == "Cart")
                {
                    Destroy(currentObject);
                    RemoveObject(true);
                }
            }
            //Destroy(currentObject);
        }
    }

    //Pick up interacted toy on server
    [Command]
    public override void CmdPickUpObject(GameObject _object)
    {
        if (isServer)
        {
            hasObject = true;
            currentObject = _object;
            _object.transform.parent = gameObject.transform;
            if (currentObject.tag != "Cart")
            {
                _object.transform.localPosition = holdingObjectPos;
            }
            _object.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //Drop currentObject on server
    [Command]
    public override void CmdDropObject()
    {
        if (isServer)
        {
            if(currentObject.tag == "Object1" || currentObject.tag == "Object2")
            {
                currentObject.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            }
            else if(currentObject.tag != "Cart")
            {
                currentObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

            }
            RemoveObject(true);
        }
    }

    [Command]
    public override void CmdDepositObject(GameObject _deposit)
    {
        if (isServer)
        {
            if (currentObject.tag.Contains("Toy"))
            {
                if (_deposit.tag == "Converter")//If _deposit is the converter, remove currentObject
                {
                    _deposit = GameObject.FindGameObjectWithTag("Converter");
                    Debug.Log(_deposit + " , " + currentObject);
                    int temp = 1;
                    if(currentObject.tag.Contains("1"))
                    {
                        temp = 1;
                    }
                    else
                    {
                        temp = 2;
                    }
                    _deposit.GetComponent<Converter>().DepositObject(currentObject, temp);
                    RemoveObject(true);
                }
                else if (_deposit.tag == "Conveyor")
                {
                    _deposit = GameObject.FindGameObjectWithTag("Conveyor");//Find conveyor on server
                    int temp = 1;
                    if (currentObject.tag.Contains("1"))
                    {
                        temp = 1;
                    }
                    else
                    {
                        temp = 2;
                    }
                    if (_deposit.GetComponent<Conveyor>().GetPlayerQueueLength(temp) < _deposit.GetComponent<Conveyor>().queueLimit)//If the queue for the selected toy isn't full
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

                        _deposit.GetComponent<Conveyor>().DepositObject(currentObject, temp);//Deposit currentObject into conveyor
                        RemoveObject();
                        RpcRemoveObject(false);
                    }
                }
            }
            else if (currentObject.tag == "Object1" || currentObject.tag == "Object2")
            {
                if (_deposit.tag == "Object1Bin")//If _deposit is the converter, find converter on server, and deposit currentObject into it
                {
                    Debug.Log(_deposit + " , " + currentObject);
                    _deposit = GameObject.FindGameObjectWithTag("Object1Bin");
                    Debug.Log(_deposit + " , " + currentObject);
                    if (currentObject.tag.Contains("1"))
                    {
                        _deposit.GetComponent<ShapeBin>().DepositObject(currentObject, playerNum);
                        Destroy(currentObject);
                        RemoveObject(true);
                    }

                }
                else if (_deposit.tag == "Object2Bin")//If _deposit is the converter, find converter on server, and deposit currentObject into it
                {
                    Debug.Log(_deposit + " , " + currentObject);
                    _deposit = GameObject.FindGameObjectWithTag("Object2Bin");
                    Debug.Log(_deposit + " , " + currentObject);
                    if (currentObject.tag.Contains("2"))
                    {
                        _deposit.GetComponent<ShapeBin>().DepositObject(currentObject, playerNum);
                        Destroy(currentObject);
                        RemoveObject(true);
                    }
                }
            }
            else if (currentObject.tag == "PickUp")
            {
                if (_deposit.tag == "Cart")
                {
                    _deposit = GameObject.FindGameObjectWithTag("Cart");
                    _deposit.GetComponent<CartScript>().DepositObject(_deposit, 1);
                    Destroy(currentObject);
                    RemoveObject(true);
                }
            }

        }

    }

    //Remvoes all references to the current object
    public void RemoveObject(bool deParent = false)
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

    //Remove the currentObject from the player on the client
    [ClientRpc]
    void RpcRemoveObject(bool deParent)
    {
        if (isClient)
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
}
