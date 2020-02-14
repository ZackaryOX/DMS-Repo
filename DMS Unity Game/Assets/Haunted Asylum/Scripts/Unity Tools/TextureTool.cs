using UnityEngine;
using UnityEditor;

public class TextureTool : EditorWindow
{
    Renderer _RenderObject;
    public Texture2D _BaseColor;
    public Texture2D _MaskMap;
    Texture2D _NormalMap;

    [MenuItem("Window/TextureTool")]
    public static void showWindow()
    {
        GetWindow<TextureTool>("TextureTool");
    }

    private Texture2D TextureFetcher(string name, Texture2D texture) //Someone made a function that formated the UI for texture placement so I used it 
                                                                     //https://answers.unity.com/questions/1424385/how-to-display-texture-field-with-label-on-top-lik.html
    {
        GUILayout.BeginVertical();
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        Texture2D result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }

    void OnGUI()
    {
        GUILayout.Label("Assets To Apply Texture", EditorStyles.boldLabel);


        EditorGUILayout.BeginHorizontal();
        _BaseColor = TextureFetcher("BasicColor", _BaseColor);
       _MaskMap = TextureFetcher("Mask Map", _MaskMap);
        _NormalMap = TextureFetcher("Normal Map", _NormalMap);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Apply To Selected") && _BaseColor != null && _MaskMap != null && _NormalMap != null)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                _RenderObject = obj.GetComponent<Renderer>();
                _RenderObject.material.SetTexture("_BaseColorMap", _BaseColor);
                _RenderObject.material.SetTexture("_MaskMap", _MaskMap);
                _RenderObject.material.SetTexture("_NormalMap", _NormalMap);
            }
        }
        else
        {
            //Debug.Log("No Textures Applied");
        }
    }
}