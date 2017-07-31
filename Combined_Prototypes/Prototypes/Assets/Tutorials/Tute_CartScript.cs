using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tute_CartScript : MonoBehaviour {

    public Text[] fruitCountTexts;
    public GameObject fruitFill;
    private Tute_GameManager gManager;

    // Use this for initialization
    void Start ()
    {
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<Tute_GameManager>();
        UpdateFruitCountTexts();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddFruit()
    {
        gManager.AddFruitToCount(1);
        UpdateFruitCountTexts();

    }

    public void UpdateFruitCountTexts()
    {
        for (int i = 0; i < fruitCountTexts.Length; i++)
        {
            fruitCountTexts[i].text = gManager.fruitCollected.ToString() + "/" + gManager.fruitGoal.ToString();
        }
        float fruitMax = gManager.fruitGoal;
        float fruitCur = gManager.fruitCollected;
        fruitFill.transform.localScale = new Vector3(fruitFill.transform.localScale.x, (float)(fruitCur / fruitMax), fruitFill.transform.localScale.z);
    }
}
