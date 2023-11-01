using UnityEditor;

//---------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(WispVisualComponent))] public class WispVisualComponentInspector : WispEditor { }
[CustomEditor(typeof(WispCircularSlider))] public class WispCircularSliderInspector : WispEditor { }
[CustomEditor(typeof(WispResizingHandle))] public class WispResizingHandleInspector : WispEditor { }
[CustomEditor(typeof(WispFloatingPanel))] public class WispFloatingPanelInspector : WispEditor { }
[CustomEditor(typeof(WispEntityEditor))] public class WispEntityEditorInspector : WispEditor { }
[CustomEditor(typeof(WispFileSelector))] public class WispFileSelectorInspector : WispEditor { }
[CustomEditor(typeof(WispLoadingPanel))] public class WispLoadingPanelInspector : WispEditor { }
[CustomEditor(typeof(WispDropDownList))] public class WispDropDownListInspector : WispEditor { }
[CustomEditor(typeof(WispTextMeshPro))] public class WispTextMeshProInspector : WispEditor { }
[CustomEditor(typeof(WispContextMenu))] public class WispContextMenuInspector : WispEditor { }
[CustomEditor(typeof(WispButtonPanel))] public class WispButtonPanelInspector : WispEditor { }
[CustomEditor(typeof(WispProgressBar))] public class WispProgressBarInspector : WispEditor { }
[CustomEditor(typeof(WispDialogWindow))] public class WispPopupViewInspector : WispEditor { }
[CustomEditor(typeof(WispMessageBox))] public class WispMessageBoxInspector : WispEditor { }
[CustomEditor(typeof(WispScrollView))] public class WispScrollViewInspector : WispEditor { }
[CustomEditor(typeof(WispNodeEditor))] public class WispNodeEditorInspector : WispEditor { }
[CustomEditor(typeof(WispCalendar))] public class WispCalendarInspector : WispEditor { }
[CustomEditor(typeof(WispTimeLine))] public class WispTimeLineInspector : WispEditor { }
[CustomEditor(typeof(WispInputBox))] public class WispInputBoxInspector : WispEditor { }
[CustomEditor(typeof(WispCheckBox))] public class WispCheckBoxInspector : WispEditor { }
[CustomEditor(typeof(WispTitleBar))] public class WispTitleBarInspector : WispEditor { }
[CustomEditor(typeof(WispBarChart))] public class WispBarChartInspector : WispEditor { }
[CustomEditor(typeof(WispTabView))] public class WispTabViewInspector : WispEditor { }
[CustomEditor(typeof(WispEditBox))] public class WispEditBoxInspector : WispEditor { }
[CustomEditor(typeof(WispButton))] public class WispButtonInspector : WispEditor { }
[CustomEditor(typeof(WispSlider))] public class WispSliderInspector : WispEditor { }
[CustomEditor(typeof(WispTable))] public class WispTableInspector : WispEditor { }
[CustomEditor(typeof(WispImage))] public class WispImageInspector : WispEditor { }
[CustomEditor(typeof(WispPanel))] public class WispPanelInspector : WispEditor { }
[CustomEditor(typeof(WispPage))] public class WispPageInspector : WispEditor { }
[CustomEditor(typeof(WispGrid))] public class WispGridInspector : WispEditor { }
[CustomEditor(typeof(WispNode))] public class WispNodeInspector : WispEditor { }

//---------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(WispElementMultiSubInstance))] public class WispElementMultiSubInstanceInspector : WispElementEditor { }
[CustomEditor(typeof(WispElementSubInstance))] public class WispElementSubInstanceInspector : WispElementEditor { }
[CustomEditor(typeof(WispElementImage))] public class WispElementImageInspector : WispElementEditor { }
[CustomEditor(typeof(WispElementText))] public class WispElementTextInspector : WispElementEditor { }
[CustomEditor(typeof(WispElementDate))] public class WispElementDateInspector : WispElementEditor { }
[CustomEditor(typeof(WispElementBool))] public class WispElementBoolInspector : WispElementEditor { }

//---------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(WispDropDownListItem))] public class WispDropDownListItemInspector : WispItemEditor { }
[CustomEditor(typeof(WispFileSelectorItem))] public class WispFileSelectorItemInspector : WispItemEditor { }
[CustomEditor(typeof(WispNodeConnector))] public class WispNodeConnectorInspector : WispItemEditor { }
[CustomEditor(typeof(WispRerouteNode))] public class WispRerouteNodeInspector : WispItemEditor { }
[CustomEditor(typeof(WispGridCell))] public class WispGridCellInspector : WispItemEditor { }

//---------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(WispLineRenderer))] public class WispLineRendererInspector : WispMaskableGraphicEditor { }

//---------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(WispTooltip))] public class WispTooltipInspector : WispNoEditor { }

//---------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(WispAutoSize))] public class WispAutoSizeInspector : WispUtilityEditor { }