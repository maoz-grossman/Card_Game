using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//An enum defines a variable type with a few prenames values
public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}
//Make sure CardProspector extends Card
public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]
    //This is how you use the enum eCardState
    public eCardState state = eCardState.drawpile;
    //The hiddenBy list stores which other cards will keep this one face down
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    //The layoutID matches this card to the tableau card
    public int layoutID;
    //The SlotDef class stores information pulled in form the LayoutXML <slot>
    public SlotDef SlotDef;
}
