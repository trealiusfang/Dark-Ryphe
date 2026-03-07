using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string abilityName;

    //Set in abilitySO
    public Sprite sprite = null;

    //Conditions
    public short manaCost = 4;
    public TargetType targetType;
    public CooldownType cooldownType;
    public short cooldownTime = 0;

    public short[] activasionSpots = {1,1,1,1};
    public short[] targetSpots = {1,1,1,1};

    public Func<Character, List<Character>, IEnumerator> AbilityLogic;
    public virtual IEnumerator Execute(Character caster, List<Character> targets)
    {
        yield return PreExecute(caster, targets);

        if (AbilityLogic != null)
            yield return AbilityLogic(caster, targets);

        yield return PostExecute(caster, targets);
    }

    protected virtual IEnumerator PreExecute(Character caster, List<Character> targets)
    {
        EventBus.Raise(new AbilityUsedEvent { caster = caster, ability = this });
        caster.currentStats.currentMana -= manaCost;
        yield return new WaitForSeconds(.15f);
        yield break;
    }

    protected virtual IEnumerator PostExecute(Character caster, List<Character> targets)
    {
        yield return new WaitForSeconds(.15f);
        EventBus.Raise(new AbilityFinishedEvent { caster = caster, ability = this });
        yield break;
    }


    //If an ability requires certain conditions other than base conditions, they can be overriden here.
    public virtual bool abilityCastable()
    {
        return true;
    }
}

public enum CooldownType
{
    None,
    Round,
    Match
}

public enum TargetType
{
    Self,
    SingleEnemy,
    AoEEnemy,
    SingleAlly,
    AoEAlly
}