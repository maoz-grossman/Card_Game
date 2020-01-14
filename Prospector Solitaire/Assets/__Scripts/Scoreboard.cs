using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The Scoreboard manages showing the scire to the player
public class Scoreboard : MonoBehaviour
{
    public static Scoreboard S;//The singleton for Scorebord

    [Header("Set in inspector")]
    public GameObject prefabFloatingScore;
    [Header("Set Dynammicllay")]
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private string _scoreString;

    private Transform canvasTrans;

    //The score property also sets the scoreString
    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            _scoreString = _score.ToString("NO");
        }
    }

    //The scoreString property also sets the Text.text
    public string scoreString
    {
        get
        {
            return _scoreString;
        }
        set
        {
            _scoreString = value;
            GetComponent<Text>().text = _scoreString;
        }
    }

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("ERROR:Scoreboard.Awake(): Sis already set!");
        }
        canvasTrans = transform.parent;
    }

    //When called by SendMessage, this adds the fs.score
    public void FScallback(FloatingScore fs)
    {
        score += fs.score;
    }

    //This will Insatiate a new FloatingScore GameObject and initialize it.
    //it also returns a pointer to the FloatingScore created so that the 
    // calling function can di more with it(like Set fontSize, and so on)
    public FloatingScore CreateFloatingScore(int amt, List<Vector2> pts)
    {
        GameObject go = Instantiate<GameObject>(prefabFloatingScore);
        go.transform.SetParent(canvasTrans);
        FloatingScore fs = go.GetComponent<FloatingScore>();
        fs.score = amt;
        fs.reportFinishTo = this.gameObject;//Set fs to call back to this
        fs.init(pts);
        return fs;
    }
}
