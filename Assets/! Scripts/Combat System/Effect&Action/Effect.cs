using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect 
{
    public string EffectName = "";
    public Character caster;
    public Character target;

    public float value;
    public EffectResponseType responseType;

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
    public virtual float calc(Action action,float _value, EffectedType type)
    {
        return value;
    }


    public virtual void OnTurnStart(TurnStartEvent ev)
    {

    }

    public virtual void OnTurnEnd(TurnEndEvent ev)
    {

    }
}

public enum EffectedType
{
    Reciever,
    Dealer
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