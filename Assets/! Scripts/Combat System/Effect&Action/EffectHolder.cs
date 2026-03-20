using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHolder : BusRoute
{
    private List<Effect> effects = new List<Effect>();

    private void Awake()
    {
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

    public Effect getEffect(string effectName)
    {
        foreach (Effect effect in effects)
        {
            if (effect.EffectName == effectName)
            {
                return (Effect)effect;
            }
        }

        return null;
    }
    public List<Effect> GetEffects()
    {
        return effects;
    }
}
