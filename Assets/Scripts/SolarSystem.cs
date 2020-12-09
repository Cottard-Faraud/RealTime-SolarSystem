using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public static SolarSystem Instance;

    private LightController lightController;
    private SolarSystemUI solarSystemUI;

    public GameObject solarSystemCenter;
    public GameObject cameraPosition;

    [Header("Planets")]
    private List<Planet> listPlanets;
    public Planet mercury;
    public Planet venus;
    public Planet earth;
    public Planet mars;
    public Planet jupiter;
    public Planet saturn;
    public Planet uranus;
    public Planet neptune;
    public Moon moon;

    private GameObject currentPlanet = null;

    private System.DateTime currentDate;

    private string[] month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lightController = LightController.Instance;
        solarSystemUI = SolarSystemUI.Instance;

        listPlanets = new List<Planet>();
        listPlanets.Add(mercury);
        listPlanets.Add(venus);
        listPlanets.Add(earth);
        listPlanets.Add(mars);
        listPlanets.Add(jupiter);
        listPlanets.Add(saturn);
        listPlanets.Add(uranus);
        listPlanets.Add(neptune);

        DisplayRealTimePositions();
    }

    public void DisplayRealTimePositions()
    {
        //On récupère la date
        currentDate = System.DateTime.UtcNow;

        int currentYear = currentDate.Year;
        int currentMonth = currentDate.Month;
        int currentDay = currentDate.Day;
        int currentHour = currentDate.Hour;
        int currentMinute = currentDate.Minute;
        int currentSecond = currentDate.Second;

        //On affiche la date sur l'interface
        solarSystemUI.SetDate(month[currentMonth - 1], currentDay, currentYear);
        //On met la bonne rotation sur la Terre (= bon moment de la journée)
        earth.InitEarthRotation(currentHour - 1, currentMinute, currentSecond);
        //On change la rotation en fonction de la saison
        earth.SetEarthSeasonsRotation(GetSeasonRotation());

        //On initialise la Lune
        moon.UpdateSpeedMultiplier(0.0f);
        moon.InitOrbiting(currentYear, currentMonth, currentDay, currentHour, currentMinute);

        //On initialise les planètes
        foreach (Planet planet in listPlanets)
        {
            planet.UpdateSpeedMultiplier(0.0f);
            planet.InitOrbiting(currentYear, currentMonth, currentDay, currentHour, currentMinute);
        }
    }

    public void ChangeSpeed(float speed)
    {
        foreach (Planet planet in listPlanets)
        {
            planet.UpdateSpeedMultiplier(speed);
        }
        moon.UpdateSpeedMultiplier(speed);
    }

    public void ResetPositions()
    {
        DisplayRealTimePositions();
    }

    public void ChangeCurrentPlanet(GameObject planet)
    {
        currentPlanet = planet;

        lightController.SetPlanetToFollow(currentPlanet);
    }

    public void UseSolarSystemLight(bool solarSystem)
    {
        lightController.UseSolarSystemLight(solarSystem);
    }

    public void Select()
    {
        solarSystemCenter.GetComponent<SolarSystemCamera>().Select();
    }

    public void Unselect()
    {
        solarSystemCenter.GetComponent<SolarSystemCamera>().Unselect();
    }

    //Fonction utilisée pour le debug (appel dans Planet.Orbiting())
    public void StopOrbitingValue()
    {
        foreach (Planet planet in listPlanets)
            planet.StopOrbitingValue();
    }

    //Quand la Terre a tourné de 1/365 degrès
    public void AddDay()
    {
        currentDate = currentDate.AddDays(1);
        solarSystemUI.SetDate(month[currentDate.Month - 1], currentDate.Day, currentDate.Year);

        earth.SetEarthSeasonsRotation(GetSeasonRotation());
    }


    //Retourne l'inclinaison de la texture de la Terre en fonction des saisons
    // 1  au solstice d'hiver 
    // -1 au solstice d'été
    // 0 aux équinoxes de printemps et d'automne
    public float GetSeasonRotation()
    {
        float value = 0.0f;

        System.DateTime summerSolstice = new System.DateTime(currentDate.Year, 6, 21);
        System.DateTime winterSolstice = new System.DateTime(currentDate.Year, 12, 21);

        int daysBefore = 0;
        int daysAfter = 0;

        int sign = 1;

        //Juin à décembre
        if (!IsAfterWinter(month[currentDate.Month - 1], currentDate.Day))
        {
            daysBefore = currentDate.Subtract(summerSolstice).Days;
            daysAfter = winterSolstice.Subtract(currentDate).Days;

            //si on est plus proche du solstice d'été
            if (daysBefore < daysAfter)
                sign = -1;
        }
        //Décembre à juin
        else
        {
            //si on est en décembre
            if (month[currentDate.Month - 1].Equals("December"))
            {
                daysBefore = currentDate.Subtract(winterSolstice).Days;
                daysAfter = summerSolstice.AddYears(1).Subtract(currentDate).Days + 1;
            }
            //si on est plus en décembre
            else
            {
                daysBefore = currentDate.Subtract(winterSolstice.AddYears(-1)).Days;
                daysAfter = summerSolstice.Subtract(currentDate).Days + 1;
            }
            //si on est plus proche du solstice d'été
            if (daysBefore > daysAfter)
                sign = -1;
        }
        value = Mathf.Abs((daysAfter - daysBefore) / 182.0f) * sign;

        return value;
    }

    //retoutourne vrai si on est entre les équinoxes d'hiver et d'été
    public bool IsAfterWinter(string month, int day)
    {
        bool isBetween = false;

        if (month.Equals("December") && day >= 21)
            isBetween = true;
        if (month.Equals("January") || month.Equals("February")
            || month.Equals("March") || month.Equals("April")
            || month.Equals("May"))
            isBetween = true;
        if (month.Equals("June") && day <= 20)
            isBetween = true;

        return isBetween;
    }
}
