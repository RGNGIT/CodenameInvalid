using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispTableResizer : MonoBehaviour 
{
	protected bool horizontal;
	protected WispColumn targetColumn;
	protected WispRow targetRow;
	protected float startingY;
	protected float startingHeight;
	protected WispTable parentTable;
	protected float minX;
	protected float minY;
	protected bool mouseIn = false;

	public WispColumn TargetColumn {
		get {
			return targetColumn;
		}
		set {
			targetColumn = value;
			targetRow = null;
			horizontal = false;
			parentTable = targetColumn.ParentGrid;
		}
	}

	public WispRow TargetRow {
		get {
			return targetRow;
		}
		set {
			targetRow = value;
			targetColumn = null;
			horizontal = true;
			parentTable = targetRow.ParentTable;
		}
	}

	void Start ()
	{
		// GetComponent<Image> ().ApplyStyle_Inactive(parentTable.Style, parentTable.Opacity, WispSubStyleRule.ResizingBar);
		GetComponent<Image> ().color = parentTable.Style.ResizingBarInactiveColor.ColorOpacity(parentTable.Opacity);
	}

	// ...
	public void BeginDrag()
	{
		if (horizontal) {
			startingY = Input.mousePosition.y;
			startingHeight = targetRow.Height;
			minY = 10;
		} else {
			minX = 10;
		}
	}

	// ...
	public void onDrag()
	{

		if (horizontal) {

			float oldHeight = targetRow.Height;
			
			targetRow.Height = ((Input.mousePosition.y-startingY))*(-1) + startingHeight;

			if (oldHeight == targetRow.Height)
				return;

			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (GetComponent<RectTransform> ().anchoredPosition.x, targetRow.YPos + targetRow.Height*-1);

			WispRow shiftableRow = targetRow.ParentTable.GetRowBelowThisRow (targetRow);

			if (shiftableRow != null) {
				targetRow.ParentTable.ShiftRowPosition (shiftableRow, targetRow.YPos - targetRow.Height);
			}

			parentTable.UpdateScrollRectSize ();
		
		} else {
			float oldWidth = targetColumn.Width;

			targetColumn.Width = Input.mousePosition.x - targetColumn.transform.position.x;

			if (oldWidth == targetColumn.Width)
				return;

			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (targetColumn.XPos + targetColumn.Width, GetComponent<RectTransform> ().anchoredPosition.y);

			WispColumn shiftableColumn = targetColumn.ParentGrid.GetColumnToTheRight (targetColumn);

			if (shiftableColumn != null) {
				targetColumn.ParentGrid.ShiftColumnPosition (shiftableColumn, targetColumn.XPos + targetColumn.Width);
			}

			parentTable.UpdateScrollRectSize ();
		}
	}

	// ...
	public void EndDrag()
	{
		parentTable.UpdateResizers();
	}

	// ...
	public void PointerEnter()
	{
		// GetComponent<Image> ().ApplyStyle(parentTable.Style, parentTable.Opacity, WispSubStyleRule.ResizingBar);
		GetComponent<Image> ().color = parentTable.Style.ResizingBarActiveColor.ColorOpacity(parentTable.Opacity);
		parentTable.BringElementToFront(transform.parent);
		mouseIn = true;
	}

	// ...
	public void PointerExit()
	{
		GetComponent<Image> ().color = parentTable.Style.ResizingBarInactiveColor.ColorOpacity(parentTable.Opacity);
		parentTable.BringElementToFront(transform.parent);
		mouseIn = false;
	}

	public void UpdateStyle()
	{
		if (mouseIn)
			GetComponent<Image> ().color = parentTable.Style.ResizingBarActiveColor.ColorOpacity(parentTable.Opacity);
		else
			GetComponent<Image> ().color = parentTable.Style.ResizingBarInactiveColor.ColorOpacity(parentTable.Opacity);
	}
}