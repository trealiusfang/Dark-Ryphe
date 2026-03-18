using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Unity.VisualScripting;
public static class EffectLibrary
{
    private static List<Type> effectTypes;

    static EffectLibrary()
    {
        effectTypes = Assembly.GetAssembly(typeof(Effect))
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Effect)) && !t.IsAbstract)
            .ToList();
    }

    public class Power : Effect
    {
        public Power()
        {
            responseType = EffectResponseType.BeforeApply;
            EffectName = "Power";
            effectType = EffectType.positive;
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
            effectType = EffectType.malicious;
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
            effectType = EffectType.positive;
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
            effectType = EffectType.positive;
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
                    yield return new WaitForSeconds(.25f);

                    EffectSystem.ApplyActionImmidiate(new ActionLibrary.DamageAction { caster = action.target, target = action.caster, value = action.target.baseStats.power});
                }
            }
        }
    }

    public class Venom : Effect
    {
        public Venom()
        {
            EffectName = "Venom";
            effectType = EffectType.malicious;

            ApplyEffects = OnApply;
            durationType = EffectDuration.Round;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+"+ value + " VENOM", textAnimType = TextAnimType.Venom });
            yield return null;
        }

        public override IEnumerator OnTurnStart(TurnStartEvent ev)
        {
            EffectSystem.ApplyActionImmidiate(new ActionLibrary.VenomDamage { target = ev.unit, value = value, caster = caster });

            yield return base.WaitTimer();
        }
    }

    public class BigAndSmall : Effect
    {
        public BigAndSmall()
        {
            responseType = EffectResponseType.BeforeApply;
            EffectName = "BigAndSmall";
            effectType = EffectType.positive;
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            if (value == 0) yield break;
            if (value > 0)
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+BIGGER", textAnimType = TextAnimType.pyro });
            if (value < 0)
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "-Smol", textAnimType = TextAnimType.Premium });
            float count = target.getEffect("BigAndSmall").value;
            target.transform.localScale = new Vector3(1 + (.1f * count),1 + (.1f * count));

            GameInitializer.instance._combatManagers.GetComponent<CombatPositioner>().ResetPosition(target);
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

    public class Anger : Effect
    {
        public Anger()
        {
            responseType = EffectResponseType.BeforeApply;
            EffectName = "Anger";
            effectType = EffectType.positive;
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            if (value == 0) yield break;

            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "ANGERED", textAnimType = TextAnimType.pyro });
            target.baseStats.manaRegen += 2;
            target.baseStats.speed += 2;

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
    public class Confused : Effect
    {
        public Confused()
        {
            responseType = EffectResponseType.OnApply;
            EffectName = "Confused";
            effectType = EffectType.malicious;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "Confused?", textAnimType = TextAnimType.Metallic });

            yield return null;
        }

        public override IEnumerator Execute(Action action, EffectedType type)
        {
            if (type == EffectedType.Dealer)
            {
                if (action.actionType == ActionType.DamagePhysical || action.actionType == ActionType.DamageMagic)
                {
                    int r = UnityEngine.Random.Range(0, 100);

                    if (r < 10 * value)
                    {
                        EffectSystem.ChangeTarget(null);
                    }
                }
            }

            yield return null;
        }
    }
    public class HealthRegen : Effect
    {
        public HealthRegen()
        {
            EffectName = "HealthRegen";
            effectType = EffectType.positive;

            durationType = EffectDuration.Round;
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            if (value == 0) yield break;

            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+" + value + " Health Regen", textAnimType = TextAnimType.Heal });
            yield return null;
        }
        public override IEnumerator OnTurnEnd(TurnEndEvent ev)
        {
            EffectSystem.ApplyAction(new ActionLibrary.Heal { caster = caster, target = target, value = value});

            yield return WaitTimer();
        }
    }

    public static Effect GetARandomEffect()
    {
        int r = UnityEngine.Random.Range(0, effectTypes.Count);
        return Activator.CreateInstance(effectTypes[r]) as Effect;
    }
}
