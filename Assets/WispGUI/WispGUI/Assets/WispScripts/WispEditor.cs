using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR

public class WispEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private WispVisualComponent me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.05f, 0.25f, 0.45f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Store component
        if (me == null)
            me = (WispVisualComponent)target;

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);

        EditorGUILayout.BeginVertical(editorStyle);
        //DrawPropertiesExcluding(serializedObject, "m_Script");
        DrawDefaultInspector();
        // GUI.FocusControl("DEFAULT_FOCUS");
        EditorGUILayout.EndVertical();

        /*
        SerializedProperty p = serializedObject.FindProperty("defaultText");

        if (p == null)
        {
            Debug.Log("PNAME = " + "NULL");
        }
        else
        {
            Debug.Log("PNAME = " + p.name);
        }
        */

        if (GUI.changed)
        {
            if (Application.isPlaying)
                me.ApplyStyle();
            // else
            //     me.ApplyStyleInEditor();

            // me.Wysiwyg();
        }
    }
}

public class WispElementEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private WispVisualComponent me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.45f, 0.25f, 0.05f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Store component
        if (me == null)
            me = (WispVisualComponent)target;

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);

        EditorGUILayout.BeginVertical(editorStyle);
        DrawDefaultInspector();
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            if (Application.isPlaying)
                me.ApplyStyle();
            // else
            //     me.ApplyStyleInEditor();

            // me.Wysiwyg();
        }
    }
}

public class WispMaskableGraphicEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private MaskableGraphic me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.5f, 0.5f, 0.5f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Store component
        if (me == null)
            me = (MaskableGraphic)target;

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);

        EditorGUILayout.BeginVertical(editorStyle);
        //DrawPropertiesExcluding(serializedObject, "m_Script");
        DrawDefaultInspector();
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            /*
            if (Application.isPlaying)
                me.ApplyStyle();
            else
                me.ApplyStyleInEditor();

            me.Wysiwyg();
            */
        }
    }
}

public class WispNoEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private WispVisualComponent me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.5f, 0.5f, 0.5f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Store component
        if (me == null)
            me = (WispVisualComponent)target;

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);
        
        EditorGUILayout.BeginVertical(editorStyle);
        //DrawPropertiesExcluding(serializedObject, "m_Script");
        //DrawDefaultInspector();
        GUILayout.Label("Tooltip ID : " + me.Id, GUI.skin.label);
        GUILayout.Label("Depth in hierarchy : " + me.DepthInHierarchy, GUI.skin.label);
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            /*
            if (Application.isPlaying)
                me.ApplyStyle();
            else
                me.ApplyStyleInEditor();

            me.Wysiwyg();
            */
        }
    }
}

[CustomEditor(typeof(WispKeyBoardEventSystem))]
public class WispKeyBoardEventSystemEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private WispKeyBoardEventSystem me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
            logoStyle.normal.textColor = Color.gray;
            logoStyle.fontSize = 19;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.05f, 0.25f, 0.45f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Store component
        if (me == null)
            me = (WispKeyBoardEventSystem)target;

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);

        EditorGUILayout.BeginVertical(editorStyle);
        GUILayout.Label("Keyboard Event System", logoStyle);
        
        GUILayout.Label("Event count per key :");
        GUILayout.TextArea(me.DebugInfoText);

        if (me.LastExecutedAction != null)
        {
            GUILayout.Label("Last executed action :");
            GUILayout.TextArea(me.LastExecutedAction.ToString());
            GUILayout.Label("Last executed action selection method :");
            GUILayout.TextArea(me.LastresultType.ToString());
            GUILayout.Label("Last executed action selection depth :");
            GUILayout.TextArea(me.LastResultDepth.ToString());
        }

        EditorGUILayout.EndVertical();
    }
}

// ----------------------------------------------------------------------------------------------------------------------

public class WispItemEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private WispVisualComponent me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.05f, 0.45f, 0.20f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Store component
        if (me == null)
            me = (WispVisualComponent)target;

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);

        EditorGUILayout.BeginVertical(editorStyle);
        //DrawPropertiesExcluding(serializedObject, "m_Script");
        DrawDefaultInspector();
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            if (Application.isPlaying)
                me.ApplyStyle();
            // else
            //     me.ApplyStyleInEditor();

            // me.Wysiwyg();
        }
    }
}

// ----------------------------------------------------------------------------------------------------------------------

public class WispUtilityEditor : Editor
{
    private GUIStyle logoStyle;
    private GUIStyle editorStyle;
    private Texture2D background;
    private Sprite logo;
    private WispVisualComponent me;

    public override void OnInspectorGUI()
    {
        if (logoStyle == null)
        {
            logoStyle = new GUIStyle(GUIStyle.none);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            logoStyle.fixedHeight = 64;
        }

        if (background == null)
            background = WispTextureTools.CreateGradTexture(WispTextureTools.GenerateGradientToCenter(new Color(0.56f, 0f, 1f)), Mathf.CeilToInt(EditorGUIUtility.currentViewWidth), 1);

        // Generate BG style
        if (editorStyle == null)
        {
            editorStyle = new GUIStyle(GUI.skin.box);
            editorStyle.normal.background = background;
        }

        if (logo == null)
            logo = Resources.Load<Sprite>("Logo");

        GUILayout.Box(logo.texture, logoStyle);

        EditorGUILayout.BeginVertical(editorStyle);
        DrawDefaultInspector();
        EditorGUILayout.EndVertical();
    }
}

#endif