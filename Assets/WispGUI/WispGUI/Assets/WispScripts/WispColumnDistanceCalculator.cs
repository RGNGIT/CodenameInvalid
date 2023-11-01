using UnityEngine;

public class WispColumnDistanceCalculator
{
	protected int columnCount;
	protected float[] columnOffset;
	protected float[] columnXPostion;
	protected int currentColumn;
	protected float columnSize;
	protected float startingX;

	public WispColumnDistanceCalculator (int ParamColumnCount, float ParamComponentWidth, float ParamParentWidth, float ParamStartingY, float ParamStartingX, float ParamXMargin)
	{
		startingX = ParamStartingX;

		columnCount = ParamColumnCount;

		// Calculate the size of each column
		columnSize = (ParamParentWidth / ParamColumnCount);

		// Initialize the columns offset and x positions array
		columnOffset = new float[ParamColumnCount];

		for (int i = 0; i < columnOffset.Length; i++) 
		{
			columnOffset [i] = ParamStartingY;
		}

		columnXPostion = new float[ParamColumnCount];

		for (int i = 0; i < columnXPostion.Length; i++) 
		{
            columnXPostion[i] = (columnSize * i) + (columnSize / 2) - (ParamComponentWidth / 2) + ParamXMargin;
        }

		// Current column
		currentColumn = 0;
	}

	// ...
	public float GetCurrentX()
	{
		return columnXPostion [currentColumn] + startingX;
	}

	// ...
	public float GetCurrentY()
	{
		return columnOffset [currentColumn];
	}

	// ...
	public int GetNextColumn ()
	{
		currentColumn++;

		if (currentColumn > columnCount - 1)
			currentColumn = 0;

		return currentColumn;
	}

	// ...
	public void PushY (float ParamAmount)
	{
		columnOffset [currentColumn] += ParamAmount;
	}

	// ...
	protected void PushAllY(int ParamAmount)
	{
		int i;
		for (i = 0; i < columnOffset.Length; i++) {

			columnOffset [i] += ParamAmount;

		}
	}


	// ...
	public float GetMaxY ()
	{
		float tmp = 0;

		for (int i = 0; i < columnOffset.Length; i++) {
		
			if (columnOffset [i] > tmp)
				tmp = columnOffset [i];
		
		}

		return tmp;
	}

	// ...
	protected void EquilizeVerticalOffset ()
	{
		int i;
		float max = 0;

		for (i = 0; i < columnOffset.Length; i++) {

			if (columnOffset [i] > max)
				max = columnOffset [i];
		
		}

		for (i = 0; i < columnOffset.Length; i++) {

			columnOffset [i] = max;

		}
	}

	// For visual components that require a full row
	public void ForceColumnToFirst (int ParamVerticalPushAmount)
	{
		EquilizeVerticalOffset ();

		if (currentColumn > 0) {
			currentColumn = 0;

		}

		PushAllY (ParamVerticalPushAmount);
	}

    // ...
    public float GetColumnXPosition(int ParamColumnIndex)
    {
        return columnXPostion[ParamColumnIndex];
    }
}

