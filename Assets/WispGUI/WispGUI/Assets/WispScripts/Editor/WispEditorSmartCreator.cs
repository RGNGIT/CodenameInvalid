using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using WispExtensions;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class WispEditorSmartCreator : EditorWindow
{
    public class WispSmartCreatorResult
    {
        public WispSmartCreatorResult(PropertyInfo ParamInfo, int ParamDistance)
        {
            Info = ParamInfo;
            LevenshteinDistance = ParamDistance;
        }
        
        public PropertyInfo Info;
        public int LevenshteinDistance;
    }

    const int OFFSET_GRID_SIZE = 8;
    
    private bool newInstance = true; 
    private string currentSearchProbe = "Button";
    // private string rememberSearchProbe = "Button";
    private bool autoCanvas = true;
    private bool autoParent = true;
    private Dictionary<int, WispSmartCreatorResult> searchResults = new Dictionary<int, WispSmartCreatorResult>();
    private int selectedResult = 0;
    private WispSmartCreatorResult selectedResultInfo;
    PropertyInfo[] componentArray;

    // Duplication Array
    private int XArray = 1;
    private int YArray = 1;
    private int XOffset = 16;
    private int YOffset = 16;

    // Style
    private Color basicColor;
    private Color selectionColor = new Color(0.4f,0.6f,1,1);
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;

    public static void Init()
    {
        WispEditorSmartCreator window = (WispEditorSmartCreator)EditorWindow.GetWindow(typeof(WispEditorSmartCreator));
        window.Show();
    }

    void OnGUI()
    {
        #region Style initialization
        basicColor = GUI.backgroundColor;

        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.05f, 0.25f, 0.45f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");
        #endregion

        // Draw logo
        GUILayout.Box(logo.texture, logoStyle);

        // Keyboard events
        Event e = Event.current;
        bool readTypingInput = true;

        if (e.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == (KeyCode.Escape))
            {
                this.Close();
            }
            else if (Event.current.keyCode == (KeyCode.Return))
            {
                if (selectedResultInfo != null)
                    Create(selectedResultInfo.Info);

                this.Close();
            }
            else if (Event.current.keyCode == (KeyCode.DownArrow))
            {
                selectedResult++;

                if (selectedResult > 4)
                    selectedResult = 0;

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.UpArrow))
            {
                selectedResult--;

                if (selectedResult < 0)
                    selectedResult = 4;

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.KeypadPlus))
            {
                if (e.modifiers == EventModifiers.Alt)
                {
                    YArray++;
                }
                else if (e.modifiers == EventModifiers.Control)
                {
                    XArray++;
                }

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.KeypadMinus))
            {
                if (e.modifiers == EventModifiers.Alt)
                {
                    YArray--;
                }
                else if (e.modifiers == EventModifiers.Control)
                {
                    XArray--;
                }

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Keypad6))
            {
                if (e.modifiers == EventModifiers.Control)
                {
                    XOffset += OFFSET_GRID_SIZE;
                }

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Keypad4))
            {
                if (e.modifiers == EventModifiers.Control)
                {
                    XOffset -= OFFSET_GRID_SIZE;
                }

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Keypad8))
            {
                if (e.modifiers == EventModifiers.Control)
                {
                    YOffset += OFFSET_GRID_SIZE;
                }

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Keypad2))
            {
                if (e.modifiers == EventModifiers.Control)
                {
                    YOffset -= OFFSET_GRID_SIZE;
                }

                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Alpha1))
            {
                selectedResult = 0;
                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Alpha2))
            {
                selectedResult = 1;
                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Alpha3))
            {
                selectedResult = 2;
                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Alpha4))
            {
                selectedResult = 3;
                e.Use();
            }
            else if (Event.current.keyCode == (KeyCode.Alpha5))
            {
                selectedResult = 4;
                e.Use();
            }
        }

        ClampArrayDuplicationSettings();

        // Set style
        EditorGUILayout.BeginVertical(editorStyle);

        // Research
        GUI.SetNextControlName("SearchField");
        GUILayout.Label("Search for GUI component : ");

        if (readTypingInput)
            currentSearchProbe = EditorGUILayout.TextField(currentSearchProbe);
        else
            EditorGUILayout.TextField(currentSearchProbe);

        if (newInstance)
            EditorGUI.FocusTextInControl("SearchField");

        GUILayout.Space(16f);
        GUILayout.Label("Search results : ");
        GUILayout.Space(4f);

        if (componentArray == null)
            componentArray = typeof(WispPrefabLibrary).GetProperties();

        searchResults.Clear();

        for (int i = 0; i < componentArray.Length; i++)
        {
            if (componentArray[i].GetType() == typeof(GameObject) || true)
            {
                string nLow = componentArray[i].Name.ToLower();
                string pLow = currentSearchProbe.ToLower();
                
                int levDist = nLow.GetLevensteinDistance(pLow);

                if (nLow.StartsWith(pLow))
                    levDist -= pLow.Length;

                if (nLow.Contains(pLow))
                    levDist -= pLow.Length;

                searchResults.Add(i, new WispSmartCreatorResult(componentArray[i], levDist));
            }
        }

        DrawResults(searchResults);

        // Creation options
        GUILayout.Space(16f);
        autoCanvas = GUILayout.Toggle(autoCanvas, "Find or create canvas.");
        autoParent = GUILayout.Toggle(autoParent, "Follow parent style.");
        GUILayout.Space(8f);

        GUILayout.Label("Duplication array : ");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("X Count : ");
        EditorGUILayout.TextField(XArray.ToString());
        GUILayout.Label("Y Count : ");
        EditorGUILayout.TextField(YArray.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("X Offset : ");
        EditorGUILayout.TextField(XOffset.ToString());
        GUILayout.Label("Y Offset : ");
        EditorGUILayout.TextField(YOffset.ToString());
        EditorGUILayout.EndHorizontal();

        // Unset style
        EditorGUILayout.EndVertical();

        // OnGUI happened at least once.
        newInstance = false;
    }

    private void ClampArrayDuplicationSettings()
    {
        if (XArray < 1)
            XArray = 1;

        if (YArray < 1)
            YArray = 1;

        if (XOffset < 0)
            XOffset = 0;

        if (YOffset < 0)
            YOffset = 0;
    }

    private void DrawResults(Dictionary<int, WispSmartCreatorResult> ParamResults)
    {
        // Sort
        int max = ParamResults.Count - 1;

        for (int i = 0; i < ParamResults.Count; i++)
        {
            for (int j = 0; j < max; j++)
            {
                if (ParamResults[j].LevenshteinDistance > ParamResults[j+1].LevenshteinDistance)
                {
                    WispSmartCreatorResult temp = ParamResults[j];
                    ParamResults[j] = ParamResults[j+1];
                    ParamResults[j+1] = temp;
                }
            }

            max--;
        }
        
        // Draw
        int c = 0;

        foreach(KeyValuePair<int, WispSmartCreatorResult> kvp in ParamResults)
        {
            if (selectedResult == c)
            {
                GUI.backgroundColor = selectionColor;
                selectedResultInfo = kvp.Value;
            }
            else
            {
                GUI.backgroundColor = basicColor;
            }
            
            if (GUILayout.Button(ParamResults[c].Info.Name/* + " (" + kvp.Value.LevenshteinDistance.ToString() + ")"*/))
            {
                if (selectedResultInfo != null)
                {
                    Create(kvp.Value.Info);
                    this.Close();
                }
            }

            c++;

            if (c > 4)
                break;
        }

        GUI.backgroundColor = basicColor;
    }

    private GameObject Create(/*string ParamGameObjectName*/ PropertyInfo ParamProperty)
    {
        GameObject prefab = ParamProperty.GetValue(WispPrefabLibrary.Default) as GameObject;
        
        if (prefab == null)
            return null;

        GameObject go = null;
        
        for (int i = 0; i < XArray; i++)
        {
            for (int j = 0; j < YArray; j++)
            {
                go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                if (Selection.activeTransform != null)
                {
                    GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

                    WispVisualComponent parentVC = Selection.activeTransform.gameObject.GetComponent<WispVisualComponent>();
                    WispVisualComponent vc = go.GetComponent<WispVisualComponent>();

                    // Find parent RectTransform
                    RectTransform parentRT;

                    if (parentVC == null)
                    {
                        parentRT = Selection.activeTransform.GetComponent<RectTransform>();
                    }
                    else
                    {
                        parentRT = parentVC.MyRectTransform;
                    }
                    
                    // Assign parent VC
                    if (parentVC != null)
                    {
                        vc.SetParent(parentVC, true);
                        vc.Wysiwyg();
                    }

                    // Assign Target
                    if (parentRT != null)
                    {
                        if (vc is WispTitleBar)
                        {
                            (vc as WispTitleBar).Target = parentRT;
                        }
                        else if (vc is WispResizingHandle)
                        {
                            (vc as WispResizingHandle).Target = parentRT;
                        }
                    }

                    /*
                    // bool hr = Highlighter.Highlight ("Inspector", "defaultText");
                    // Debug.Log("HR : " + hr);
                    // // Highlighter.Stop();
                    */

                    /*
                    UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
                    UnityEngine.Object[] view = Resources.FindObjectsOfTypeAll(typeof(GUIView));

                    foreach(EditorWindow win in windows)
                    {
                        if (win.GetType().ToString() == "UnityEditor.InspectorWindow")
                        {
                            Debug.Log("Window : " + win.titleContent.text + " /// Type : " + win.GetType().ToString());
                            win.Focus();

                            for (int k = 0; k < win.rootVisualElement.childCount; k++)
                            {
                                Debug.Log("Child : " + win.rootVisualElement.ElementAt(k).name);
                            }

                            GUI.FocusControl("DEFAULT_FOCUS");
                        }
                    }
                    */
                }
                else if (autoCanvas)
                {
                    Canvas canvas = WispVisualComponent.GetMainCanvas();

                    if (canvas != null)
                    {
                        GameObjectUtility.SetParentAndAlign(go, canvas.gameObject);
                    }
                    else
                    {
                        GameObject canvas_go = PrefabUtility.InstantiatePrefab(WispPrefabLibrary.Default.Canvas) as GameObject;
                        GameObjectUtility.SetParentAndAlign(go, canvas_go);
                    }

                    RectTransform rt = go.GetComponent<RectTransform>();
                    rt.position += new Vector3(rt.rect.width*i + XOffset*i, -(rt.rect.height*j + YOffset*j), 0);
                }

                Undo.RegisterCreatedObjectUndo(go, "Create " + prefab.name);
            }
        }

        Selection.activeObject = go;
        return go;
    }
}