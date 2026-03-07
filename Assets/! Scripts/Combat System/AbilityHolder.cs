using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    private List<Ability> allAbilities = new List<Ability>();
    private List<Ability> activeAbilities = new List<Ability>();

    public  List<AbilitySO> Abilities = new List<AbilitySO>();
    private Character character;
    private void Start()
    {
        character = GetComponent<Character>();
        for (int i = 0; i < Abilities.Count; i++)
        {
            allAbilities.Add(AbilityLibrary.stringToAbility(Abilities[i].name));
            allAbilities[i].sprite = Abilities[i].abilitySprite;
        }

        activeAbilities = allAbilities;
    }

    public bool abilityAvailable(Ability ability)
    {
        if (ability.manaCost > character.currentStats.currentMana)
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
        return true;
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
