using UnityEngine;
using UnityEngine.UI;

public class PlanetUI : MonoBehaviour
{
    private GameObject planetOrbit;

    private Transform lookAt;
    private Camera cam;

    private bool planetSelected = false;
    private bool onScreen = false;

    private bool displayGUIEnable = true;
    private bool displayOrbitEnable = true;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!planetSelected && displayGUIEnable)
        {
            Vector3 pos = cam.WorldToScreenPoint(lookAt.position);
            if (pos.z >= 0) //pour ne pas voir les sprites des planètes derrière nous
            {
                if (!onScreen)
                {
                    onScreen = true;
                    this.GetComponent<Image>().enabled = true;
                }
                pos = new Vector3(pos.x, pos.y, 0);
                if (transform.position != pos)
                    transform.position = pos;
            }
            else if (onScreen)
            {
                onScreen = false;
                this.GetComponent<Image>().enabled = false;
            }
        }
    }

    public void SetPlanetOrbit(GameObject po)
    {
        planetOrbit = po;
    }

    public void SetLookAt(GameObject g)
    {
        lookAt = g.transform;
    }

    public void PlanetSelected()
    {
        planetSelected = true;
        this.GetComponent<Image>().enabled = false;

        DisplayOrbit(false);
    }

    public void PlanetUnselected()
    {
        planetSelected = false;
        this.GetComponent<Image>().enabled = true;

        if (displayOrbitEnable)
            DisplayOrbit(true);
    }

    public void DisplayOrbit(bool display)
    {
        if (!displayOrbitEnable && display)
            return;
        if (display && planetSelected)
            return;

        if (planetOrbit != null)
            planetOrbit.SetActive(display);
    }

    public void DisplayOrbitEnable(bool enable)
    {
        displayOrbitEnable = enable;
        DisplayOrbit(enable);
    }

    public void DisplayGUI(bool display)
    {
        displayGUIEnable = display;
        if (planetSelected)
            return;
        if (displayGUIEnable && !onScreen)
            return;

        this.GetComponent<Image>().enabled = displayGUIEnable;
    }

    public void ButtonPlanetSelected(GameObject planet)
    {
        SolarSystemUI.Instance.PlanetSelected(planet);
    }
}