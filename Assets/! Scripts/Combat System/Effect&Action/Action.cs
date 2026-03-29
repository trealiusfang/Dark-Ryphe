using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public Character caster;
    public Character target;
    public float value;
    public bool isCrit;

    public AudioClip hitSound;
    public AudioClip critSound;

    public ActionType actionType;

    public Func<Character,Character, float, bool, IEnumerator> actionLogic;
    public virtual IEnumerator Execute(Character caster, Character target, float value, bool isCrit)
    {
        if (actionLogic != null)
            yield return actionLogic(caster, target, value, isCrit);
    }

    public virtual IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
    {
        if (actionLogic != null)
            yield return actionLogic(caster, target, value, isCrit);
    }
}

public enum ActionType
{
    None,
    DamagePhysical,
    DamageMagic,
    Heal,
    Buff,
    Debuff,
    StatusApplier,
    StatIncreaseHealth,
    StatIncreaseMana,
    StatIncreaseSpeed,
    StatIncreasePower,
    StatIncreaseLuck,
    StatIncreaseManaRegen,
    StatIncreaseAll,
    StatIncrease,
}
