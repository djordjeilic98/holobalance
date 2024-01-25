using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomHighlighter : MonoBehaviour
{

    public float scaleFactor = 1.2f;
    public float offset = 0.02f;
    public float transitionSpeed = 8;

    private Vector3 startScale;
    private Vector3 startPosition;

    private Vector3 targetScale;
    private Vector3 targetPosition;

    private float transitionFactor;
    public AudioSource zoomIn;

    private bool IsInitialized = false;

    public enum STATE
    {
        NONE,
        IN,
        OUT,
        ZOOMED,
    }

    private STATE state;

    public STATE GetState()
    {
        return state;
    }

    public void Start()
    {
        startPosition = gameObject.transform.localPosition;
        targetPosition = gameObject.transform.localPosition + new Vector3(0, 0, offset);

        startScale = gameObject.transform.localScale;
        targetScale = gameObject.transform.localScale * scaleFactor;
    }


    public void Highlight()
    {
        if (zoomIn)
        {
            //AudioManager.instance.PlaySound(zoomIn);
        }
        if (state == STATE.NONE)
        {
            transitionFactor = 0;
        }
        state = STATE.IN;

        if (!IsInitialized)
        {
            startPosition = gameObject.transform.position;
            targetPosition = gameObject.transform.position + new Vector3(0, 0, offset);

            startScale = gameObject.transform.localScale;
            targetScale = gameObject.transform.localScale * scaleFactor;

            IsInitialized = true;
        }
    }

    public void Unhighlight()
    {
        state = STATE.OUT;
    }

    public void Update()
    {
        if (state == STATE.IN || state == STATE.OUT)
        {
            if (state == STATE.IN)
            {
                transitionFactor += Time.deltaTime * transitionSpeed;
            }
            else if (state == STATE.OUT)
            {
                transitionFactor -= Time.deltaTime * transitionSpeed;
            }

            if (transitionFactor > 1)
            {
                transitionFactor = 1;
                state = STATE.ZOOMED;
            }

            if (transitionFactor < 0)
            {
                transitionFactor = 0;
                state = STATE.NONE;
            }

            gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, transitionFactor);
            gameObject.transform.localScale = Vector3.Lerp(startScale, targetScale, transitionFactor);
        }
    }

}