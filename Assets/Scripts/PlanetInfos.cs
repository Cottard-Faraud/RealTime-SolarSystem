using UnityEngine;

[HelpURL("http://stjarnhimlen.se/comp/ppcomp.html")]
public class PlanetInfos : MonoBehaviour
{
    public string planetName = "";

    [Space]

    public float orbitingSpeedMultiplier = 0f;

    [Space]

    [Tooltip("deg. longitude of the ascending node")]
    public float N;
    public float N_date;
    [Tooltip("deg. inclination to the ecliptic (plane of the earth orbit)")]
    public float i;
    public float i_date;
    [Tooltip("deg. argument of perihelion")]
    public float w;
    public float w_date;
    [Tooltip("semi-major axis (mean distance to the sun)")]
    public float a;
    public float a_date;
    [Tooltip("eccentricity (0 = circle, 0-1 = ellipse, 1 = parabola")]
    public float e;
    public float e_date;
    [Tooltip("deg. mean anomaly (0 at perihelion, increases uniformly with time)")]
    public float M;
    public float M_date;

    public float GetN(float d)
    {
        return N + N_date * d;
    }

    public float GetI(float d)
    {
        return i + i_date * d;
    }

    public float GetW(float d)
    {
        return w + w_date * d;
    }

    public float GetA(float d)
    {
        return a + a_date * d;
    }

    public float GetE(float d)
    {
        return e + e_date * d;
    }

    public float GetM(float d)
    {
        return M + M_date * d;
    }
}
