using System;
using System.Collections.Generic;

namespace NullSave.TOCK.Stats
{
    public class ExpressionSubscription
    {

        #region Properties

        public string Expression { get; private set; }
        public Action<bool> ExpressionUpdate { get; private set; }
        private StatsCog StatsCog { get; set; }
        private List<StatValue> Subscriptions { get; set; }

        #endregion

        #region Constructors

        public ExpressionSubscription(string expression, Action<bool> callback)
        {
            Expression = expression;
            ExpressionUpdate = callback;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds subscription and immediately evaluates for inital value
        /// </summary>
        /// <param name="statsCog"></param>
        public void Subscribe(StatsCog statsCog)
        {
            Unsubscribe();

            StatsCog = statsCog;

            // Increment
            StatValue stat;
            Subscriptions = new List<StatValue>();
            List<string> req = StatsCog.GetSubscriptionRequirements(Expression);
            foreach (string statName in req)
            {
                stat = StatsCog.FindStat(statName);
                if (stat != null && !Subscriptions.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateResult);
                    Subscriptions.Add(stat);
                }
            }

            UpdateResult(0, 0);
        }

        public void Unsubscribe()
        {
            if (StatsCog == null || Subscriptions == null) return;

            foreach (StatValue stat in Subscriptions)
            {
                stat.onValueChanged.RemoveListener(UpdateResult);
            }
            Subscriptions = null;
        }

        #endregion

        #region Private Methods

        private void UpdateResult(float oldValue, float newValue)
        {
            ExpressionUpdate?.Invoke(StatsCog.EvaluateCondition(Expression));
        }

        #endregion

    }
}