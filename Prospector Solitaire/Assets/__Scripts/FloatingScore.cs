using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eFSSstate
{
    idle,
    pre,
    active,
    post
}

public class FloatingScore : MonoBehaviour
{
    [Header("Set Dynamically")]
    public eFSSstate state = eFSSstate.idle;

    [SerializeField]
    protected int _score = 0;
    public string scoreString;

    //The score property sets both _score and scoreString
    public int score
    {
        get
        {
            return (_score);
        }
        set
        {
            _score = value;
            scoreString = _score.ToString("NO");//"NO" adds commas to the num
            GetComponent<Text>().text = scoreString;
        }
    }

    public List<Vector2> bezierPts;//bezier points for movment
    public List<float> fontSizes;//Bezier points for font scaling
    public float timeStart = -1f;
    public float timeDuration = 1f;
    public string easingCurve = Easing.InOut;//Use Easing in Utils.cs

    //The gameObject that will receive the SendMessage when this is done moving
    public GameObject reportFinishTo = null;
    private RectTransform rectTrans;
    private Text txt;

    //Set up the FloatingScore and movment
    //Note the use of parameter defaults for eTimesS & eTimeD
    public void init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;
        txt = GetComponent<Text>();
        bezierPts = new List<Vector2>(ePts);

        if (ePts.Count == 1)
        {
            //..Then just go there
            transform.position = ePts[0];
            return;
        }
        //If eTimeS is the default, just start at the current time
        if (eTimeS == 0)
        {
            eTimeS = Time.time;
        }
        timeStart = eTimeS;
        timeDuration = eTimeD;

        //Set it ti the pre state' ready to start time
        state = eFSSstate.pre;
    }

    public void FSCallback(FloatingScore fs)
    {
        //When this callback is called by SendMessage,
        //add the score from the calling FloatungScore
        score += fs.score;
    }

    //Update is called once per frame
    void Update()
    {
         //If this is not moving, just return 
        if (state == eFSSstate.idle) return;

        //Get u from the current tim duration 
        //u range from 0 to 1
        float u = (Time.time - timeStart) / timeDuration;
        // Use Easing class from Utils to curv the u value.
        float uC = Easing.Ease(u, easingCurve);
        if (u < 0)
        {
            //If u<0, then we shouldn't move yet
            state = eFSSstate.pre;
            txt.enabled = false;//Hide the initially
        }
        else;
        {
            if (u >= 1)
            {
                //If u>=1, we're done moving
                uC = 1;//Set uC=1 so we don't overshoot
                state = eFSSstate.post;
                if (reportFinishTo != null)
                {
                    //If there's a callback GameObjact 
                    //Use SendMessageto call the FScallback method
                    //with this parameter.
                    reportFinishTo.SendMessage("FSCallback", this);
                    //Now that the message has been sent
                    // Destroy this GameObject
                    Destroy(gameObject);
                }
                else
                {
                    //If there is nothing to callback
                    //.. then dont destroy this, just let it stay still
                    state = eFSSstate.idle;
                }
            }
            else
            {
                //0<=u<1,which means that ths is active and moving 
                state = eFSSstate.active;
                txt.enabled = true;//Show the score once more
            }
                //Use bezier curve to move it to the right point
                Vector2 pos = Utils.Bezier(uC, bezierPts);
                //RecTransform anchors can be used to position UI object relative
                //to total size of the screen
                rectTrans.anchorMin = rectTrans.anchorMax = pos;
                if(fontSizes!= null&& fontSizes.Count>0)
            {
                //If fotSizes hes  values in it 
                //...then adjust the fontSize if this GUIText 
                int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
                GetComponent<Text>().fontSize = size;
            }
            
        }
    }
}
