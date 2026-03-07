using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData charData;
    public CharacterTeam Team;
    [HideInInspector] public AbilityHolder abilityHolder;

    [HideInInspector] public CombatStats baseStats;
     public Stats currentStats;
    
    void Awake()
    {
        abilityHolder = GetComponent<AbilityHolder>();
        if (charData != null)
        {
            baseStats = charData.characterStats;
            GetComponent<SpriteRenderer>().sprite = charData.charSprite;
            transform.name = charData.characterName + " (Character)";
            abilityHolder.Abilities = charData.Abilities;
        }

        currentStats.currentHP = baseStats.maxHP;
        currentStats.currentMana = baseStats.maxMana;
    }

    public void TakeDamage(int dmg)
    {
        currentStats.currentHP -= dmg;
        if (currentStats.currentHP <= 0)
            Die();
    }

    void Die()
    {
        EventBus.Raise(new UnitDeathEvent
        {
            unit = this
        });

        Destroy(gameObject);
    }
}

