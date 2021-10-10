using System;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [Serializable]
    public class DamageDealt : UnityEvent<float, DamageReceiver> { }

    [Serializable]
    public class DamageTaken : UnityEvent<float, DamageDealer, GameObject> { }

    [Serializable]
    public class DamageRecieved : UnityEvent<DamageDealer, GameObject> { }

    [Serializable]
    public class EffectAdded : UnityEvent<StatEffect> { }

    [Serializable]
    public class EffectEnded : UnityEvent<StatEffect> { }

    [Serializable]
    public class EffectRemoved : UnityEvent<StatEffect> { }

    [Serializable]
    public class EffectResisted : UnityEvent<StatEffect> { }

    [Serializable]
    public class Impacted : UnityEvent<HitDirection> { }

    [Serializable]
    public class ValueChanged : UnityEvent<float, float> { }

}