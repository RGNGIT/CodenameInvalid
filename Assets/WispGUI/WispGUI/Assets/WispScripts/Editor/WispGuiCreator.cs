using UnityEngine;
using UnityEditor;

public static class WispGuiCreator
{
    #region Helper functions
    private static GameObject LoadGoFromAsset(string ParamAssetPath)
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath(ParamAssetPath, typeof(GameObject)) as GameObject;
        return PrefabUtility.InstantiatePrefab(asset) as GameObject;
    }
    #endregion

    #region Input

    [MenuItem("GameObject/WispGUI/Create/Input/Edit Box")]
    static void CreateEditBox()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispEditBox/Prefab/WispEditBox.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Edit box");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Input/Button")]
    static void CreateButton()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispButton/Prefab/WispButton.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Button");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Input/Check Box")]
    static void CreateCheckBox()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispCheckBox/Prefab/WispCheckBox.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Check box");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Input/Slider")]
    static void CreateSlider()
    {
        // GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispCheckBox/Prefab/WispCheckBox.prefab");
        GameObject go = WispSlider.Create(null).gameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Slider");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Input/Circular Slider")]
    static void CreateCircularSlider()
    {
        GameObject go = WispCircularSlider.Create(null).gameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Circular Slider");
        Selection.activeObject = go;
    }

    #endregion

    #region Selector

    [MenuItem("GameObject/WispGUI/Create/Selector/Calendar")]
    static void CreateCalendar()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispEditBox/WispCalendar/Prefab/WispCalendar.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Calendar");
        Selection.activeObject = go;
    }

    #endregion

    #region Container

    [MenuItem("GameObject/WispGUI/Create/Container/Grid")]
    static void CreateGrid()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispGrid/Prefab/WispGrid.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Grid");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Container/Button Panel")]
    static void CreateButtonPanel()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispButtonPanel/Prefab/WispButtonPanel.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Button Panel");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Container/Scroll View")]
    static void CreateScrollView()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispScrollView/Prefab/WispScrollView.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Scroll View");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Container/Tab View")]
    static void CreateTabView()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispTabView/Prefab/WispTabView.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Tab View");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Container/Panel")]
    static void CreatePanel()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispPanel/Prefab/WispPanel.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Panel");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Container/Floating Panel")]
    static void CreateFloatingPanel()
    {
        // GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispPanel/Prefab/WispPanel.prefab");
        GameObject go = WispFloatingPanel.Create(null).gameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Floating Panel");
        Selection.activeObject = go;
    }

    #endregion

    #region Organiser

    [MenuItem("GameObject/WispGUI/Create/Organiser/Table")]
    static void CreateTable()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispTable/Prefab/WispTable.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Table");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Organiser/TimeLine")]
    static void CreateTimeLine()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispTimeLine/Prefab/WispTimeLine.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create TimeLine");
        Selection.activeObject = go;
    }

    #endregion

    #region Popup

    [MenuItem("GameObject/WispGUI/Create/Popup/Message Box")]
    static void CreateMessageBox()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispMessageBox/Prefab/WispMessageBox.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Message Box");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Popup/File Selector")]
    static void CreateFileSelector()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispFileSelector/Prefab/WispFileSelector.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create File Selector");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Popup/Input Box")]
    static void CreateInputBox()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispInputBox/Prefab/WispInputBox.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Input Box");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Popup/Dialog Window")]
    static void CreatePopupView()
    {
        // GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispPopupView/Prefab/WispPopupView.prefab");
        GameObject go = WispDialogWindow.Create(null).gameObject;
        Debug.Log(go);

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Dialog Window");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Popup/Context Menu")]
    static void CreateContextMenu()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispContextMenu/Prefab/WispContextMenu.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Context menu");
        Selection.activeObject = go;
    }

    #endregion

    #region Editor

    [MenuItem("GameObject/WispGUI/Create/Editor/Entity Editor")]
    static void CreateEntityEditor()
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath("Assets/WispGUI/Assets/WispEntityEditor/Prefab/WispEntityEditor.prefab", typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(asset) as GameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Entity Editor");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Editor/Node Editor")]
    static void CreateNodeEditor()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispNodeEditor/Prefab/WispNodeEditor.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Node editor");
        Selection.activeObject = go;
    }

    #endregion

    #region Media

    [MenuItem("GameObject/WispGUI/Create/Media/Image")]
    static void CreateImage()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispImage/Prefab/WispImage.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Image");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Media/TextMeshPro")]
    static void CreateTextMeshPro()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispTextMeshPro/WispTextMeshPro.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create TextMeshPro");
        Selection.activeObject = go;
    }

    #endregion

    #region Feedback

    [MenuItem("GameObject/WispGUI/Create/Feedback/Loading panel")]
    static void CreateLoadingPanel()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispLoadingPanel/Prefab/WispLoadingPanel.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Loading panel");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Feedback/Progress Bar")]
    static void CreateProgressBar()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispProgressBar/Prefab/WispProgressBar.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Progress Bar");
        Selection.activeObject = go;
    }

    #endregion

    #region Utility

    [MenuItem("GameObject/WispGUI/Create/Utility/Title Bar")]
    static void CreateTitleBar()
    {
        GameObject go = LoadGoFromAsset("Assets/WispGUI/Assets/WispTitleBar/Prefab/WispTitleBar.prefab");

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Title Bar");
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/WispGUI/Create/Utility/Resizing Handle")]
    static void CreateResizingHandle()
    {
        GameObject go = WispResizingHandle.Create(null).gameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Resizing Handle");
        Selection.activeObject = go;
    }

    #endregion

    #region Statistics

    [MenuItem("GameObject/WispGUI/Create/Statistics/Bar Chart")]
    static void CreateBarChart()
    {
        GameObject go = WispBarChart.Create(null).gameObject;

        if (Selection.activeTransform != null)
            GameObjectUtility.SetParentAndAlign(go, Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Bar Chart");
        Selection.activeObject = go;
    }

    #endregion
}