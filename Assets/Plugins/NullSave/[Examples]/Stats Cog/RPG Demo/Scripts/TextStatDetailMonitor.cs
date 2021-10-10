using NullSave.TOCK.Stats;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextStatDetailMonitor : MonoBehaviour
{

    #region Variables

    public StatsCog statCog;
    public string statName;
    public string formattedText = "<color=orange>{0}</color> <color=#FFFFFF88>[{1}]</color> <color=#4378a4>({2})</color>";
    public int decimals = 1;

    private Text text;
    private StatValue stat;
    #endregion

    #region Unity Methods

    public void Start()
    {
        text = GetComponent<Text>();
        stat = statCog.FindStat(statName);
        if (stat != null)
        {
            stat.onInit.AddListener(ValueInit);
            stat.onValueChanged.AddListener(ValueChanged);
            stat.onBaseValueChanged.AddListener(ValueChanged);
            ValueInit();
        }
    }

    #endregion

    #region Private Methods

    private string FormatValue(float value)
    {
        return System.Math.Round(value, decimals).ToString();
    }

    private void ValueInit()
    {
        string output = formattedText;

        output = output.Replace("{0}", FormatValue(stat.CurrentValue));
        output = output.Replace("{1}", FormatValue(stat.CurrentBaseValue));
        output = output.Replace("{2}", FormatValue(stat.CurrentValue - stat.CurrentBaseValue));

        text.text = output;
    }

    private void ValueChanged(float oldValue, float newValue)
    {
        string output = formattedText;

        output = output.Replace("{0}", FormatValue(stat.CurrentValue));
        output = output.Replace("{1}", FormatValue(stat.CurrentBaseValue));
        output = output.Replace("{2}", FormatValue(stat.CurrentValue - stat.CurrentBaseValue));

        text.text = output;
    }

    #endregion

}
