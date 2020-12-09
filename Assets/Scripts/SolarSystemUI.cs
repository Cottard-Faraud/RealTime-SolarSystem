using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolarSystemUI : MonoBehaviour
{
    public static SolarSystemUI Instance;
    private SolarSystem solarSystem;
    private CameraController cameraController;

    [Header("Planets UI")]
    public GameObject planetUI;
    public List<PlanetUI> listPlanetUI;

    [Header("Button")]
    public GameObject buttonGUI;
    public GameObject buttonOrbit;
    public GameObject buttonLight;

    [Header("Slider")]
    public GameObject sliderSpeed;

    [Header("Other")]
    public GameObject infoPanel;
    public GameObject dateText;

    [Header("Animation")]
    public Animator planetSelectedAnimator;
    private bool planetSelectedPanelShown = false;

    private bool showGUI = true;
    private bool showOrbit = true;
    private bool globalLight = true;
    private bool solarSystemFirstTime = true; //sert pour le réaffichage des orbites

    private Color32 normalColor = new Color32(33, 43, 190, 255);
    private Color32 disabledColor = new Color32(0, 10, 82, 255);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        solarSystem = SolarSystem.Instance;
        cameraController = CameraController.Instance;

        //Vue Galaxie
        PlanetUnselected();
        ButtonLight(true);
    }

    public void PlanetSelected(GameObject planet)
    {
        //On active le mask
        planetUI.GetComponent<Image>().enabled = true;

        ButtonOrbit(true);

        //On reset les planètes et on désactive les orbites
        foreach (PlanetUI planetUI in listPlanetUI)
        {
            planetUI.PlanetUnselected();
        }

        //On désactive la caméra du système
        cameraController.ChangeCamera(planet);

        //On va notifier la planete selectionnée
        planet.GetComponent<Planet>().planetUI.PlanetSelected();

        //On notifie la classe principale du changement de planète
        solarSystem.ChangeCurrentPlanet(planet);

        //Animation de l'interface
        PlayGUIAnimation(true);
    }

    //Bouton "return" pour revenir à la galaxie
    public void PlanetUnselected()
    {
        //On désactive le mask
        planetUI.GetComponent<Image>().enabled = false;

        cameraController.ChangeCamera(solarSystem.gameObject);

        //UseGlobalLight(true);

        foreach (PlanetUI planetUI in listPlanetUI)
        {
            planetUI.PlanetUnselected();
            planetUI.DisplayOrbit(true);
        }

        //On affiche les icônes des planètes
        ButtonGUI(true);

        //Animation de fermeture pour la GUI
        PlayGUIAnimation(false);

        //Permet de réafficher les orbites
        solarSystemFirstTime = true;
        ButtonOrbit();
    }

    //Affichage des icônes des planètes
    public void ButtonGUI(bool forceActivate = false)
    {
        //On active de force
        if (forceActivate)
            showGUI = true;
        //On alterne
        else
            showGUI = !showGUI;

        foreach (PlanetUI planetUI in listPlanetUI)
        {
            planetUI.DisplayGUI(showGUI);
        }

        if (showGUI)
            buttonGUI.GetComponent<Image>().color = normalColor;
        else
            buttonGUI.GetComponent<Image>().color = disabledColor;
    }

    //Gère l'affichage des orbites
    public void ButtonOrbit(bool forceDisable = false)
    {
        //réactive les orbites lorsque l'on retourne au système solaire
        if (solarSystemFirstTime)
        {
            showOrbit = true;
            solarSystemFirstTime = false;
        }
        else if  (!forceDisable)
            showOrbit = !showOrbit;
        else
            showOrbit = false;

        foreach (PlanetUI planetUI in listPlanetUI)
        {
            planetUI.DisplayOrbitEnable(showOrbit);
        }

        if (showOrbit)
            buttonOrbit.GetComponent<Image>().color = normalColor;
        else
            buttonOrbit.GetComponent<Image>().color = disabledColor;
    }

    //Bouton Local Light
    public void ButtonLight(bool forceDisable = false)
    {
        if (forceDisable)
            globalLight = true;
        else
            globalLight = !globalLight;

        solarSystem.UseSolarSystemLight(globalLight);

        if (globalLight)
            buttonLight.GetComponent<Image>().color = disabledColor;
        else
            buttonLight.GetComponent<Image>().color = normalColor;
    }

    public void ButtonReset()
    {
        solarSystem.ResetPositions();
        sliderSpeed.GetComponent<Slider>().value = 0.0f;
    }

    public void SliderSpeed()
    {
        float value = sliderSpeed.GetComponent<Slider>().value / 1.0f;
        solarSystem.ChangeSpeed(value);
    }

    public void PlayGUIAnimation(bool state)
    {
        //Vérifier la condition permet de ne pas déclencher l'animation dans le Start 
        //car dans le Start : !state et !planetSelectedPanelShown
        if (!state && planetSelectedPanelShown || state && !planetSelectedPanelShown)
        {
            planetSelectedPanelShown = state;
            planetSelectedAnimator.SetTrigger("ChangeState");
        }
    }

    //Affiche la date
    public void SetDate(string month, int day, int year)
    {
        dateText.GetComponent<TextMeshProUGUI>().text = month + " " + day.ToString() + " " + year.ToString();
    }
}
