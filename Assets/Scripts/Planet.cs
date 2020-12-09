using System.Collections;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject planet;
    public GameObject planetCenter;
    public GameObject planetOrbit;
    public PlanetInfos planetInfos;
    public GameObject planetPosition; //utilisé pour la position de la planète sur son orbite
    public PlanetUI planetUI;
    public GameObject cameraPosition;

    [Header("Movements")]

    private float speedMultiplier = 0.0f;
    private bool resetMovement = false;

    public float rotationSpeed;
    private bool rotation = false;
    private System.DateTime newRotationDate;

    public float orbitingSpeed;
    private bool orbiting = false;

    [Header("Other")]

    private LineRenderer orbit;

    private Material earthMat;

    //Test coroutines move
    private Vector3 newPosition = Vector3.zero;

    //Calcule t affichage des positions des planètes quand la Terre a fini un tour complet
    //string resultat = "";
    //bool firstTimeAt1 = false;

    void Awake()
    {
        orbit = planetOrbit.GetComponent<LineRenderer>();

        if (planetUI == null)
            return;

        planetUI.SetLookAt(planet);
        planetUI.SetPlanetOrbit(planetOrbit);
    }

    public void InitOrbiting(int year, int month, int day, int hour, int minute)
    {
        float lon = GetRotation(year, month, day, hour, minute);

        // -lon car sinon symétrie sur Z
        planetPosition.transform.eulerAngles = new Vector3(0, -lon, 0);
        int nb = FindClosestPoint();
        planetPosition.transform.eulerAngles = new Vector3(0, 0, 0);

        StartCoroutine(Movements(nb));
    }

    public void InitEarthRotation(int hour, int minute, int second)
    {
        float r;

        earthMat = planet.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;

        earthMat.SetVector("Vector2_3D4D28F7", new Vector2(0, 0));
        r = (6.283185307f * (hour * 3600 + minute * 60 + second) / 43200) * 180 / Mathf.PI;
        while (r > 360)
            r -= 360;

        r /= 360;

        int i = 1;
        if (hour >= 12)
            i = 0;

        earthMat.SetVector("Vector2_3D4D28F7", new Vector2(-r / 2.0f - i * .5f, 0));

        newRotationDate = new System.DateTime(2000, 1, 1, hour, minute, second);
    }

    public float GetRotation(int year, int month, int day, int hour, int minute)
    {
        float E0, E1, x, y, dist, v, xeclip, yeclip, zeclip, lon, lat, r, d;
        d = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530;
        d += hour / 24.0f + minute / 60.0f;

        float N = planetInfos.GetN(d);
        float i = planetInfos.GetI(d);
        float w = planetInfos.GetW(d);
        float a = planetInfos.GetA(d);
        float e = planetInfos.GetE(d);
        float M = planetInfos.GetM(d);

        E0 = M + (180 / Mathf.PI) * e * Mathf.Sin(M * Mathf.PI / 180) * (1 + e * Mathf.Cos(M * Mathf.PI / 180));
        E1 = E0 - (E0 - (180 / Mathf.PI) * e * Mathf.Sin(E0 * Mathf.PI / 180) - M) / (1 - e * Mathf.Cos(E0 * Mathf.PI / 180));

        //find the most accurate value for E
        while (Mathf.Abs(E0 - E1) > 0.005)
        {
            E0 = E1;
            E1 = E0 - (E0 - (180 / Mathf.PI) * e * Mathf.Sin(E0 * Mathf.PI / 180) - M) / (1 - e * Mathf.Cos(E0 * Mathf.PI / 180));
        }

        //compute rectangular coordinates (x, y) in the plane of the planet orbit
        x = a * (Mathf.Cos(E1 * Mathf.PI / 180) - e);
        y = a * Mathf.Sqrt(1 - e * e) * Mathf.Sin(E1 * Mathf.PI / 180);

        //convert the results to distance (radius vector) & true anomaly
        dist = Mathf.Sqrt(x * x + y * y);
        v = Mathf.Atan2(y, x) * 180 / Mathf.PI; //in degree

        //compute the planet's position in ecliptic coordinates
        xeclip = dist * (Mathf.Cos(N * Mathf.PI / 180) * Mathf.Cos((v + w) * Mathf.PI / 180) - Mathf.Sin(N * Mathf.PI / 180) * Mathf.Sin((v + w) * Mathf.PI / 180) * Mathf.Cos(i * Mathf.PI / 180));
        yeclip = dist * (Mathf.Sin(N * Mathf.PI / 180) * Mathf.Cos((v + w) * Mathf.PI / 180) + Mathf.Cos(N * Mathf.PI / 180) * Mathf.Sin((v + w) * Mathf.PI / 180) * Mathf.Cos(i * Mathf.PI / 180));
        zeclip = dist * Mathf.Sin((v + w) * Mathf.PI / 180) * Mathf.Sin(i * Mathf.PI / 180);

        //convert to ecliptic longitude, latitude and distance
        lon = Mathf.Atan2(yeclip, xeclip) * 180 / Mathf.PI; //degree
        lat = Mathf.Atan2(zeclip, Mathf.Sqrt(xeclip * xeclip + yeclip * yeclip)) * 180 / Mathf.PI; //degree
        r = Mathf.Sqrt(xeclip * xeclip + yeclip * yeclip + zeclip * zeclip);

        if (lon < 0)
            lon += 360;

        //Debug.Log(this.name + " lon = " + lon);

        return lon;
    }

    public int FindClosestPoint()
    {
        float distance = float.MaxValue;
        float distanceTemp;
        int nb = 0;
        for (int i = 0; i < orbit.positionCount - 1; i++)
        {
            distanceTemp = Vector3.Distance(planetPosition.transform.GetChild(0).transform.position, orbit.transform.TransformPoint(orbit.GetPosition(i)));
            if (distanceTemp < distance)
            {
                distance = distanceTemp;
                nb = i;
            }
        }
        return nb;
    }

    IEnumerator Movements(int nb)
    {
        resetMovement = true;
        yield return new WaitUntil(() => !orbiting);
        yield return new WaitUntil(() => !rotation);
        resetMovement = false;

        InitEarthRotation(newRotationDate.Hour, newRotationDate.Minute, newRotationDate.Second);

        StartCoroutine(Rotation());
        StartCoroutine(Orbiting(nb));
    }

    IEnumerator Rotation()
    {
        rotation = true;
        if (planetInfos.planetName.Equals("Earth"))
        {
            System.DateTime rotationDate = newRotationDate;
            while (!resetMovement)
            {
                yield return new WaitUntil(() => speedMultiplier != 0 || resetMovement);
                if (speedMultiplier != 0)
                {
                    yield return new WaitForSeconds(0.01f);
                }

                rotationDate = rotationDate.AddMinutes(speedMultiplier);

                float r;

                earthMat = planet.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;

                earthMat.SetVector("Vector2_3D4D28F7", new Vector2(0, 0));
                r = (6.283185307f * (rotationDate.Hour * 3600 + rotationDate.Minute * 60 + rotationDate.Second) / 43200) * 180 / Mathf.PI;
                while (r > 360)
                    r -= 360;

                r /= 360;

                int i = 1;
                if (rotationDate.Hour >= 12)
                    i = 0;

                earthMat.SetVector("Vector2_3D4D28F7", new Vector2(-r / 2.0f - i * .5f, 0));
                yield return null;
            }
        }
        else
        {
            planet.transform.eulerAngles = new Vector3(0, 0, 0);
            while (!resetMovement)
            {
                planet.transform.Rotate(0, rotationSpeed * speedMultiplier * Time.deltaTime, 0);
                yield return null;
            }
        }
        rotation = false;
    }

    IEnumerator Orbiting(int nb)
    {
        orbiting = true;
        int count;

        if (nb < orbit.positionCount - 1)
            count = nb + 1;
        else
            count = 0;

        Transform orbitTransform = orbit.transform;

        //varibles utilisées pour le debug
        //Vector3 startPosition = this.transform.localPosition;
        //int cpt = 0;
        //float angleTemp = 0;
        //float maxAngle = 0;
        //bool decrease = false;

        transform.localPosition = orbit.transform.TransformPoint(orbit.GetPosition(nb));

        while (!resetMovement)
        {
            Vector3 nextPosition = orbitTransform.TransformPoint(orbit.GetPosition(count));
            while (transform.localPosition != nextPosition && !resetMovement)
            {
                yield return new WaitUntil(() => speedMultiplier != 0 || resetMovement);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPosition, speedMultiplier * orbitingSpeed * planetInfos.orbitingSpeedMultiplier * Time.deltaTime);
                yield return null;
            }

            //!resetPosition pour éviter que l'on ajoute un jour lorsque l'on reset
            if (planetInfos.planetName.Equals("Earth") && !resetMovement)
                SolarSystem.Instance.AddDay();

            //////////////////////
            //Début vérification//
            //////////////////////
            //float distStart = Mathf.Sqrt(Mathf.Pow(startPosition.x, 2) + Mathf.Pow(startPosition.y, 2) + Mathf.Pow(startPosition.z, 2));
            //float distEnd = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x, 2) + Mathf.Pow(transform.localPosition.y, 2) + Mathf.Pow(transform.localPosition.z, 2));
            //float distStartEnd = Mathf.Sqrt(
            //      Mathf.Pow(transform.localPosition.x - startPosition.x, 2)
            //    + Mathf.Pow(transform.localPosition.y - startPosition.y, 2)
            //    + Mathf.Pow(transform.localPosition.z - startPosition.z, 2));

            //float angle = Mathf.Acos((-Mathf.Pow(distStartEnd, 2) + Mathf.Pow(distStart, 2) + Mathf.Pow(distEnd, 2)) / (2 * distStart * distEnd)) * (180.0f / Mathf.PI);

            //if (angle > angleTemp)
            //{
            //    if (decrease)
            //    {
            //        decrease = false;
            //        cpt += 1;
            //    }
            //    maxAngle = angle;
            //}
            //else
            //{

            //    decrease = true;
            //    maxAngle = 360 - angle;
            //}

            //angleTemp = angle;
            //resultat = (cpt + (maxAngle / 360.0f).ToString().Substring(1)).ToString();

            //if (planetInfos.planetName.Equals("Earth") && resultat.StartsWith("1") && !firstTimeAt1)
            //{
            //    SolarSystem.Instance.StopOrbitingValue();
            //    firstTimeAt1 = true;
            //}
            ////////////////////
            //Fin vérification//
            ////////////////////

            if (count < orbit.positionCount - 1)
                count++;
            else
                count = 0;
        }
        orbiting = false;
    }

    public void SetEarthSeasonsRotation(float angle)
    {
        earthMat.SetFloat("Vector1_9AE4D463", angle);
    }

    //Fonction utilisée pour le debug
    public void StopOrbitingValue()
    {
        //Debug.Log(planetInfos.planetName + " : " + resultat);
    }

    //Pour l'animation de la caméra
    public void Select()
    {
        planetCenter.GetComponent<PlanetCamera>().Select();
    }

    //Pour l'animation de la caméra
    public void Unselect()
    {
        planetCenter.GetComponent<PlanetCamera>().Unselect();
    }

    public void UpdateSpeedMultiplier(float speed)
    {
        speedMultiplier = speed;
    }
}
