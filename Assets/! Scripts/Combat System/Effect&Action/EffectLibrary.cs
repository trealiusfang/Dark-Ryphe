using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectLibrary 
{
    public class Power : Effect
    {
        public Power()
        {
            responseType = EffectResponseType.BeforeApply;
            EffectName = "Power";
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+ " + value + " Power", textAnimType = TextAnimType.pyro });
            yield return null;
        }

        public override float calc(Action action, float _value, EffectedType type)
        {
            if (type == EffectedType.Dealer)
            {
                if (action.actionType == ActionType.DamagePhysical)
                {
                    return _value + value;
                }
            }

            return _value;
        }
    }

    public class Weakness : Effect
    {
        public Weakness()
        {
            responseType = EffectResponseType.BeforeApply;
            EffectName = "Weakness";
            ApplyEffects = OnApply;
        }

        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+ " + value + " Weakness", textAnimType = TextAnimType.Spooky});
            yield return null;
        }
        public override float calc(Action action, float _value, EffectedType type)
        {
            if (type == EffectedType.Dealer)
            {
                if (action.actionType == ActionType.DamagePhysical)
                {
                    return _value - value;
                }
            }

            return _value;
        }
    }

    public class Brace : Effect
    {
        public Brace() 
        {
            responseType = EffectResponseType.BeforeApply;
            EffectName = "Brace";
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+ " + value + " Brace", textAnimType = TextAnimType.Metallic });
            yield return null;
        }
        public override float calc(Action action, float _value, EffectedType type)
        {
            if (type == EffectedType.Reciever)
            {
                if (action.actionType == ActionType.DamagePhysical)
                {
                    return _value - value;
                }
            }

            return _value;
        }
    }

    public class Riposte : Effect
    {
        public Riposte()
        {
            responseType = EffectResponseType.AfterApply;
            EffectName = "Brace";
            ApplyEffects = OnApply;
            EffectLogic = Execute;
            duration = 3;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "RIPOSTE", textAnimType = TextAnimType.Spooky });
            yield return null;
        }

        public override IEnumerator Execute(Action action, EffectedType type)
        {
            if (type == EffectedType.Reciever)
            {
                if (action.actionType == ActionType.DamagePhysical || action.actionType == ActionType.DamageMagic)
                {
                    yield return new WaitForSeconds(.35f);

                    EffectSystem.ApplyActionImmidiate(new ActionLibrary.CritAction { caster = action.target, target = action.caster, value = action.target.baseStats.power});
                }
            }
        }
    }
}
