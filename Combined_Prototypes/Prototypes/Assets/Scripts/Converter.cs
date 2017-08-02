using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Converter : baseInteractionObjectClass {

    public bool busy;
    public GameObject object1Prefab;
    public GameObject object2Prefab;
    public float convertDuration;
    public Material activeMat;
    public Material inactiveMat;
    public Vector3 outputPos;
    Conveyor conveyor;

	// Use this for initialization
	void Start ()
    {
		if(isServer)
        {
            conveyor = GameObject.FindGameObjectWithTag("Conveyor").GetComponent<Conveyor>();
            UpdateObjectTexts("Empty");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void DepositObject(GameObject _object, int obNum)
    {
        if (isServer)
        {
            busy = true;
            gameObject.GetComponent<MeshRenderer>().material = activeMat;
            RpcSyncClient(true);
            _object.transform.parent = gameObject.transform;/*
            _object.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            _object.tag = "Untagged";*/
            UpdateObjectTexts("Busy");
            StartCoroutine(Convert(_object, obNum));
        }
    }

    public override void UpdateObjectTexts(string _text)
    {
        for (int i = 0; i < objectTexts.Length; i++)
        {
            objectTexts[i].text = _text;
        }
        RpcUpdateObjectTexts(_text);
    }

    [ClientRpc]
    public override void RpcDepositObject(GameObject _object, int obNum)
    {
        
    }

    [ClientRpc]
    public override void RpcUpdateObjectTexts(string _text)
    {
        for (int i = 0; i < objectTexts.Length; i++)
        {
            objectTexts[i].text = _text;
        }
    }

    private IEnumerator Convert(GameObject _object, int objectNum)
    {
        if(isServer)
        {
            Destroy(_object);
            yield return new WaitForSeconds(convertDuration);
            busy = false;
            gameObject.GetComponent<MeshRenderer>().material = inactiveMat;
            RpcSyncClient(false);
            if (objectNum == 1)
            {
                GameObject newOb = (GameObject)Instantiate(object2Prefab, outputPos, Quaternion.identity);
                NetworkServer.Spawn(newOb);
                if (conveyor.GetPlayerQueueLength(2) < conveyor.queueLimit)
                {
                    conveyor.DepositObject(newOb, 2);
                }
            }
            else if (objectNum == 2)
            {
                GameObject newOb = (GameObject)Instantiate(object1Prefab, outputPos, Quaternion.identity);
                NetworkServer.Spawn(newOb);
                if (conveyor.GetPlayerQueueLength(1) < conveyor.queueLimit)
                {
                    conveyor.DepositObject(newOb, 1);
                }
            }
            UpdateObjectTexts("Empty");
        }
        
    }

    [ClientRpc]
    public void RpcSyncClient(bool _busy)
    {
        if(isClient)
        {
            busy = _busy;
            if(busy)
            {
                gameObject.GetComponent<MeshRenderer>().material = activeMat;
            }
            else if(!busy)
            {
                gameObject.GetComponent<MeshRenderer>().material = inactiveMat;
            }
        }
    }
}
