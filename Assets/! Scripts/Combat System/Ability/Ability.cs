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
    public AudioClip abilityAlternativeClip = null;
    public AudioClip abilityCritClip = null;
    public AnimationClip abilityEffectClip; //If an ability cast is a ranged spell for example, the spell animation object that spawns uses this animation.
    public void SetAbility(AbilitySO abilitySO)
    {
        sprite = abilitySO.abilitySprite;
        abilitySuccessClip = abilitySO.abilitySuccessClip;
        abilityAlternativeClip = abilitySO.abilityAlternativeClip;
        abilityCritClip = abilitySO.abilityCritClip;
        abilityEffectClip = abilitySO.abilityEffectClip;

        virtualmanaCost = manaCost;
        virtualCostType = costType;
    }

    //Conditions
    public short manaCost = 4;
    public short virtualmanaCost = 4;
    public TargetType targetType;
    public CooldownType cooldownType;
    public AbilityFireType fireType;
    public AbilityCostType costType;
    public AbilityCostType virtualCostType;
    public short cooldownTime = 0; //Cooldown time is lowered by CharacterScript, if you want a custom behaviour, free feel to add the condition on CharacterScript and apply the behaviour on the ability.

    public short[] activasionSpots = {1,1,1,1};
    public short[] targetSpots = {1,1,1,1};
    public int abilityValue = 1;

    public Func<Character, List<Character>, IEnumerator> _abilityLogic;
    public virtual IEnumerator Execute(Character caster, List<Character> targets, Ability ability = null)
    {
        yield return PreExecute(caster, targets);

        yield return AbilityLogic(caster, targets);

        yield return PostExecute(caster, targets);
    }

    public virtual IEnumerator AbilityLogic(Character caster, List<Character> targets)
    {
        if (_abilityLogic != null)
            yield return _abilityLogic(caster, targets);
    }

    protected virtual IEnumerator PreExecute(Character caster, List<Character> targets)
    {
        EventBus.Raise(new AbilityUsedEvent { caster = caster, ability = this, targets = targets });
        if (costType == AbilityCostType.Mana) caster.ChangeStat(statType.Mana_Current, -manaCost);
        if (costType == AbilityCostType.HP) caster.ChangeStat(statType.HP_Current, -manaCost);

        yield return new WaitForSeconds(.50f);
        yield break;
    }

    protected virtual IEnumerator PostExecute(Character caster, List<Character> targets)
    {
        yield return new WaitForSeconds(.30f);
        EventBus.Raise(new AbilityFinishedEvent { caster = caster, ability = this });
        yield break;
    }

    //Animatons!!
    //Set character animation
    public virtual void PlayCharacterAnimation(Character caster, int index = 0)
    {
        caster.renderer.PlayActionAnimation(CharAnimationType.Attack, index);
    }
    //For distant objects
    public virtual void PlayEffectAnimation(Character target, Vector3 offset, float size = 1)
    {
        if (target == null || abilityEffectClip == null)
        {
            return;
        } else
        {
            Vector3 spawnPos = new Vector3(target.transform.position.x, 0, target.transform.position.z) + offset;
            
            EventBus.Raise(new BattleEffectEvent { position = spawnPos, effectAnimation = abilityEffectClip, effectSize = size});
        }
    }

    public virtual string GetAbilityDescription(Character caster)
    {
        return "";
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

    //The reason we have virtual values of cost and cost type is because we don't want to change their original values, they might change from passives quite a lot
    public int GetVirtualCost()
    {
        return virtualmanaCost;
    }

    public void SetVirtualCost(short changeAmount)
    {
        virtualmanaCost = changeAmount;

        if (virtualmanaCost <= 0 && manaCost != 0) virtualmanaCost = 1;
        else if (virtualmanaCost <= 0) virtualmanaCost = 0;
    }

    public void ChangeVirtualCostType(AbilityCostType costType)
    {
        virtualCostType = costType;
    }
}


public enum AbilityFireType
{
    BySelector,
    Instant
}

public enum AbilityCostType
{
    Mana,
    HP,
}

public enum CooldownType
{
    None,
    Round,
    Combat,
    Custom
}

public enum TargetType
{
    Self,
    SingleEnemy,
    AoEEnemy,
    SingleAlly,
    AoEAlly,
    SingleAll,
    AoEAll
}