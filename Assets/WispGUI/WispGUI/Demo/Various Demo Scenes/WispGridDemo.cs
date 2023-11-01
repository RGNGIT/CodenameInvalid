using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispGridDemo : MonoBehaviour
{
    private WispGrid grid;
    
    // Start is called before the first frame update
    void Start()
    {
        // Define a standard button size for later use;
        const float buttonSize = 64f;
        
        // Assign grid dimensions, 2 columns and 2 rows in this case.
        // The result is 4 cells in total.
        grid = GetComponent<WispGrid>();
        grid.SetDimensions(2,2);
        
        // Create a button at cell 0.
        WispButton btnAdd = WispButton.Create(grid.GetCell(0).MyRectTransform);
        // Assign a width and a height to the button.
        btnAdd.Width = buttonSize;
        btnAdd.Height = buttonSize;
        // Assign an icon to the button.
        btnAdd.EnableIcon = true;
        btnAdd.IconPlacement = WispButton.WispButtonIconPlacement.Full;
        btnAdd.SetIcon(WispIconLibrary.Default.Add);

        // Do the same with a button at cell 1.
        WispButton btnEdit = WispButton.Create(grid.GetCell(1).MyRectTransform);
        btnEdit.Width = buttonSize;
        btnEdit.Height = buttonSize;
        btnEdit.EnableIcon = true;
        btnEdit.IconPlacement = WispButton.WispButtonIconPlacement.Full;
        btnEdit.SetIcon(WispIconLibrary.Default.Edit);

        // Do the same with a button at cell 2.
        WispButton btnDelete = WispButton.Create(grid.GetCell(2).MyRectTransform);
        btnDelete.Width = buttonSize;
        btnDelete.Height = buttonSize;
        btnDelete.EnableIcon = true;
        btnDelete.IconPlacement = WispButton.WispButtonIconPlacement.Full;
        btnDelete.SetIcon(WispIconLibrary.Default.Delete);

        // Do the same with a button at cell 3.
        WispButton btnLoad = WispButton.Create(grid.GetCell(3).MyRectTransform);
        btnLoad.Width = buttonSize;
        btnLoad.Height = buttonSize;
        btnLoad.EnableIcon = true;
        btnLoad.IconPlacement = WispButton.WispButtonIconPlacement.Full;
        btnLoad.SetIcon(WispIconLibrary.Default.Directory);

        // Make the grid cells fit the total size of the grid game object.
        grid.AutoFit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
