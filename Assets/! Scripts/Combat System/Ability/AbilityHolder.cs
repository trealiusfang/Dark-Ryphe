using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    private List<Ability> allAbilities = new List<Ability>();
    private List<Ability> activeAbilities = new List<Ability>();

    public  List<AbilitySO> Abilities = new List<AbilitySO>();
    List<AbilityCooldownHandling> cooldownHandlings = new List<AbilityCooldownHandling>();

    private Character character;
    private void Start()
    {
        character = GetComponent<Character>();
        for (int i = 0; i < Abilities.Count; i++)
        {
            allAbilities.Add(AbilityLibrary.stringToAbility(Abilities[i].name));
            allAbilities[i].sprite = Abilities[i].abilitySprite;
        }

        EventBus.Sub<TurnEndEvent>(LowerCooldown);
        EventBus.Sub<CombatEndEvent>(LowerCooldown);

        activeAbilities = allAbilities;
    }

    public bool abilityAvailable(Ability ability)
    {
        if (ability.manaCost > character.currentStats.currentMana && ability.manaCost > 0)
        {
            return false;
        }
        //If there are no targets
        List<Character> targets = TargetSetter.SetTarget(character, ability);
        if (targets.Count == 0)
        {
            return false;
        }
        //Ability specific condition
        if (!ability.abilityCastable())
        {
            return false;
        }

        foreach (AbilityCooldownHandling cooldownHandling in cooldownHandlings)
        {
            if (ability == cooldownHandling.ability)
            {
                if (cooldownHandling.cooldownTime > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void NotifyCooldownChecks(Ability ability)
    {
        if (ability.cooldownType != CooldownType.None)
        {
            AbilityCooldownHandling handling = new AbilityCooldownHandling();
            handling.ability = ability;
            handling.cooldownType = ability.cooldownType;
            handling.cooldownTime = ability.cooldownTime;

            cooldownHandlings.Add(handling);
        }
    }

    private void LowerCooldown(TurnEndEvent ev)
    {
        List<AbilityCooldownHandling> removedHandlings = new List<AbilityCooldownHandling>();
        if (ev.unit == character)
        {
            foreach (AbilityCooldownHandling cooldownHandling in cooldownHandlings)
            {
                if (cooldownHandling.cooldownType == CooldownType.Round)
                {
                    if (cooldownHandling.cooldownTime > 0)
                    {
                        cooldownHandling.cooldownTime--;

                        if (cooldownHandling.cooldownTime == 0)
                        {
                            removedHandlings.Add(cooldownHandling);
                        }
                    }
                }
            }

            foreach (AbilityCooldownHandling removedHandling in removedHandlings)
            {
                cooldownHandlings.Remove(removedHandling);
            }
        }
    }
    private void LowerCooldown(CombatEndEvent ev)
    {
        foreach (AbilityCooldownHandling cooldownHandling in cooldownHandlings)
        {
            if (cooldownHandling.cooldownType == CooldownType.Match)
            {
                if (cooldownHandling.cooldownTime > 0)
                    cooldownHandling.cooldownTime--;
            }
        }
    }

    public void AddAbility(Ability newAbility)
    {
        allAbilities.Add((Ability)newAbility);
    }

    public void RemoveAbility(Ability newAbility)
    {
        allAbilities.Remove((Ability)newAbility);
    }

    public void RemoveAbilityAt(int i)
    {
        allAbilities.RemoveAt(i);
    }

    public List<Ability> GetAllAbilities()
    {
        return allAbilities ?? new List<Ability>();
    }

    public List<Ability> GetActiveAbilities()
    {
        return activeAbilities ?? new List<Ability>();
    }
}

[Serializable]
public class AbilityCooldownHandling
{
    public Ability ability;
    public short cooldownTime = 0;
    public CooldownType cooldownType = CooldownType.None;
}
