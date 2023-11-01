using UnityEngine;

[CreateAssetMenu(fileName = "Style Library", menuName = "Wisp GUI/Style Library", order = 4)]

public class WispStyleLibrary : ScriptableObject
{
	
	public string LibraryName = "Default";

    [SerializeField] private WispGuiStyle authoritiesFinest;
    [SerializeField] private WispGuiStyle cleanAsphalt;
    [SerializeField] private WispGuiStyle crimson;
    [SerializeField] private WispGuiStyle deepOcean;
    [SerializeField] private WispGuiStyle frosty;
    [SerializeField] private WispGuiStyle glassmorphism;
    [SerializeField] private WispGuiStyle minimalist;
    [SerializeField] private WispGuiStyle oasis;
    [SerializeField] private WispGuiStyle obsidian;
    [SerializeField] private WispGuiStyle sugarPlum;
    [SerializeField] private WispGuiStyle titaniumBorderless;
    [SerializeField] private WispGuiStyle titanium;
    [SerializeField] private WispGuiStyle trinity;
    [SerializeField] private WispGuiStyle vantaBlack;
    [SerializeField] private WispGuiStyle tokwa;

    public static WispStyleLibrary Default
    {
        get
        {
            return Resources.Load<WispStyleLibrary>("Default Style Library");
        } 
    }

    public WispGuiStyle AuthoritiesFinest { get => authoritiesFinest; set => authoritiesFinest = value; }
    public WispGuiStyle CleanAsphalt { get => cleanAsphalt; set => cleanAsphalt = value; }
    public WispGuiStyle Crimson { get => crimson; set => crimson = value; }
    public WispGuiStyle DeepOcean { get => deepOcean; set => deepOcean = value; }
    public WispGuiStyle Frosty { get => frosty; set => frosty = value; }
    public WispGuiStyle Glassmorphism { get => glassmorphism; set => glassmorphism = value; }
    public WispGuiStyle Minimalist { get => minimalist; set => minimalist = value; }
    public WispGuiStyle Oasis { get => oasis; set => oasis = value; }
    public WispGuiStyle Obsidian { get => obsidian; set => obsidian = value; }
    public WispGuiStyle SugarPlum { get => sugarPlum; set => sugarPlum = value; }
    public WispGuiStyle TitaniumBorderless { get => titaniumBorderless; set => titaniumBorderless = value; }
    public WispGuiStyle Titanium { get => titanium; set => titanium = value; }
    public WispGuiStyle Trinity { get => trinity; set => trinity = value; }
    public WispGuiStyle VantaBlack { get => vantaBlack; set => vantaBlack = value; }
    public WispGuiStyle Tokwa { get => tokwa; set => tokwa = value; }
}