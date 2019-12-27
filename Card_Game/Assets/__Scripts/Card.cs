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
