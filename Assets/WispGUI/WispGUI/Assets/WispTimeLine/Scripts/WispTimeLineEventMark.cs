using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.UI;
using WispExtensions;

public class WispTimeLineEventMark : WispImage
{
    private List<WispTimeLineEvent> events = new List<WispTimeLineEvent>();
    private long baseTick = 0;
    private long maxTick = 0;
    private float xPos = 0;
    private TextMeshProUGUI textComponent;

    public long BaseTick { get => baseTick; set => baseTick = value; }
    public long MaxTick { get => maxTick; set => maxTick = value; }
    public int EventCount { get => events.Count; }
    public float XPos { get => xPos; set => xPos = value; }

    public override bool Initialize()
    {
        if (isInitialized)
			return true;
		
		base.Initialize();

        // ---------------------------------------------------------------------

        textComponent = transform.Find("Label").GetComponent<TextMeshProUGUI>();

		// ---------------------------------------------------------------------

		isInitialized = true;

        return true;
    }

    
    public void SetLabel (string ParamLabel)
    {
        transform.Find("Label").GetComponent<TMPro.TextMeshProUGUI>().text = ParamLabel;
    }

    public void RegisterEvent(WispTimeLineEvent ParamEvent)
    {
        events.Add(ParamEvent);
    }

    public void UpdateTooltipAndLabel()
    {
        if (events.Count == 0)
            return;

        if (events.Count == 1)
        {
            GetComponent<WispImage>().SetTooltipText(events.First().Name, events.First().Description);
            GetComponent<WispImage>().TooltipConfiguration.fadeDelay = 0.25f;
            SetLabel(events.First().Name);
        }
        else
        {
            StringBuilder sb = new StringBuilder();

            foreach(WispTimeLineEvent evnt in events)
            {
                sb.Append(evnt.Name);
                sb.Append(Environment.NewLine);
            }

            GetComponent<WispImage>().SetTooltipText(events.Count.ToString() + " Events", sb.ToString());
            GetComponent<WispImage>().TooltipConfiguration.fadeDelay = 0.25f;
            SetLabel(events.Count.ToString() + " Events");
        }
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
        
        textComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
    }
}