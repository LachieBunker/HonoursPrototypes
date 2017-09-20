using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ShapeBin : baseInteractionObjectClass {

    public int numObjectsToCollect;
    public int numObjectsCollected;

	// Use this for initialization
	void Start ()
    {
        if (isServer)
        {
            gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            if(gameObject.tag.Contains("1"))
            {
                //numObjectsToCollect = GameObject.FindGameObjectsWithTag("Object1").Length;
            }
            else if (gameObject.tag.Contains("2"))
            {
                //numObjectsToCollect = GameObject.FindGameObjectsWithTag("Object2").Length;
            }
            UpdateObjectTexts();
        }
        //gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isClient)
        {
            //CmdUpdateScore(2);
            gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            if (gameObject.tag.Contains("1"))
            {
                //numObjectsToCollect = GameObject.FindGameObjectsWithTag("Object1").Length;
            }
            else if (gameObject.tag.Contains("2"))
            {
                //numObjectsToCollect = GameObject.FindGameObjectsWithTag("Object2").Length;
            }
            UpdateObjectTexts();
            //RpcUpdateObjectTexts(numObjectsCollected + "/" + numObjectsToCollect);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Add fruit to gameManager
    public override void DepositObject(GameObject _object, int obNum)
    {
        if (isServer)
        {
            if(gameObject.tag.Contains(_object.tag))
            {
                numObjectsCollected += 1;
                gManager.AddObjectToCount(1);
                UpdateObjectTexts();
                RpcDepositObject(_object, obNum);
            }
            
        }

    }

    //Add fruit to score
    [ClientRpc]
    public override void RpcDepositObject(GameObject _object, int obNum)
    {
        if (isClient)
        {


        }
    }

    //Update the text panels to show score
    public virtual void UpdateObjectTexts()
    {
        string obText = numObjectsCollected.ToString() + "/" + numObjectsToCollect.ToString();
        for (int i = 0; i < objectTexts.Length; i++)
        {
            objectTexts[i].text = obText;
        }
        if (isServer)
        {
            RpcUpdateObjectTexts(obText);
        }
    }

    //Update the text panels to show score on clients(called from server)
    [ClientRpc]
    public override void RpcUpdateObjectTexts(string _text)
    {
        if (isClient)
        {
            for (int i = 0; i < objectTexts.Length; i++)
            {
                objectTexts[i].text = _text;
            }
        }
    }
}
