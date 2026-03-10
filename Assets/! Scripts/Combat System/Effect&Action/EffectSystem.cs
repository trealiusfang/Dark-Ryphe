using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectSystem : MonoBehaviour
{
    private static ActionResponse actionResponse;
    private static Character target;
    /// <summary>
    /// Whole scene based system, operates with CombatDirector
    /// </summary>
    /// <param name="action"></param>
    public static void ApplyAction(Action action)
    {
        float value = action.value;
        target = action.target;
        actionResponse = ActionResponse.None;
        List<Effect> effects = action.caster.GetEffects();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.BeforeApply)
            {
                value = effects[i].calc(action,value, EffectedType.Dealer);
            }
        }

        if (value <= 0)
        {
            //your attack failed, too weak!
            return;
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.BeforeApply)
                {
                    value = effects[i].calc(action, value, EffectedType.Reciever);
                }
            }
        }

        if (value <= 0)
        {
            //your attack failed, too weak!
            return;
        }

       effects = action.caster.GetEffects();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.OnApply)
            {
                GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Dealer));
            }
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.OnApply)
                {
                    GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Reciever));
                }
            }
        }

        if (actionResponse == ActionResponse.Dodge)
        {
            //your attack failed!
            return;
        }
        if (actionResponse == ActionResponse.Miss)
        {
            //your attack failed!
            return;
        }

        effects = action.caster.GetEffects();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.AfterApply)
            {
                GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Dealer));
            }
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.AfterApply)
                {
                    GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Reciever));
                }
            }
        }

        if (target == null && action.actionType != ActionType.None)
        {
            //Target is hidden! Can't be attacked.
            return;
        }

        //Action applied
        GameInitializer.instance.StartCoroutine(action.Execute(action.caster, target, value));
    }

    public static void ApplyActionImmidiate(Action action)
    {
        float value = action.value;
        List<Effect> effects = action.caster.GetEffects();

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.BeforeApply)
            {
                value = effects[i].calc(action, value, EffectedType.Dealer);
            }
        }

        if (action.target != null && action.target != action.caster) 
        {
            effects = action.caster.GetEffects();
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.BeforeApply)
                {
                    value = effects[i].calc(action, value, EffectedType.Dealer);
                }
            }
        } 
        //No further responses
        GameInitializer.instance.StartCoroutine(action.Execute(action.caster, action.target, action.value));
    }

    public static void ApplyEffect(Effect effect)
    {
        effect.target.AddEffect(effect);
        GameInitializer.instance.StartCoroutine(effect.OnApply(effect.target, effect.value));
    }

    public static void SetActionResponse(ActionResponse response)
    {
        actionResponse = response;
    }

    public static void ChangeTarget(Character newTarget)
    {
        target = newTarget;
    }
}

public enum ActionResponse
{
    None,
    Dodge,
    Miss,
}