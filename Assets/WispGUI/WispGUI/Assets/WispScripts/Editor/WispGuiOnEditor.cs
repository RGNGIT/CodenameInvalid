using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class WispGuiOnEditor
{
    private static string currentScene;
    private static int count;
    const int FRAMES_UNTIL_SLOW_TICK = 50;

    static WispGuiOnEditor()
    {
        currentScene = EditorSceneManager.GetActiveScene().path;
        EditorApplication.hierarchyChanged += HierarchyWindowChanged;
        Selection.selectionChanged += HierarchyWindowChanged;

        // System to update every FRAMES_UNTIL_SLOW_TICK GUI Updates (Editor Frames sort of...)
        EditorApplication.update += OnUpdate;
    }

    private static void HierarchyWindowChanged()
    {
        if (Selection.activeTransform != null)
        {
            WispVisualComponent vc = Selection.activeTransform.GetComponent<WispVisualComponent>();
            
            if (vc != null)
            {
                WispVisualComponent.LastSelectedInHierarchy = vc;
            }
        }
        
        if (currentScene != EditorSceneManager.GetActiveScene().path)
        {
            InitializeWispGUI();
            currentScene = EditorSceneManager.GetActiveScene().path;
        }
    }

    private static void InitializeWispGUI()
    {
        WispVisualComponent[] visualComponents = UnityEngine.Object.FindObjectsOfType<WispVisualComponent>();
        int len = visualComponents.Length;

        for (int i = 0; i < len; i++)
        {
            visualComponents[i].Wysiwyg();
        }

    }

    static void OnUpdate()
    {
        if (++count > FRAMES_UNTIL_SLOW_TICK)
        {
            count = 0;
            SlowTick();
        }
    }

    // Thanks To : https://answers.unity.com/questions/1418564/how-can-i-run-a-editorscript-every-few-seconds.html
    static void SlowTick()
    {
        // Debug.Log("Slow Tick");

        if (WispVisualComponent.ProcessWysiwygWaitingList())
            EditorApplication.QueuePlayerLoopUpdate();

        if (WispVisualComponent.ProcessApplyStyleWaitingList())
            EditorApplication.QueuePlayerLoopUpdate();

        // UnityEditor.SceneView.RepaintAll();
    }
}