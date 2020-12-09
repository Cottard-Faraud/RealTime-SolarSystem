using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private PlayableDirector playableDirector;
    private TimelineAsset timelineAsset;

    [Header("Cameras")]
    public GameObject camera1;
    public GameObject camera2;

    private GameObject currentAstralBody;
    private Transform currentPosition;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        timelineAsset = (TimelineAsset)playableDirector.playableAsset;
    }

    public void ChangeCamera(GameObject astralBody)
    {
        //Initialisation de la caméra sur le système solaire
        if (currentPosition == null)
        {
            currentAstralBody = SolarSystem.Instance.gameObject;
            currentPosition = SolarSystem.Instance.cameraPosition.transform;
            currentAstralBody.GetComponent<SolarSystem>().Select();
            return;
        }

        Transform t = null;

        //Solar System
        if (astralBody.GetComponent<SolarSystem>())
            t = astralBody.GetComponent<SolarSystem>().cameraPosition.transform;

        //Planet
        else if (astralBody.GetComponent<Planet>())
            t = astralBody.GetComponent<Planet>().cameraPosition.transform;

        if (t != null)
            StartCoroutine(ChangeCameraCoroutine(astralBody, t));
    }

    IEnumerator ChangeCameraCoroutine(GameObject astralBody, Transform t)
    {
        //On place la première caméra sur la position actuelle
        camera1.transform.position = currentPosition.position;
        camera1.transform.rotation = currentPosition.rotation;

        //On place la dexième caméra sur la position d'arrivée
        camera2.transform.position = t.position;
        camera2.transform.rotation = t.rotation;
        //On parente la caméra (pour qu'elle bouge avec la planète le temps que l'animation se fasse)
        camera2.transform.SetParent(t.transform);

        //On deselectionne afin que les contrôles à la souris ne fonctionnent plus le temps de l'animation
        if (currentAstralBody.gameObject.GetComponent<Planet>())
            currentAstralBody.gameObject.GetComponent<Planet>().Unselect();
        else
            currentAstralBody.gameObject.GetComponent<SolarSystem>().Unselect();
        
        //On joue l'animation
        playableDirector.Play();
        
        //On attend la fin de l'animation
        yield return new WaitForSeconds((float)playableDirector.duration);

        currentAstralBody = astralBody;
        currentPosition = t;

        //On selectionne pour que les contrôles à la souris fonctionnent de nouveau
        if (currentAstralBody.gameObject.GetComponent<Planet>())
            currentAstralBody.gameObject.GetComponent<Planet>().Select();
        else
            currentAstralBody.gameObject.GetComponent<SolarSystem>().Select();

        //On dé-parente la caméra (pas très utile, mais plus esthétique dans la hierarchie)
        camera2.transform.SetParent(null);

        //On parente la caméra afin qu'elle bouge correctement avec les planètes ou le système solaire
        this.transform.SetParent(t);

    }
}
