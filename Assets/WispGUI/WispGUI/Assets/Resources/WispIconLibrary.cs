using UnityEngine;

[CreateAssetMenu(fileName = "Icon Library", menuName = "Wisp GUI/Icon Library", order = 3)]

public class WispIconLibrary : ScriptableObject 
{
	public string LibraryName = "Default";

    [SerializeField] private Sprite add;
    [SerializeField] private Sprite edit;
    [SerializeField] private Sprite file;
    [SerializeField] private Sprite delete;
    [SerializeField] private Sprite history;
    [SerializeField] private Sprite no_image;
    [SerializeField] private Sprite directory;
    [SerializeField] private Sprite hourglass;

    public static WispIconLibrary Default
    {
        get
        {
            return Resources.Load<WispIconLibrary>("Default Icon Library");
        } 
    }

    public Sprite Add { get => add; set => add = value; }
    public Sprite File { get => file; set => file = value; }
    public Sprite Edit { get => edit; set => edit = value; }
    public Sprite Delete { get => delete; set => delete = value; }
    public Sprite History { get => history; set => history = value; }
    public Sprite No_image { get => no_image; set => no_image = value; }
    public Sprite Directory { get => directory; set => directory = value; }
    public Sprite Hourglass { get => hourglass; set => hourglass = value; }
}