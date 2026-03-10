using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string abilityName;

    //Set in abilitySO
    public Sprite sprite = null;
    public AudioClip abilitySuccessClip = null;
    //Conditions
    public short manaCost = 4;
    public TargetType targetType;
    public CooldownType cooldownType;
    public short cooldownTime = 0;

    public short[] activasionSpots = {1,1,1,1};
    public short[] targetSpots = {1,1,1,1};
    public int abilityValue = 1;

    public Func<Character, List<Character>, Ability, IEnumerator> AbilityLogic;
    public virtual IEnumerator Execute(Character caster, List<Character> targets, Ability ability = null)
    {
        yield return PreExecute(caster, targets);

        if (AbilityLogic != null)
            yield return AbilityLogic(caster, targets, ability);

        yield return PostExecute(caster, targets);
    }

    protected virtual IEnumerator PreExecute(Character caster, List<Character> targets)
    {
        EventBus.Raise(new AbilityUsedEvent { caster = caster, ability = this, targets = targets });
        caster.currentStats.currentMana -= manaCost;
        yield return new WaitForSeconds(.10f);
        yield break;
    }

    protected virtual IEnumerator PostExecute(Character caster, List<Character> targets)
    {
        yield return new WaitForSeconds(.30f);
        EventBus.Raise(new AbilityFinishedEvent { caster = caster, ability = this });
        yield break;
    }


    //If an ability requires certain conditions other than base conditions, they can be overriden here.
    public virtual bool abilityCastable(Character caster)
    {
        return true;
    }
    public virtual bool unitTargetable(Character target)
    {
        return true;
    }
}

public enum CooldownType
{
    None,
    Round,
    Combat
}

public enum TargetType
{
    Self,
    SingleEnemy,
    AoEEnemy,
    SingleAlly,
    AoEAlly
}