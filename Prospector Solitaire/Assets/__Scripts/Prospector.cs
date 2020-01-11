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
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }
        CardProspector cp;
        //Follow the layout
        foreach (SlotDef tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            //This replaces the previous parent; deck.deckAnchor which 
            //appears as _Deck in the Hierarchy when the the scene is playing
            cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.LayerID);
            //Set the localPosition of the card based on slotDef
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            //CrdProsperctor in the tableau have the state CardState.tableau
            cp.state = eCardState.tableau;
            cp.SetSortingLayerName(tSD.layerName);//Set the sorting layers
            tableau.Add(cp);//Add this CradProspector to the List<> tableau 
        }

        //Set which cards are hiding others
        foreach (CardProspector tCP in tableau)
        {
            foreach (int hid in tCP.slotDef.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }
        //Set up initial target card
        MoveToTarget(Draw());

        //Set up the Draw pile
        UpdateDrawPile();
    }


    //Convert from layoutID int to the CardProspector with that ID
    CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach (CardProspector tCP in tableau)
        {
            //Search throught all cards in the tableau List<>
            if (tCP.layoutID == layoutID)
            {
                return tCP;
            }
        }
        //If it's not found, return null
        return null;
    }

    //this turns cards in the mind face-up or down
    void SetTableauFaces()
    {
        foreach(CardProspector cd in tableau)
        {
            bool faceUp = true;//Assume this card will be face-up
            foreach (CardProspector cover in cd.hiddenBy)
            {
                //If either of the converting cards are in the tableau
                if (cover.state == eCardState.tableau)
                {
                    faceUp = false;//then this card is face down
                }
            }
            cd.faceUp = faceUp;//Set the value on this card
        }
    }

    //Move the current target to the dicard pile 
    void MoveToDiscard(CardProspector cd)
    {
        //Change the state of the card to discard
        cd.state = eCardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;

        //Position this card on the discardPile
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.LayerID + 0.5f);
        cd.faceUp = true;
        //Place it on top of the pile for deph sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    //Make cd the new target card
    void MoveToTarget(CardProspector cd)
    {
        //If there is currently a target card, move it to discard pile
        if (target != null) MoveToDiscard(target);
        target = cd;
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;
        //Move to the target position 
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.LayerID);

        cd.faceUp = true;//Make it faceUP
        //Set the depth sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }

    //Arrange all the cards of the draw pile to show how many are left
    void UpdateDrawPile()
    {
        CardProspector cd;
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            //Position it correctly with the layout.drawPile.stagger
            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x)
                , layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y)
                , -layout.drawPile.LayerID + 0.1f * i);

            cd.faceUp = false;//Make theme all face-down
            cd.state = eCardState.drawpile;
            //Set depth sorting
            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }
    }

    //CardClicked is called any time a card in the game is clicked
    public void CardClicked(CardProspector cd)
    {
        //The reaction is determind the by state of the thhe clicked card
        switch (cd.state)
        {
            case eCardState.target:
                //Does nothing
                break;
            case eCardState.drawpile:
                //Clicking any card in the drawPile will draw the next pile
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                break;
            case eCardState.tableau:
                bool validMatch = true;
                if (!cd.faceUp)
                {
                    //If the card is face down
                    validMatch = false;
                }
                if (!AdjacentRank(cd, target))
                {
                    //If it is not an adjacent rank, it is not valid
                    validMatch = false;
                }
                if (!validMatch)
                    return;

                //If we got here, then:Yay! it's a valiid card
                tableau.Remove(cd);//Remive it from the tableau List
                MoveToTarget(cd);//Make it target card
                SetTableauFaces();//Update tableau card face-ups
                break;
        }
    }




    //Return true if the two cards are adjacent in rank 
    public bool AdjacentRank(CardProspector c0,CardProspector c1)
    {
        if (Mathf.Abs((c0.rank % 13) - (c1.rank % 13)) == 1)
        {
            return true;
        }
        //For the king and the queen
        if ((c0.rank == 13 && c1.rank == 12) || (c0.rank == 12 && c1.rank == 13))
            return true;
        else return false;
    }

}
