using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class Character : BusRoute
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
        //make a different class for effects
        Sub<TurnEndEvent>(LowerEffectCooldown);
        Sub<CombatEndEvent>(LowerEffectCooldown);
    }

    public void AddEffect(Effect effect)
    {
        foreach (Effect charEffect in effects)
        {
            if (charEffect.EffectName == effect.EffectName)
            {
                charEffect.value += effect.value;
                Debug.Log("Effect value increase");
                return;
            }
        }
        SubnApply<TurnStartEvent>(effect.OnTurnStart);
        SubnApply<TurnEndEvent>(effect.OnTurnEnd);

        effects.Add(effect);
    }

    private void LowerEffectCooldown(TurnEndEvent ev)
    {
        if (ev.unit == this)
        {
            List<Effect> removals = new List<Effect>();
            foreach (Effect charEffect in effects)
            {
                if (charEffect.durationType == EffectDuration.Round)
                {
                    charEffect.duration--;

                    if (charEffect.duration <= 0)
                    {
                        removals.Add(charEffect);
                    }
                }
            }

            foreach (Effect effect in removals)
            {
                effects.Add(effect);
            }
        }
    }
    private void LowerEffectCooldown(CombatEndEvent ev)
    {
        List<Effect> removals = new List<Effect>();
        foreach (Effect charEffect in effects)
        {
            if (charEffect.durationType == EffectDuration.Combat)
            {
                charEffect.duration--;

                if (charEffect.duration <= 0)
                {
                    removals.Add(charEffect);
                }
            }
        }

        foreach (Effect effect in removals)
        {
            effects.Add(effect);
        }
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

