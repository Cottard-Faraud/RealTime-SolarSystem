using UnityEngine;

public class PlanetCamera : MonoBehaviour
{ 
    private Camera mainCamera;
    
    private float maxRotation = 90;
    private float dragSpeed = 2f;

    private bool isSelected = false;

    private Vector3 dragOrigin;
    private Vector3 move; // angle de la rotation

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Select()
    {
        isSelected = true;
    }

    public void Unselect()
    {
        isSelected = false;
    }

    void Update()
    {
        if (!isSelected)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            move = transform.eulerAngles;
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            if (ClampEulerAngle(move.x + (pos.y * dragSpeed)) >= -maxRotation && ClampEulerAngle(move.x + (pos.y * dragSpeed)) <= maxRotation)
            {
                move += new Vector3(pos.y * dragSpeed, pos.x * dragSpeed, 0);
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
}
