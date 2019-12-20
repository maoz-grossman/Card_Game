using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //this class will be defined later
}

[System.Serializable]
/* A Serializable class is able to be 
 edited in the Inspector*/
public class Decorator
{
    //this class srotes information about each decorator or pip from DeckXML
    public string type; //for card pips, tye="pip"
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
