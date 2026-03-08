using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectContext 
{
    public static EffectContext Current;

    public List<IPassive> Passives = new();

    public void Notify(EffectType eventType, Effect effect)
    {
        foreach (var passive in Passives)
        {
            passive.OnEvent(eventType, effect);
        }
    }
}

public enum EffectType
{
    DamageDealt,
}
