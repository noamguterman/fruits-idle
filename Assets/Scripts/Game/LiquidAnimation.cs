using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidAnimation : MonoBehaviour
{
    [SerializeField]
    CameraPathBezierAnimator animator;
    public GameObject chunkPref;

    private GameObject curChunk;
    private Vector3 startPos;
    void Awake()
    {
        startPos = transform.position;
        //animator.AnimationStarted += OnAnimationStarted;
        //animator.AnimationPaused += OnAnimationPaused;
        //animator.AnimationStopped += OnAnimationStopped;
        animator.AnimationFinished += OnAnimationFinished;
        //animator.AnimationLooped += OnAnimationLooped;
        //animator.AnimationPingPong += OnAnimationPingPonged;
        //animator.AnimationPointReached += OnPointReached;
        //animator.AnimationPointReachedWithNumber += OnPointReachedByNumber;


    }

    void Start()
    {
        //animator.Play();
    }

    public void PlayOnceAnim(float speed, Color col)
    {
        //if(animator.isPlaying == true)
        {
            OnAnimationFinished();
        }

        animator.Play();

        animator.pathTime = Mathf.Clamp(speed, 0.5f, 1.5f);

        transform.position = startPos;
        curChunk = Instantiate(chunkPref);
        curChunk.transform.SetParent(transform);
        curChunk.transform.localPosition = Vector3.zero;
        curChunk.transform.localScale = Vector3.one;
        curChunk.GetComponent<SpriteRenderer>().color = col;
        curChunk.GetComponent<TrailRenderer>().startColor = col;
        curChunk.SetActive(true);
    }

    public void ChangeTubeAnimSpeed(float speed)
    {
        animator.pathTime = speed;
    }
    //private void OnAnimationLooped()
    //{
    //    Debug.Log("---------The animation has looped back to the start");
    //}

    //private void OnAnimationStarted()
    //{
    //    Debug.Log("The animation has begun");
    //}

    //private void OnAnimationPaused()
    //{
    //    Debug.Log("The animation has been paused");
    //}

    //private void OnAnimationStopped()
    //{
    //    Debug.Log("The animation has been stopped");
    //}

    private void OnAnimationFinished()
    {
        if (curChunk != null)
        {
            animator.Stop();
            transform.position = animator.transform.GetChild(2).position;
            curChunk.transform.SetParent(null);
            Destroy(curChunk, 1);
        }
    }

    //private void OnAnimationPingPonged()
    //{
    //    Debug.Log("The animation has ping ponged into the other direction");
    //}

    //private void OnPointReached()
    //{
    //    Debug.Log("A point was reached");
    //}

    private void OnPointReachedByNumber(int pointNumber)
    {
        Debug.Log("The point " + pointNumber + " was reached");
    }
}
