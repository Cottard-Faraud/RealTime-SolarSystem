using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoUI : MonoBehaviour
{
    private Canvas canvas;
    public TextMeshProUGUI text;

    private string planetName = "";

    private void Start()
    {
        canvas = this.GetComponent<Canvas>();
        canvas.enabled = false;
    }

    void Update()
    {
        string temp = IsMouseOverPlanet();
        transform.position = Input.mousePosition + new Vector3(60, 25, 0);
        if (!planetName.Equals(temp))
        {
            planetName = temp;
            if (planetName.Equals(""))
            {
                canvas.enabled = false;
            }
            else
            {
                canvas.enabled = true;
                text.text = planetName;
            }
        }
    }

    private string IsMouseOverPlanet()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        //On récupère seulement le premier résultat (le premier UI trouvé par le raycast)
        if (raycastResultList.Count != 0 && raycastResultList[0].gameObject.GetComponent<PlanetUI>())
            return raycastResultList[0].gameObject.name;

        return "";
    }
}
