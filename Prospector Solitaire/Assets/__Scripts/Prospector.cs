using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//will be used later
using UnityEngine.UI; //will be used later

public class Prospector : MonoBehaviour
{
    static public Prospector S;
    [Header("Set in inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public float xoffset = 3;
    public float yoffset = -2.5f;
    public Vector3 layoutCenter;

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;
    void Awake()
    {
        S = this;//set up a singleton for prospector    
    }
    void Start()
    {
        deck = GetComponent<Deck>();// get the Deck
        deck.InitDeck(deckXML.text);// pass DeckXML to it
        Deck.Shuffle(ref deck.cards);//this shuffles the deck by reference
        //Card c;
        //for(int cNum=0; cNum<deck.cards.Count; cNum++)
        //{
        //    c = deck.cards[cNum];
        //    c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        //}
        layout = GetComponent<Layout>();//Get the Layout component
        layout.ReadLayout(layoutXML.text);//Pass LayouutXML to it
        drawPile = Convert_List_Card_To_List_CradProspector(deck.cards);
        LayoutGame();
    }

    List<CardProspector> Convert_List_Card_To_List_CradProspector(List<Card> lcd)
    {
        List<CardProspector> lcp = new List<CardProspector>();
        CardProspector tCP;
        foreach (Card tCD in lcd)
        {
            tCP = tCD as CardProspector;
            lcp.Add(tCP);
        }
        return lcp;
    }

    //The Draw function will pull a single card from the drawPile
    CardProspector Draw()
    {
        CardProspector cd = drawPile[0];
        drawPile.RemoveAt(0);
        return (cd);
    }

    //LayoutGame() positions the initial tableau of card, a.k.a the "mine"
    void LayoutGame()
    {
        //Create an empty object to serve as an anchor for the tableau
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("layoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }
        CardProspector cp;
        //Follow the layout
        foreach(SlotDef tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            //This replaces the previous parent; deck.deckAnchor which 
            //appears as _Deck in the Hierarchy when the the scene is playing
            cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.LayerID);
            //Set the localPosition of the card based on slotDef
            cp.layoutID = tSD.id;
            cp.SlotDef = tSD;
            //CrdProsperctor in the tableau have the state CardState.tableau
            cp.state = eCardState.tableau;
            cp.SetSortingLayerName(tSD.layerName);//Set the sorting layers
            tableau.Add(cp);//Add this CradProspector to the List<> tableau 
        }
    }
}
