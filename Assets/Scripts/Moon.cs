using System.Collections;
using UnityEngine;

public class Moon : MonoBehaviour
{
    //On a besoin de le référence de la planète Terre pour pouvoir la regarder
    public GameObject earthPlanet;
    public GameObject planet;
    public PlanetInfos planetInfos;
    public GameObject planetOrbit;
    public GameObject planetPosition;
    public float speed = 20.0f;

    private float speedMultiplier = 1.0f;
    private bool resetMovement = false;

    public float orbitingSpeed = 1.0f;
    private bool orbiting = false;

    private LineRenderer orbit;

    void Awake()
    {
        orbit = planetOrbit.GetComponent<LineRenderer>();
    }

    void Update()
    {
        //La lune montre toujours la même face à la Terre
        transform.LookAt(earthPlanet.transform.localPosition);
    }

    public void InitOrbiting(int year, int month, int day, int hour, int minute)
    {
        float lon = GetRotation(year, month, day, hour, minute);

        // -lon car sinon symétrie sur Z
        planetPosition.transform.eulerAngles = new Vector3(0, -lon, 0); 
        int nb = FindClosestPoint();
        //planetPosition.transform.eulerAngles = new Vector3(0, 0, 0);

        StartCoroutine(Movements(nb));
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
            distanceTemp = Vector3.Distance(planetPosition.transform.GetChild(0).transform.localPosition, orbit.transform.TransformPoint(orbit.GetPosition(i)) - earthPlanet.transform.position);
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
        resetMovement = false;

        StartCoroutine(Orbiting(nb));
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

        Vector3 startPosition = this.transform.localPosition;

        transform.localPosition = orbit.transform.TransformPoint(orbit.GetPosition(nb)) - earthPlanet.transform.position;

        while (!resetMovement)
        {
            Vector3 nextPosition = orbitTransform.TransformPoint(orbit.GetPosition(count)) - earthPlanet.transform.position;
            while (transform.localPosition != nextPosition && !resetMovement)
            {
                yield return new WaitUntil(() => speedMultiplier != 0 || resetMovement);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPosition, speedMultiplier * orbitingSpeed * planetInfos.orbitingSpeedMultiplier * Time.deltaTime);
                yield return null;
            }


            if (count < orbit.positionCount - 1)
                count++;
            else
                count = 0;

            yield return null;
        }
        orbiting = false;
    }

    public void UpdateSpeedMultiplier(float speed)
    {
        speedMultiplier = speed;
    }
}

