using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController Instance;

    Transform lightTransform;

    public GameObject cameraLight;

    private Transform planetToFollow;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lightTransform = this.transform;
    }

    void Update()
    {
        if (planetToFollow == null)
            return;

        //Permet a la lumière du soleil de toujours pointer vers la planète sur laquelle on se trouve
        lightTransform.LookAt(planetToFollow);
    }

    //Permet a la lumière du soleil de toujours pointer vers la planète sur laquelle on se trouve
    public void SetPlanetToFollow(GameObject planet)
    {
        planetToFollow = planet.transform;
    }

    public void UseSolarSystemLight(bool solarSystem)
    {
        GetComponent<Light>().enabled = solarSystem;
        cameraLight.GetComponent<Light>().enabled = !solarSystem;
    }
}
