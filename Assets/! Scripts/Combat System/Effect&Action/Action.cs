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

    public ActionType actionType;

    public Func<Character,Character, float, bool, IEnumerator> ActionLogic;
    public virtual IEnumerator Execute(Character caster, Character target, float value, bool isCrit)
    {
        if (ActionLogic != null)
            yield return ActionLogic(caster, target, value, isCrit);
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
