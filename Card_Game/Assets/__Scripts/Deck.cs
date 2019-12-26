using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Set in Inspector")]
    //suits
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;

    //prefab
    public GameObject prefabCard;
    public GameObject prefabSprite;


    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;


    //InitDeck is called by prospector wwhen it is ready
    public void InitDeck(string deckXMLText)
    {

        //this creates an anchor for all the Card GameObjects in thr Hierarchy
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }
        //initialize the Dictionary of SuitSprites with necessary Sprites
        dictSuits = new Dictionary<string, Sprite>()
        {
            {"c",suitClub},
            {"D",suitDiamond },
            {"H",suitHeart },
            {"S", suitSpade }
        };

        ReadDeck(deckXMLText);//this will preexisting line from earlier
                              //MakeCards();

    }


    //ReadDeck parses the XML file passed to it into CardDefinition
    public void ReadDeck(string deckXMLText)
    {
        xmlr = new PT_XMLReader();//create a new PT_XMLReder 
        xmlr.Parse(deckXMLText);
        //this prints a test line to show you how xmlr can be used
        string s = "xml[0] decorator[0]";
        s += "type=" + xmlr.xml["xml"][0]["decorator"][0].att("type");
        s += " x=" + xmlr.xml["xml"][0]["decorator"][0].att("x");
        s += " y=" + xmlr.xml["xml"][0]["decorator"][0].att("y");
        s += " scale=" + xmlr.xml["xml"][0]["decorator"][0].att("scale");
        //print(s); //we done with the test


        //read decorator for all Cards
        decorators = new List<Decorator>();//init the list of decorators
        PT_XMLHashList xDecos = xmlr.xml["xml"][0]["decorator"];
        Decorator deco;
        for (int i = 0; i < xDecos.Count; i++)
        {
            deco = new Decorator();
            //copy the attributes of the <decorator> to the Decorator
            deco.type = xDecos[i].att("type");
            //bool deco.flip is true if the text io the flip attribute is "1"
            deco.flip = (xDecos[i].att("flip") == "1");
            //floats need to be parsed from the attribute string
            deco.scale = float.Parse(xDecos[i].att("scale"));
            //vector3 loc initializes to [0,0,0],so we need to modify it
            deco.loc.x = float.Parse(xDecos[i].att("x"));
            deco.loc.y = float.Parse(xDecos[i].att("y"));
            deco.loc.z = float.Parse(xDecos[i].att("z"));
            //add temporary deco to the list Decorators
            decorators.Add(deco);
        }

        //read pip location for each card number 
        cardDefs = new List<CardDefinition>();
        PT_XMLHashList xCardDefs = xmlr.xml["xml"][0]["card"];
        for (int i = 0; i < xCardDefs.Count; i++)
        {
            CardDefinition cDef = new CardDefinition();
            //prase the attibute values and add them to cDef
            cDef.rank = int.Parse(xCardDefs[i].att("rank"));
            //grab an PT_XMLHashList of all the <pip>s on this <card>
            PT_XMLHashList xPips = xCardDefs[i]["pip"];
            if (xPips != null)
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    //iterate through all the <pip>s
                    deco = new Decorator();
                    //<pip>s on the <card> are handle via the Decorator Class
                    deco.type = "pip";
                    deco.flip = (xPips[j].att("flip") == "1");
                    deco.loc.x = float.Parse(xPips[j].att("x"));
                    deco.loc.y = float.Parse(xPips[j].att("y"));
                    deco.loc.z = float.Parse(xPips[j].att("z"));
                    if (xPips[j].HasAtt("scale"))
                    {
                        deco.scale = float.Parse(xPips[j].att("scale"));
                    }
                    cDef.pips.Add(deco);
                }
            }
            //face cards (Jack,Queen & King) have a face attribute
            if (xCardDefs[i].HasAtt("face"))
            {
                cDef.face = xCardDefs[i].att("face");
            }
            cardDefs.Add(cDef);
        }
    }


    //get the proper CardDefinition based on Rank(1 to 14)
    public CardDefinition GetCardDefinitionByRank(int rnk)
    {
        //search through all of the CardDefinition
        foreach (CardDefinition cd in cardDefs)
        {
            //if the rank is correct, return this definition
            if (cd.rank == rnk)
            {
                return (cd);
            }
        }
        return null;
    }

    //make the card GameObject
    public void MakeCards()
    {
        //cardName will be the names of crds to build
        //each suit goes from 1 to 14 (e.g., C1 to C4 for Clubs)
        cardNames = new List<string>();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string s in letters)
        {
            for (int i = 0; i < 13; i++)
            {
                cardNames.Add(s + (i + 1));
            }
        }
        //make list to hold all  the cards
        cards = new List<Card>();

        //iterate through all of the card names that were just made
        for (int i = 0; i < cardNames.Count; i++)
        {
            //makethe cards and add it to the cards Deck
            cards.Add(MakeCard(i));
        }
    }



    private Card MakeCard(int cNum)
    {
        //create a new Card GameObject
        GameObject cgo = Instantiate(prefabCard) as GameObject;
        //set the transform.parent of the new card to the anchor
        cgo.transform.parent = deckAnchor;
        Card card = cgo.GetComponent<Card>();//get Card component
        //this line stacks the cards so that they're all in nice rows
        cgo.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);

        //assign basic values to the card
        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse(card.name.Substring(1));
        if (card.suit == "D" || card.suit == "H")
        {
            card.colS = "Red";
            card.color = Color.red;
        }

        //pull the CardDefinition for this card
        card.def = GetCardDefinitionByRank(card.rank);

        AddDecorators(card);

        return card;
    }
    //temporary veriables will be reused several times in helper methods
    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    private void AddDecorators(Card card)
    {
        //Add Decorators
        foreach(Decorator deco in decorators)
        {
            if (deco.type == "suit")
            {
                //instantiate a Sprite GameObject
                _tGO = Instantiate(prefabSprite) as GameObject;
                //get the spriteRenderer Component
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                //set the Srite to the proper suit
                _tSR.sprite = dictSuits[card.suit];
            }
            else
            {
                _tGO = Instantiate(prefabSprite) as GameObject;
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                //get the proper sprite to show this rank
                _tSp = rankSprites[card.rank];
                //assign this rank sprite to the SpriteRenderer
                _tSR.sprite = _tSp;
                //set the color of the rank to match the suit
                _tSR.color = card.color;
            }
            //make the deco Sprites render above the Card
            _tSR.sortingOrder = 1;
            //make the decorator Sprites render above the Card
            _tGO.transform.SetParent(card.transform);
            //set the localPosition based pn the location from DeckXML
            _tGO.transform.localPosition = deco.loc;
            //flip the Decorator if needed

        }
    }
}
