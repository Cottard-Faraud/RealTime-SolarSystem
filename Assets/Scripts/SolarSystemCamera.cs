using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemCamera : MonoBehaviour
{ 
    [Header("Zoom")]
    [Tooltip("Zoom increment")]
    public float transitionValue = 50;
    [Tooltip("Zoom increment when shift is pressed")]
    public float transitionValueShift = 5;
    [Tooltip("Zoom increment speed")]
    public float zoomSpeed = 500;
    [Tooltip("Zoom min value")]
    public float minValue = 50;
    [Tooltip("Zoom max value")]
    public float maxValue = 1200;

    private bool runningCoroutine = false; // est ce que la coroutine de zoom est en train de tourner
    private bool stopCoroutine = false; // message a envoyer pour terminer la coroutine plus tôt que prévu

    [Space]

    [Header("Rotation")]
    [Tooltip("Drag speed")]
    public float dragSpeed = 1;
    [Tooltip("Max rotation along x axis")]
    public float maxRotation = -80;

    private Vector3 dragOrigin; //point d'origine du drag
    private Vector3 move; // angle de la rotation

    public GameObject cameraPosition;

    private Camera mainCamera;
    private bool isSelected = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isSelected)
        {
            CameraZoom();

            CameraRotation();
        }
    }

    void CameraZoom()
    {
        float valueForTransition = transitionValue;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            valueForTransition = transitionValueShift;
        }

        int zoomValue = 1;
        bool isZoom = false;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (cameraPosition.transform.localPosition.y > minValue)
            {
                zoomValue = -1;
                isZoom = true;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cameraPosition.transform.localPosition.y < maxValue)
            {
                zoomValue = 1;
                isZoom = true;
            }
        }

        valueForTransition = Mathf.RoundToInt(cameraPosition.transform.localPosition.y / (valueForTransition / 2))+1;

        if (isZoom)
            StartCoroutine(NewZoom(zoomValue, valueForTransition));
    }

    //zoomValue = -1 pour zoomer et 1 pour dezoomer
    //valueForTransition = increment
    //
    IEnumerator NewZoom(int zoomValue, float valueForTransition)
    {
        if (runningCoroutine)
        {
            stopCoroutine = true;
            while (runningCoroutine)
            {
                yield return null;
            }
        }
        StartCoroutine(Zoom(zoomValue, valueForTransition));
    }

    IEnumerator Zoom(int zoomValue, float valueForTransition)
    {
        runningCoroutine = true;
        float posY = cameraPosition.transform.localPosition.y + valueForTransition * zoomValue;
        if (posY < minValue)
            posY = minValue;
        if (posY > maxValue)
            posY = maxValue;

        Vector3 nextPosition = new Vector3(0, posY, 0);

        while (!stopCoroutine && cameraPosition.transform.localPosition != nextPosition)
        {
            cameraPosition.transform.localPosition = Vector3.MoveTowards(cameraPosition.transform.localPosition, nextPosition, zoomSpeed * Time.deltaTime);
            yield return null;
        }

        if (stopCoroutine)
        {
            cameraPosition.transform.localPosition = nextPosition;
            stopCoroutine = false;
        }

        runningCoroutine = false;
    }

    void CameraRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            move = transform.eulerAngles;
            return;
        }

        if (Input.GetMouseButton(1))
        { 
            Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            if (ClampEulerAngle(move.x + (-pos.y * dragSpeed)) >= -maxRotation && ClampEulerAngle(move.x + (-pos.y * dragSpeed)) <= 0)
            {
                move += new Vector3(-pos.y * dragSpeed, pos.x * dragSpeed, 0);
            }
            else
            {
                move += new Vector3(0, pos.x * dragSpeed, 0);
            }
            transform.eulerAngles = move;
        }
    }

    public static float ClampEulerAngle(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < -180)
        {
            result += 360f;
        }
        return result;
    }

    public void Select()
    {
        isSelected = true; ;
    }

    public void Unselect()
    {
        isSelected = false; ;
    }
}
