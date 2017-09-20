using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tute_ObjectCollector : Tute_CharacterClass {

    private bool hasFruit;
    public GameObject currentFruit;
    public Vector3 holdingFruitPos;

    // Use this for initialization
    void Start ()
    {
        pRole = PlayerRoles.ObjectCollector;
        //gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<Tute_GameManager>();
        GetComponent<MeshRenderer>().material.color = Color.blue;
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.parent = gameObject.transform;
        cam.transform.position = camPos.position;
        cam.transform.rotation = camPos.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gManager.playing)
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
                    if (hasFruit)
                    {
                        DropFruit();
                    }
                }
            }
        }

        if (transform.position.y < 0)
        {
            Respawn();
        }
    }

    public override void Interact(GameObject interactionObject, string objectTag)
    {
        if (objectTag == "PickUp")
        {
            if (!hasFruit)
            {
                PickUpFruit(interactionObject);
            }
        }
        else if (objectTag == "Cart")
        {
            if (hasFruit)
            {
                DepositFruit(interactionObject);
            }
        }
        else
        {
            DropFruit();
        }
    }

    public void PickUpFruit(GameObject _fruit)
    {
        hasFruit = true;
        currentFruit = _fruit;
        currentFruit.transform.parent = transform;
        currentFruit.transform.localPosition = holdingFruitPos;
    }

    public void DropFruit()
    {
        currentFruit.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        RemoveFruit();
    }

    public void DepositFruit(GameObject _cart)
    {
        _cart.GetComponent<Tute_CartScript>().AddFruit();
        Destroy(currentFruit);
        RemoveFruit();
    }

    public void RemoveFruit()
    {
        hasFruit = false;
        if (currentFruit != null)
        {
            currentFruit.transform.parent = null;
        }
        currentFruit = null;
    }
}
