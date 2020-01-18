using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// an enum ti handle all to possible scoring event
public enum eScoreEvent
{
  draw,
  mine,
  mineGold,
  gameWin,
  gameLoss
}


//Score Manager handles all of the scoring
public class ScoreManager : MonoBehaviour

{
    static private ScoreManager S;//singleton 
    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]
    //Fields to track score info
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;
    

    public void Awake()
    {
        
        if (S == null)
        {
            
            S = this;//Set the private singleton
        }
        else
        {
            Debug.LogError("ERROR: ScoreManager.Awake() : S is already set!");
        }

        //Check for a high score in PlayPrefs
        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
            /**
             *PlayerPrefs is a simple way to store and retrieve values for use within your project.
             * It is well known that as it isn’t encrypted you shouldn’t store anything sensitive in there. 
             * However, it can be very useful for storing other values,
             * such as user preferences or configuration.
             * */
        }

        //Add the score from the last round, which will be >0 if it was a win
        score += SCORE_FROM_PREV_ROUND;
        //And reset the score form prev round
        SCORE_FROM_PREV_ROUND = 0;
    }

    static public void EVENT(eScoreEvent evt)
    {
        try
        {
            S.Event(evt);
        }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager: EVENT() called while S=null\n" + nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            //Thing that should happen wheter it's a draw, a win, or a loss
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                chain = 0;// reset the score chain
                score += scoreRun;//add scoreRun to total score
                scoreRun = 0;//reset scoreRun
                break;

            case eScoreEvent.mine://Remove a mine card
                chain++;//increase the score chain 
                scoreRun += chain;//add score for this card to scoreRun
                break;
        }

        //This second switch statement handles round wins and losses
        switch (evt)
        {
            case eScoreEvent.gameWin:
                //if this is a win, add the score to the next round
                //static fields are not by SceneManager.LoadScene()
                SCORE_FROM_PREV_ROUND = score;
                print("You won this round! Round score: " + score);
                break;

            case eScoreEvent.gameLoss:
                //If it a loss' check against the high score
                if (HIGH_SCORE <= score)
                {
                    print("You got the high score! High score: " + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print("Your final score for the game was: " + score);
                }
                break;

            default:
                print("score: " + score + "  ,scoreRun: " + scoreRun + "  ,chain: " + chain);
                break;
        }
    }

    static public int CHAIN { get { return S.chain; } }
    static public int SCORE { get { return S.score; } } 
    static public int SCORE_RUN { get { return S.scoreRun; } }

}
