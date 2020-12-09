using UnityEngine;
using UnityEditor;

public class OrbitRenderer : MonoBehaviour
{ 
    private LineRenderer line;

    [Header("Parameters")]
    public int segments;
    public float xradius;
    public float yradius;


    [Header("Display")]
    public float lineWidth = 50;
    public float maxView = 500;

    private Camera mainCamera;

    void Awake()
    {
        line = gameObject.GetComponent<LineRenderer>();
        mainCamera = Camera.main;
    }

    public void DrawOrbitFromEditor()
    {
        line = gameObject.GetComponent<LineRenderer>();
        ResetPoints();
        CreatePoints();
    }

    void CreatePoints()
    {
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;

        float x = 0f;
        float y = 0f;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }

    void ResetPoints()
    {
        line.positionCount = 0;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(OrbitRenderer))]
public class OrbitRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OrbitRenderer orbitRenderer = (OrbitRenderer)target;

        EditorGUILayout.LabelField("PARAMETERS: ");
        orbitRenderer.segments = EditorGUILayout.IntField("    Segments:", orbitRenderer.segments);
        orbitRenderer.xradius  = EditorGUILayout.FloatField("    XRadius:", orbitRenderer.xradius);
        orbitRenderer.yradius  = EditorGUILayout.FloatField("    YRadius:", orbitRenderer.yradius);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("DISPLAY: ");
        orbitRenderer.lineWidth = EditorGUILayout.FloatField("   Line width:", orbitRenderer.lineWidth);
        orbitRenderer.maxView   = EditorGUILayout.FloatField("   Max view:", orbitRenderer.maxView);

        EditorGUILayout.Space();

        if (GUILayout.Button("Draw Orbit"))
        {
            orbitRenderer.DrawOrbitFromEditor();
        }
    }
}
#endif
