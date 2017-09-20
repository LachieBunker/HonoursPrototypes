using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tute_CartMover : Tute_CharacterClass {

    private bool holdingCart;
    public GameObject cart;
    public Vector3 holdingCartPos;

    // Use this for initialization
    void Start ()
    {
        pRole = PlayerRoles.CartMover;
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
            }
        }

        if (transform.position.y < 0)
        {
            Respawn();
        }
    }

    public override void Interact(GameObject interactionObject, string objectTag)
    {
        if (objectTag == "Cart")
        {
            if (!holdingCart)
            {
                GrabCart(interactionObject);
            }
            else
            {
                ReleaseCart();
            }
        }
    }

    public void GrabCart(GameObject _cart)
    {
        holdingCart = true;
        cart = _cart;
        cart.transform.parent = transform;
        cart.transform.localPosition = holdingCartPos;
        cart.transform.rotation = transform.rotation;

    }

    public void ReleaseCart()
    {
        holdingCart = false;
        cart.transform.parent = null;
        cart = null;

    }
}
