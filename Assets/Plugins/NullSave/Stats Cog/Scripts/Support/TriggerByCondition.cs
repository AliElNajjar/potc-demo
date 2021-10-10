using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [HierarchyIcon("condition_check")]
    public class TriggerByCondition : MonoBehaviour
    {

        #region Variables

        public StatsCog statsCog;
        public string expression = "1 = 2";
        public UnityEvent onTrue, onFalse;

        private List<StatValue> subs = new List<StatValue>();
        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            Subscribe();
            Evaluate(0, 0);
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Start()
        {
            Unsubscribe();
            Subscribe();
            Evaluate(0, 0);
        }

        #endregion

        #region Private Methods

        private void Evaluate(float oldValue, float newValue)
        {
            if (statsCog.EvaluateCondition(expression))
            {
                onTrue?.Invoke();
            }
            else
            {
                onFalse?.Invoke();
            }
        }

        private void Subscribe()
        {
            StatValue stat;
            List<string> req = SubscriptionRequirements(expression);
            foreach (string statName in req)
            {
                stat = statsCog.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(Evaluate);
                    subs.Add(stat);
                }
            }
        }

        private List<string> SubscriptionRequirements(string equation)
        {
            List<string> res = new List<string>();
            if (!equation.IsNumeric())
            {
                equation = equation.Replace("(", "( ");
                equation = equation.Replace(")", " )");

                string[] parts = equation.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();

                    if (!parts[i].IsNumeric() && !LogicExtensions.BASIC_EXPRESSIONS.Contains(parts[i]))
                    {
                        res.Add(parts[i]);
                    }
                }
            }
            return res;
        }

        private void Unsubscribe()
        {
            foreach (StatValue stat in subs)
            {
                stat.onValueChanged.RemoveListener(Evaluate);
            }
        }

        #endregion

    }
}