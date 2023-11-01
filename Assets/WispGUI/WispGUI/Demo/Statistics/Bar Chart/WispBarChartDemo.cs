using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispBarChartDemo : MonoBehaviour
{
    public WispBarChart chart;
    public WispButton regenBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        regenBtn.AddOnClickAction(Regenerate);

        Regenerate();
    }

    private void Regenerate()
    {
        #region Generate some random data
        const float minDamage = 1000;
        const float maxDamage = 10000;

        Dictionary<string, float> playerDamage = new Dictionary<string, float>();

        float maxDamagePerPlayer = UnityEngine.Random.Range(minDamage, maxDamage);
        
        for (int i = 1; i <= 8; i++)
        {
            playerDamage.Add("Player " + i.ToString(), UnityEngine.Random.Range(minDamage, maxDamagePerPlayer));
        }
        #endregion
        
        // Use the random data to draw a chart
        chart.DrawChart(playerDamage, 0, maxDamage, 4, "Damage dealt by player");
    }
}