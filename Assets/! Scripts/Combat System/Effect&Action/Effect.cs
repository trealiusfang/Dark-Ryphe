using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect 
{
    public string EffectName = "";
    public EffectType effectType;
    public Character caster;
    public Character target;

    public float value;
    public EffectResponseType responseType;
    public int priority = 0; // value between 0-100

    public int duration = 1;
    public EffectDuration durationType;

    public Func<Action, EffectedType,IEnumerator> EffectLogic;
    public Func<Character, float,IEnumerator> ApplyEffects;

    public virtual IEnumerator Execute(Action action, EffectedType type)
    {
        if (EffectLogic != null)
            yield return EffectLogic(action, type);
    }

    public virtual IEnumerator OnApply(Character target, float value)
    {
        if (ApplyEffects != null)
            yield return ApplyEffects(target, value);
    }
    public virtual float actionCalc(Action action,float _value, EffectedType type)
    {
        return _value;
    }

    public virtual float statCalc(statType statType,float _value)
    {
        return _value;
    }

    public virtual IEnumerator OnTurnStart(TurnStartEvent ev)
    {
        yield break;
    }
    public virtual IEnumerator OnTurnEnd(TurnEndEvent ev)
    {
        yield break;
    }
    public virtual IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(.3f);
        yield break;
    }
}

public enum EffectedType
{
    Reciever,
    Dealer,
}

public enum EffectResponseType
{
    None,
    BeforeApply,
    OnApply,
    AfterApply
}
public enum EffectDuration
{
    Infinite,
    Round,
    Combat
}

public enum EffectType
{
    none,
    positive,
    malicious,
    both,
}