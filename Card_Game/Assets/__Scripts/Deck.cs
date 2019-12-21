using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
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
        ReadDeck(deckXMLText);
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
        for(int i=0;i<xCardDefs.Count;i++)
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

}
