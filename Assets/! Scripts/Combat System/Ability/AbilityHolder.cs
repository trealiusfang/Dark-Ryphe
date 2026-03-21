using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : BusRoute
{
    private List<Ability> allAbilities = new List<Ability>();
    private List<Ability> activeAbilities = new List<Ability>();

    public  List<AbilitySO> abilityStrings = new List<AbilitySO>();
    List<AbilityCooldownHandling> cooldownHandlings = new List<AbilityCooldownHandling>();

    private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
        if (character == null) { Debug.LogError($"CHARACTER IS NULL! \"{transform.name}\""); }

        abilityStrings = character.charData.Abilities;
        for (int i = 0; i < abilityStrings.Count; i++)
        {
            allAbilities.Add(AbilityLibrary.StringToAbility(abilityStrings[i].name));
            allAbilities[i].SetAbility(abilityStrings[i]);
        }

        Sub<TurnEndEvent>(LowerCooldown);
        Sub<CombatEndEvent>(LowerCooldown);

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
        if (!ability.abilityCastable(character))
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
        List<AbilityCooldownHandling> removedHandlings = new List<AbilityCooldownHandling>();

        foreach (AbilityCooldownHandling cooldownHandling in cooldownHandlings)
        {
            if (cooldownHandling.cooldownType == CooldownType.Combat)
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
