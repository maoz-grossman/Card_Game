using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;//(C,D,H,or S)
    public int rank;//(1-14)
    public Color color = Color.black;//color to tint pips
    public string colS = "Black";//or Red. Name thr color

    //this list hold all of thre Decorator GameObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    //this list holds all thr pip GameObject
    public List<GameObject> pipGOs = new List<GameObject>();
    public GameObject back;//the GameObject of the back of the card
    public CardDefinition def;//parsed from DeckXML.xml
    //List of the SpriteRenderer Components of this GameObject
    public SpriteRenderer[] spriteRenderers;

     void Start()
    {
        SetSortOrder(0);
    }

    //If spriteRenderers is not yet define, this function defins it 
    public void PopulateSpriteRenderers()
    {
        // If spriteRenderers is null or empty
        if(spriteRenderers==null||spriteRenderers.Length==0)
        {
            //Get SpriteRenderer Components of this GameObject and its children
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    //Sets the sortingLayerName on all SpriteRenderer Components
    public void SetSortingLayerName(string tSLN)
    {
        PopulateSpriteRenderers();
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }

    //Sets the sortingOreder of all SpriteRenderer Components
    public void SetSortOrder(int sOrd)
    {
        PopulateSpriteRenderers();

        //Iterate through all the spriteRenderers as tSR
        foreach(SpriteRenderer tSR in spriteRenderers)
        {
            //If the gmeObject is this.gameObject,it's the backgroud
            if (tSR.gameObject == this.gameObject)
            {
                tSR.sortingOrder = sOrd;//Set it's order to sOrd
                continue;//And continue to the next iteration of the loop
            }
           //Each of the children of this GameObject are named
           //switch based on the names
           switch(tSR.gameObject.name)
            {
                case "back":
                    //Set it to the highest layer to cover the other sprites
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "face"://if the name is "face"
                default:// or if it's anything else
                    //Set it to the middle layer to be above the background
                    tSR.sortingOrder = sOrd + 1;
                    break;
            }
        }
    }



    public bool faceUp
    {
        get
        {
            return (!back.activeSelf);
        }
        set
        {
            back.SetActive(!value);
        }
    }

}


[System.Serializable]
/* A Serializable class is able to be 
 edited in the Inspector*/
public class Decorator
{
    //this class srotes information about each decorator or pip from DeckXML
    public string type; //for card pips, type="pip"
    public Vector3 loc;//the location of the Sprite on the Card
    public bool flip = false;//whether to flip the Sprite vertically
    public float scale = 1f;//the scale of the Sprite
}

[System.Serializable]
public class CardDefinition
{
    //this class stores information for each rank of card
    public string face;//sprite to use for each card
    public int rank;//the rank(1-13) of thus card
    public List<Decorator> pips = new List<Decorator>();//pips used
}
