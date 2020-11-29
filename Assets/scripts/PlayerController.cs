using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PlayerController : Singleton<PlayerController> {
    //input variables
    float hPrev = 0f;
    float vPrev = 0f;
    //direction buffer
    int dirBuffer = 0;
    int prevLane = 0;
    //lane variable
    int currentLane = 0;
    [SerializeField]
    int numLanes = 3;
    public int NumLanes { get {return numLanes;}}
    public float laneWidth;
    [SerializeField]
    float strafeSpeed = 5f;
    float h = 0f; 

    Coroutine CurrentLaneChange;
    [SerializeField]
    float g = -9.81f;
    [SerializeField]
    float Vi = 5f;

    //animation
    Animator anim;
    int jumpParam;
    int slideParam;

    
    
    //states
    private Enums.State state = Enums.State.Run;

    //keep track of the lane change stack size 
    int laneChangeStack = 0;


    void Awake () {
        // middle lanes is always at the origin 
        transform.position = Vector3.zero;
        //StartCoroutine(TestRoutine());
        laneWidth = 7.5f / numLanes;

        anim = GetComponent<Animator>();
        jumpParam = Animator.StringToHash("Jump");
        slideParam = Animator.StringToHash("Slide");
    }


	void Update () {
        GetInput();
	}
   
    void MovePlayer(int direction)
    {
        
        if (CurrentLaneChange !=null)
        {
            if (currentLane + direction != prevLane)
            {
                dirBuffer = direction;
                return;
            }
            //overwrite previous moment 
            StopCoroutine(CurrentLaneChange);
            dirBuffer = 0;
        }
        //currentLane += direction;
        //transform.position = new Vector3(currentLane, 0f, 0f);

        prevLane = currentLane;
        currentLane = Mathf.Clamp(currentLane + direction, numLanes / -2, numLanes / 2);
        CurrentLaneChange = StartCoroutine(LaneChange());
        
        
    }
    void GetInput()
    {
        float hNew = Input.GetAxisRaw(InputNames.horizontalAxis);
        float vNew = Input.GetAxisRaw(InputNames.verticalAxis);
        float hDelta = hNew - hPrev;
        float vDelta = vNew = vPrev;
        if (Mathf.Abs(hDelta) > 0f && Mathf.Abs(hNew) > 0f && state == Enums.State.Run)
        {
            MovePlayer((int)hNew);
        }
        

        if (Mathf.Abs(Input.GetAxis(InputNames.horizontalAxis)) > 0f )
        {
            //Debug.Log("Horizontal Axis:" + Input.GetAxis(InputNames.horizontalAxis));
        }

        int v = 0;
        if (Mathf.Abs(vDelta) > 0f)
        {
            v = (int)vNew;
        }
        //jumping
        if ((Input.GetButtonDown("Jump") || v == 1) && state == Enums.State.Run && state == Enums.State.Run)//InputNames.jumpButton))
        {
            //Debug.Log("Jump button pressed");
            state = Enums.State.Jump;
            StartCoroutine(Jump());
        }

        //sliding
        if ((Input.GetButtonDown(InputNames.slideButton) || v == -1) && state == Enums.State.Run)
        {
            //Debug.Log("Slide button pressed");
            state = Enums.State.Slide;
            anim.SetTrigger(slideParam);
        }
        hPrev = hNew;
        vPrev = vNew;

    }
    void FinishSlide()
    {
        state = Enums.State.Run;
    }
    void FinishJump()
    {
        state = Enums.State.Run; 
    }

 


    IEnumerator Jump() { 

        //animation
        anim.SetBool(jumpParam, true);
        // calculate total time of jump
        float tFinal = (Vi *2f) /-g;

        float tLand = tFinal - 0.125f;
        float t = Time.deltaTime;
        for (; t < tLand; t+=Time.deltaTime )
        {
            float y = (g * t * t) / 2 + (Vi * t);
            Helpers.SetPositionY(transform,y);
            yield return null;

        }

        // transition back to run
        anim.SetBool(jumpParam, false);
        
        for (; t < tFinal; t+=Time.deltaTime )
        {
            float y = g * (t * t) / 2f + Vi * t;
            Helpers.SetPositionY(transform, y);
            yield return null;
        }
        Helpers.SetPositionY(transform, 0f);

        //transition back to run
        anim.SetBool(jumpParam, false);
    }

    //strafe movement coroutine 
    IEnumerator LaneChange()
    {
        Vector3 From = transform.right * prevLane * laneWidth;
        Vector3 To = Vector3.right * currentLane * laneWidth;
        float t = (laneWidth - Vector3.Distance(transform.position.x * Vector3.right, To)) / laneWidth;
        for (; t < 1f
             ; t += strafeSpeed * Time.deltaTime / laneWidth)
        {
            transform.position = Vector3.Lerp(From + Vector3.up * transform.position.y,
                To + Vector3.up * transform.position.y, t);
            yield return null;
        }
        transform.position = To + Vector3.up * transform.position.y;

        CurrentLaneChange = null;
        if (dirBuffer != 0 && ++laneChangeStack < 2)
        {
            MovePlayer(dirBuffer);
            dirBuffer = 0;
        }

        laneChangeStack = 0;
    }
}
