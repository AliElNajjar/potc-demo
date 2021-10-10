using NullSave.TOCK.Stats;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextStatValueMonitor : MonoBehaviour
{

    #region Variables

    public StatsCog statCog;
    public string formattedText = "{STATNAME}";
    public bool displayAsInt = false;

    private Text text;

    private List<StatValue> stats;

    #endregion

    #region Unity Methods

    public void Start()
    {
        text = GetComponent<Text>();
        stats = new List<StatValue>();

        int i, e;
        string val = formattedText;
        string statName;
        StatValue stat;

        // Find stats
        while (true)
        {
            i = val.IndexOf('{');
            e = val.IndexOf('}');
            if (i < 0 || e < 0) break;

            statName = val.Substring(i + 1, e - i - 1);

            stat = statCog.FindStat(statName);
            if (stat != null)
            {
                stat.onValueChanged.AddListener(ValueChanged);
                stats.Add(stat);
            }

            val = val.Substring(e + 1);
        }

        ValueChanged(0, 0);
    }

    #endregion

    #region Private Methods

    private void ValueChanged(float oldValue, float newValue)
    {
        string output = formattedText;

        foreach (StatValue stat in stats)
        {
            if (displayAsInt)
            {
                output = output.Replace("{" + stat.name + "}", ((int)stat.CurrentValue).ToString());
            }
            else
            {
                output = output.Replace("{" + stat.name + "}", stat.CurrentValue.ToString());
            }
        }

        text.text = output;
    }

    #endregion

}
