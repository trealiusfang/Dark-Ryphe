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
    private List<Effect> effects = new List<Effect>();
    bool dead = false;

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

    public void AddEffect(Effect effect)
    {
        foreach (Effect charEffect in effects)
        {
            if (charEffect.EffectName == effect.EffectName)
            {
                charEffect.value += effect.value;
                Debug.Log("Here it is +" + charEffect.value);
                return;
            }
        }

        EventBus.Sub<TurnStartEvent>(effect.OnTurnStart);
        EventBus.Sub<TurnEndEvent>(effect.OnTurnEnd);

        effects.Add(effect);
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

        dead = true;
        Destroy(gameObject);
    }

    public bool isDead()
    {
        return dead;
    }

    public List<Ability> getActiveAbilities()
    {
        return abilityHolder.GetActiveAbilities();
    }
    public List<Ability> getAllAbilities()
    {
        return abilityHolder.GetAllAbilities();
    }

    public List<Effect> GetEffects()
    {
        return effects;
    }
}

