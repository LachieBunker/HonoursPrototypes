using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CartScript : baseInteractionObjectClass {
    
    public GameObject fruitFill;

	// Use this for initialization
	void Start ()
    {
        if(isServer)
        {
            gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            UpdateObjectTexts();
        }
        //gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (isClient)
        {
            //CmdUpdateScore(2);
            gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            RpcUpdateObjectTexts(gManager.objectsCollected + "/" + gManager.objectGoal);
        }
    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    //Add fruit to gameManager
    public override void DepositObject(GameObject _object, int obNum)
    {
        if(isServer)
        {
            gManager.AddObjectToCount(1);
            UpdateObjectTexts();
            RpcDepositObject(_object, obNum);
            
            //UpdateFruitCountTexts();
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
        if (isServer)
        {
            string obText = gManager.objectsCollected.ToString() + "/" + gManager.objectGoal.ToString();
            for (int i = 0; i < objectTexts.Length; i++)
            {
                objectTexts[i].text = obText;
            }
            float fruitMax = gManager.objectGoal;
            float fruitCur = gManager.objectsCollected;
            fruitFill.transform.localScale = new Vector3(fruitFill.transform.localScale.x, (float)(fruitCur / fruitMax), fruitFill.transform.localScale.z);
            RpcUpdateObjectTexts(obText);
        }
    }

    //Update the text panels to show score on clients(called from server)
    [ClientRpc]
    public override void RpcUpdateObjectTexts(string _text)
    {
        if(isClient)
        {
            for (int i = 0; i < objectTexts.Length; i++)
            {
                objectTexts[i].text = _text;
            }
            
            string[] fruitScores = _text.Split('/');
            float fruitCur = int.Parse(fruitScores[0]);
            float fruitMax = int.Parse(fruitScores[1]);
            fruitFill.transform.localScale = new Vector3(fruitFill.transform.localScale.x, (float)(fruitCur / fruitMax), fruitFill.transform.localScale.z);
        }
    }
}
