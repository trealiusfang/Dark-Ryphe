using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
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
        List<Effect> effects = action.caster.effectHolder.GetEffects();
        List<Passive> passives = action.caster.passiveHolder.GetPassives();

        bool isCrit = false;
        int luckAmount = action.caster.GetStatFloor(statType.Luck);

        int r = UnityEngine.Random.Range(15, 100);

        if (r < luckAmount * 8) isCrit = true; 

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.BeforeApply)
            {
                value = effects[i].actionCalc(action,value, EffectedType.Dealer);
            }
        }

        for (int i = 0; i < passives.Count; i++)
        {
            value = passives[i].actionCalc(action, value, EffectedType.Dealer);
        }

        if (value <= 0)
        {
            //your attack failed, too weak!
            return;
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.effectHolder.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.BeforeApply)
                {
                    value = effects[i].actionCalc(action, value, EffectedType.Reciever);
                }
            }

            passives = action.target.passiveHolder.GetPassives();

            for (int i = 0; i < passives.Count; i++)
            {
                value = passives[i].actionCalc(action, value, EffectedType.Dealer);
            }
        }

        if (value <= 0)
        {
            //your attack failed, too weak!
            return;
        }

        effects = action.caster.effectHolder.GetEffects();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.OnApply)
            {
                GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Dealer));
            }
        }
        passives = action.caster.passiveHolder.GetPassives();

        for (int i = 0; i < passives.Count; i++)
        {
            GameInitializer.instance.StartCoroutine(passives[i].Execute(action, EffectedType.Dealer));
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.effectHolder.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.OnApply)
                {
                    GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Reciever));
                }
            }

            passives = action.target.passiveHolder.GetPassives();

            for (int i = 0; i < passives.Count; i++)
            {
                GameInitializer.instance.StartCoroutine(passives[i].Execute(action, EffectedType.Reciever));
            }
        }

        if (actionResponse == ActionResponse.Dodge)
        {
            //your attack failed!
            GameInitializer.instance.StartCoroutine(new ActionLibrary.Dodge { }.Execute(action.caster, action.target, action.value, isCrit));
            EventBus.Raise(new ActionHappenedEvent { action = new ActionLibrary.Dodge { caster = action.caster, target = action.target } });
            return;
        }
        if (actionResponse == ActionResponse.Miss)
        {
            //your attack failed!
            GameInitializer.instance.StartCoroutine(new ActionLibrary.Miss { }.Execute(action.caster, action.target, action.value, isCrit));
            EventBus.Raise(new ActionHappenedEvent { action = new ActionLibrary.Miss { caster = action.caster, target = action.target } });
            return;
        }

        effects = action.caster.effectHolder.GetEffects();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.AfterApply)
            {
                GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Dealer));
            }
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.effectHolder.GetEffects();

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
        GameInitializer.instance.StartCoroutine(action.Execute(action.caster, target, value, isCrit));
        EventBus.Raise(new ActionHappenedEvent { action = action });
    }
    public static void ApplyActionImmidiate(Action action)
    {
        float value = action.value;
        List<Effect> effects = action.caster.effectHolder.GetEffects();
        List<Passive> passives = action.caster.passiveHolder.GetPassives();

        bool isCrit = false;

        int luckAmount = action.caster.GetStatFloor(statType.Luck);

        int r = UnityEngine.Random.Range(15, 100);

        if (r < luckAmount * 8) isCrit = true;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.BeforeApply)
            {
                value = effects[i].actionCalc(action, value, EffectedType.Dealer);
            }
        }

        for (int i = 0; i < passives.Count; i++)
        {
            value = passives[i].actionCalc(action, value, EffectedType.Dealer);
        }

        if (value <= 0)
        {
            //your attack failed, too weak!
            return;
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.effectHolder.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.BeforeApply)
                {
                    value = effects[i].actionCalc(action, value, EffectedType.Reciever);
                }
            }

            passives = action.target.passiveHolder.GetPassives();

            for (int i = 0; i < passives.Count; i++)
            {
                value = passives[i].actionCalc(action, value, EffectedType.Dealer);
            }
        }

        effects = action.caster.effectHolder.GetEffects();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.OnApply)
            {
                GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Dealer));
            }
        }

        passives = action.caster.passiveHolder.GetPassives();

        for (int i = 0; i < passives.Count; i++)
        {
            GameInitializer.instance.StartCoroutine(passives[i].Execute(action, EffectedType.Dealer));
        }

        if (action.caster != action.target && target != null)
        {
            effects = action.target.effectHolder.GetEffects();

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].responseType == EffectResponseType.OnApply)
                {
                    GameInitializer.instance.StartCoroutine(effects[i].Execute(action, EffectedType.Reciever));
                }
            }

            passives = action.target.passiveHolder.GetPassives();

            for (int i = 0; i < passives.Count; i++)
            {
                GameInitializer.instance.StartCoroutine(passives[i].Execute(action, EffectedType.Reciever));
            }
        }

        if (actionResponse == ActionResponse.Dodge)
        {
            //your attack failed!
            GameInitializer.instance.StartCoroutine(new ActionLibrary.Dodge { }.Execute(action.caster, action.target, action.value, isCrit));
            EventBus.Raise(new ActionHappenedEvent { action = new ActionLibrary.Dodge { caster = action.caster, target = action.target } });
            return;
        }
        if (actionResponse == ActionResponse.Miss)
        {
            //your attack failed!
            GameInitializer.instance.StartCoroutine(new ActionLibrary.Miss { }.Execute(action.caster, action.target, action.value, isCrit));
            EventBus.Raise(new ActionHappenedEvent { action = new ActionLibrary.Miss { caster = action.caster, target = action.target } });
            return;
        }
        //No further responses
        GameInitializer.instance.StartCoroutine(action.Execute(action.caster, action.target, action.value, isCrit));
    }

    public static void ApplyEffect(Effect effect)
    {
        Debug.Log(effect.EffectName + " has been applied to: " + effect.target);
        effect.target.effectHolder.AddEffect(effect);
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

    public static int GetEffectCalculation(float baseValue, Character character, Action action)
    {
        List<Effect> effects = character.effectHolder.GetEffects();

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].responseType == EffectResponseType.BeforeApply)
            {
                baseValue = effects[i].actionCalc(action, baseValue, EffectedType.Dealer);
            }
        }

        return Math.RoundValue(baseValue);
    }
}

public enum ActionResponse
{
    None,
    Dodge,
    Miss,
}