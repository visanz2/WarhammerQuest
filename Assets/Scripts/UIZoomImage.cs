using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIZoomImage : MonoBehaviour, IScrollHandler
{
    public Vector3 initialScale;

    [SerializeField]
    private float zoomSpeed = 0.1f;
    [SerializeField]
    private float maxZoom = 10f;

    private float initialFingersDistance;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchSupported)
        {
            if (Input.touches.Length == 2)
            {
                Touch t1 = Input.touches[0];
                Touch t2 = Input.touches[1];

                if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
                {
                    initialFingersDistance = Vector2.Distance(t1.position, t2.position);
                    initialScale = transform.localScale;
                }
                else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
                {
                    var currentFingersDistance = Vector2.Distance(t1.position, t2.position);
                    var scaleFactor = currentFingersDistance / initialFingersDistance;
                    transform.localScale = initialScale * scaleFactor;
                }
            }


                // Pinch to zoom
            /*    if (Input.touchCount == 2)
            {

                // get current touch positions
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);
                // get touch position from the previous frame
                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Debug.Log("deltaDistance: " + deltaDistance.ToString());

                var delta = Vector3.one;

                // close gesture with fingers is positive
                if (deltaDistance > 0)
                {
                    delta = Vector3.one * (-1 * zoomSpeedFingers);
                    Debug.Log("deltaDistance: " + deltaDistance.ToString());
                }
                else if (deltaDistance < 0) // open gesture with fingers is negative
                {
                    delta = Vector3.one * (1 * zoomSpeedFingers);
                    Debug.Log("deltaDistance: " + deltaDistance.ToString());
                }
                //Zoom(deltaDistance, TouchZoomSpeed);

                
                var desiredScale = transform.localScale + delta;

                desiredScale = ClampDesiredScale(desiredScale);

                transform.localScale = desiredScale;

            }*/
        }
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        transform.localScale = desiredScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(initialScale, desiredScale);
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
        return desiredScale;
    }
}