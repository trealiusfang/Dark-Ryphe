using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public Character caster;
    public Character target;
    public float value;

    public ActionType actionType;

    public Func<Character,Character, float, IEnumerator> ActionLogic;
    public virtual IEnumerator Execute(Character caster, Character target, float value)
    {
        if (ActionLogic != null)
            yield return ActionLogic(caster, target, value);
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
