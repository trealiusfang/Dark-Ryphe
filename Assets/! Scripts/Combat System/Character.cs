using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using System.Collections;
using UnityEngine;

public class Character : BusRoute, IInspectable
{
    public CharacterData charData;
    public CharacterTeam Team;
    [HideInInspector] public AbilityHolder abilityHolder;
    [HideInInspector] public CombatStats baseStats;
    public Stats currentStats;
    private List<Effect> effects = new List<Effect>();
    bool dead = false;
    private Material material;
    public Sprite DeathSprite;

    void Awake()
    {
        abilityHolder = GetComponent<AbilityHolder>();
        material = GetComponent<SpriteRenderer>().material;

        if (charData != null)
        {
            baseStats = charData.characterStats;
            GetComponent<SpriteRenderer>().sprite = charData.charSprite;
            transform.name = charData.characterName + " (Character)";
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
                return;
            }
        }

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
        dead = true;
        StartCoroutine(dieFunc());
    }

    private IEnumerator dieFunc()
    {
        yield return null;

        GetComponent<SpriteRenderer>().sprite = DeathSprite;
        EventBus.Raise(new SFXEvent { sfx_string = "unit death"});
        float timeElapsed = 0;
        while (material.GetFloat("_Fade") > 0)
        {
            material.SetFloat("_Fade", 1 - timeElapsed);
            timeElapsed += .05f;
            yield return new WaitForSeconds(.05f);
        }
        EventBus.Raise(new UnitDeathEvent
        {
            unit = this
        });

        yield return new WaitForSeconds(.05f);

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

    public Effect getEffect(string effectName)
    {
        foreach (Effect effect in effects)
        {
            if (effect.EffectName == effectName)
            {
                return (Effect) effect;
            }
        }

        return null;
    }

    public void OnInspect()
    {
        EventBus.Raise(new InspectedCharacterEvent { character = this });
    }
}

