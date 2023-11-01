using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using WispExtensions;
using TMPro;
using System;

public static class WispEditorMenuTools
{
    [MenuItem("WispGUI/Tool Box %SPACE", false, 0)]
    static void OpenSmartCreator()
    {
        if (Application.isEditor && !Application.isPlaying)
            WispEditorSmartCreator.Init();
        else if (Application.isEditor && Application.isPlaying && !Application.isFocused)
            WispEditorSmartCreator.Init();
    }

    [MenuItem("WispGUI/Rebuild Global Component List", false, 1)]
    static void RebuildGlobalComponentList()
    {
        WispVisualComponent.RebuildGlobalComponentList();
    }

    [MenuItem("WispGUI/Find WispGUI Asset folder", false, 2)]
    static void OpenAssetFolder()
    {
        var obj = AssetDatabase.LoadMainAssetAtPath("Assets/WispGUI/Assets");
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
    
    [MenuItem("GameObject/WispGUI/Auto Parent children", false, 3)]
    static void AutoParentChildren()
    {
        if (Selection.activeTransform != null)
        {
            WispVisualComponent vc = Selection.activeTransform.GetComponent<WispVisualComponent>();

            if (vc != null)
            {
                vc.AutoSetParents(true);
                EditorUtility.SetDirty(vc);
                EditorUtility.SetDirty(vc.gameObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(vc.gameObject.scene);
            }
        }
    }

    [MenuItem("GameObject/WispGUI/Shift position by...", false, 4)]
    static void OpenPositionShifter()
    {
        WispEditorPositionShifter.Init();
    }

    [MenuItem("GameObject/WispGUI/Create WispGUI equivalent", false, 5)]
    static void AutoCreateEquivalent()
    {
        if (Selection.activeTransform != null)
        {
            if (Selection.activeTransform.GetComponent<Button>())
            {
                Debug.Log("Creating Button equivalent...");
                
                Button btn_1 = Selection.activeTransform.GetComponent<Button>();
                WispButton btn_2 = WispButton.Create(btn_1.transform.parent);

                btn_2.GetComponent<RectTransform>().SetSettingsFrom(btn_1.GetComponent<RectTransform>());
                btn_2.gameObject.name = "WispButton (" + btn_1.name + ")";
                
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(Selection.activeTransform.gameObject.scene);
            }
            else if (Selection.activeTransform.GetComponent<TMP_InputField>())
            {
                Debug.Log("Creating Inputfield equivalent...");
                
                TMP_InputField field_1 = Selection.activeTransform.GetComponent<TMP_InputField>();
                WispEditBox field_2 = WispEditBox.Create(field_1.transform.parent);

                field_2.GetComponent<RectTransform>().SetSettingsFrom(field_1.GetComponent<RectTransform>());
                field_2.gameObject.name = "WispEditBox (" + field_1.name + ")";
                
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(Selection.activeTransform.gameObject.scene);
            }
            else if (Selection.activeTransform.GetComponent<Image>())
            {
                Debug.Log("Creating panel equivalent...");
                
                Image pan_1 = Selection.activeTransform.GetComponent<Image>();
                WispPanel pan_2 = WispPanel.Create(pan_1.transform.parent);

                pan_2.GetComponent<RectTransform>().SetSettingsFrom(pan_1.GetComponent<RectTransform>());
                pan_2.gameObject.name = "WispPanel (" + pan_1.name + ")";
                
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(Selection.activeTransform.gameObject.scene);
            }
            
        }
    }

    [MenuItem("GameObject/WispGUI/Convert To Window", false, 6)]
    static void ConvertToWindow()
    {
        if (Selection.activeTransform != null)
        {
            RectTransform parent_rt = Selection.activeTransform.GetComponent<RectTransform>();

            if (parent_rt == null)
            {
                WispVisualComponent.LogError("Can't convert to window ! please select a game object with a RectTransform component.");
                return;
            }


            WispTitleBar bar = WispTitleBar.Create(parent_rt);
            bar.Target = parent_rt;

            WispResizingHandle handle = WispResizingHandle.Create(parent_rt, parent_rt, 32f, 32f);
            
            // Styling
            WispVisualComponent parent_vc = parent_rt.GetComponent<WispVisualComponent>();

            if (parent_vc != null)
            {
                bar.SetParent(parent_vc, true);
                handle.SetParent(parent_vc, true);
            }
        }
    }
}