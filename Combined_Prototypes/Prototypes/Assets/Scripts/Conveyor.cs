using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Conveyor : baseInteractionObjectClass {

    public int queueLimit;
    public List<GameObject> object1Queue;
    public List<GameObject> object2Queue;
    public Vector3 object1Pos;
    public Vector3 object2Pos;

	// Use this for initialization
	void Start ()
    {
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Add object to conveyor
    public override void DepositObject(GameObject _object, int obNum)
    {
        Debug.Log("Conveyor deposit: " + _object);
        if(isServer)
        {
            //Add object to corresponding queue, and position queue
            if (obNum == 1)
            {
                object1Queue.Add(_object);
                _object.transform.parent = gameObject.transform;
                _object.transform.localPosition = new Vector3(object1Pos.x, object1Pos.y, 1 + (object1Pos.z * object1Queue.Count-1));
                _object.transform.rotation = Quaternion.Euler(0, -90, 0);
                PositionObjectQueue(object1Queue, 1);
            }
            else if (obNum == 2)
            {
                object2Queue.Add(_object);
                _object.transform.parent = gameObject.transform;
                _object.transform.localPosition = new Vector3(object2Pos.x, object2Pos.y, 1+ (object2Pos.z * object2Queue.Count-1));
                _object.transform.rotation = Quaternion.Euler(0, -90, 0);
                PositionObjectQueue(object2Queue, 2);
            }
            UpdateObjectTexts("string");
            //RpcDepositObject(_object, obNum);

            if (object1Queue.Count > 0 && object2Queue.Count > 0)//If there is at least one object in each queue
            {
                DeliverObjects();
            }
        }
    }

    //Update UI texts of object with queue capacity
    public override void UpdateObjectTexts(string _text)
    {
        if(isServer)
        {
            objectTexts[0].text = object1Queue.Count + "/" + queueLimit;
            objectTexts[1].text = object2Queue.Count + "/" + queueLimit;
            string tempText = object1Queue.Count + "/" + queueLimit + ',' + object2Queue.Count + "/" + queueLimit;
            Debug.Log(tempText);
            RpcUpdateObjectTexts(tempText);
        }
    }

    //Add object to conveyor on client
    [ClientRpc]
    public override void RpcDepositObject(GameObject _object, int obNum)
    {
        if(isClient)
        {
            //Add object to corresponding queue, and position queue
            if (obNum == 1)
            {

                object1Queue.Add(_object);
                _object.transform.parent = gameObject.transform;
                _object.transform.position = new Vector3(object1Pos.x, object1Pos.y, 1 + (object1Pos.z * object1Queue.Count));
                _object.transform.rotation = Quaternion.Euler(0, -90, 0);
                PositionObjectQueue(object1Queue, 1);
            }
            else if (obNum == 2)
            {
                object2Queue.Add(_object);
                _object.transform.parent = gameObject.transform;
                _object.transform.position = new Vector3(object2Pos.x, object2Pos.y, 1 + (object2Pos.z * object2Queue.Count));
                _object.transform.rotation = Quaternion.Euler(0, -90, 0);
                PositionObjectQueue(object2Queue, 2);
            }
        }
    }

    //Update UI texts of object with queue capacity on client
    [ClientRpc]
    public override void RpcUpdateObjectTexts(string _text)
    {
        if (isClient)
        {
            string[] obScores = _text.Split(',');
            //float queue1 = int.Parse(obScores[0]);
            //float queue2 = int.Parse(obScores[1]);
            objectTexts[0].text = obScores[0];
            objectTexts[1].text = obScores[1];
        }
    }

    //Position each object in selected queue
    public void PositionObjectQueue(List<GameObject> obQueue, int obNum)
    {
        Vector3 pos = new Vector3();
        if (obNum == 1)
        {
            pos = object1Pos;
        }
        else if (obNum == 2)
        {
            pos = object2Pos;
        }
        for (int i = 0; i < obQueue.Count; i++)
        {
            obQueue[i].transform.localPosition = new Vector3(pos.x, pos.y, 1 + (pos.z * i));
        }
    }

    //Deliver objects to gManager
    public void DeliverObjects()
    {
        if(isServer)
        {
            int numObjects = 0;
            while (object1Queue.Count > 0 && object2Queue.Count > 0)
            {
                //If there is atleast one object in each queue, remove the first object from each queue, and add one score to gManager
                gManager.AddObjectToCount(1);
                Debug.Log(object1Queue[0]);
                Destroy(object1Queue[0]);
                Destroy(object2Queue[0]);
                object1Queue.RemoveAt(0);
                object2Queue.RemoveAt(0);
                numObjects++;
            }
            //Position the objects in each queue
            PositionObjectQueue(object1Queue, 1);
            PositionObjectQueue(object2Queue, 2);
            //RpcDeliverObjects(numObjects);
            UpdateObjectTexts("string");
        }
        
    }

    //Sync teh object queues on client with server
    [ClientRpc]
    public void RpcDeliverObjects(int numObjects)
    {
        if(isClient)
        {
            for (int i = 0; i < numObjects; i++)
            {
                object1Queue.RemoveAt(0);
                object2Queue.RemoveAt(0);
            }
            PositionObjectQueue(object1Queue, 1);
            PositionObjectQueue(object2Queue, 2);
        }
        
    }

    //Return the length of selected queue
    public int GetPlayerQueueLength(int pNum)
    {
        if (pNum == 1)
        {
            return object1Queue.Count;
        }
        else if (pNum == 2)
        {
            return object2Queue.Count;
        }
        return 0;
    }
}
