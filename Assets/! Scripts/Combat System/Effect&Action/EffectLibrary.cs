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

        public override float statCalc(statType statType, float _value)
        {
            if (statType == statType.Power)
            {
                return _value + value;
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

        public override float statCalc(statType statType, float _value)
        {
            if (statType == statType.Power)
            {
                return _value - value;
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
        public override float actionCalc(Action action, float _value, EffectedType type)
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

                    EffectSystem.ApplyActionImmidiate(new ActionLibrary.DamageAction { caster = action.target, target = action.caster, value = action.target.GetStat(statType.Power)});
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
    public class Bleed : Effect
    {
        public Bleed()
        {
            EffectName = "Bleed";
            effectType = EffectType.malicious;

            ApplyEffects = OnApply;
            durationType = EffectDuration.Round;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+"+ value + " BLEED", textAnimType = TextAnimType.Damage });
            yield return null;
        }

        public override IEnumerator OnTurnStart(TurnStartEvent ev)
        {
            EffectSystem.ApplyActionImmidiate(new ActionLibrary.DamageAction { target = ev.unit, value = value, caster = caster });

            yield return base.WaitTimer();
        }
    }

    public class DodgeInstinct : Effect
    {
        public DodgeInstinct()
        {
            responseType = EffectResponseType.OnApply;
            EffectName = "Dodge Instinct";
            effectType = EffectType.positive;

            ApplyEffects = OnApply;
            value = 1;
            durationType = EffectDuration.Infinite;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+" + value + " Dodge", textAnimType = TextAnimType.Metallic });
            yield return null;
        }

        public override IEnumerator Execute(Action action, EffectedType type)
        {
            Debug.Log("dod called");
            if (value <= 0)
            {
                action.target.effectHolder.RemoveEffect(this);
                Debug.Log("Removed");
                yield break;
            }
            if (type == EffectedType.Reciever)
            {
                if (action.actionType == ActionType.DamagePhysical || action.actionType == ActionType.DamageMagic)
                {
                    Debug.Log("dodge");
                    EffectSystem.SetActionResponse(ActionResponse.Dodge);
                    value -= 1;
                }
            }

            yield return null;
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
            float count = target.effectHolder.getEffect("BigAndSmall").value;
            target.transform.localScale = new Vector3(1 + (.1f * count),1 + (.1f * count));

            GameInitializer.instance._combatManagers.GetComponent<CombatPositioner>().ResetPosition(target);
            yield return null;
        }

        public override float actionCalc(Action action, float _value, EffectedType type)
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
            yield return null;
        }
        public override float actionCalc(Action action, float _value, EffectedType type)
        {
            if (type == EffectedType.Dealer)
            {
                if (action.actionType == ActionType.DamagePhysical)
                {
                    return _value + value;
                }
            }

            if (type == EffectedType.Dealer)
            {
                if (action.actionType == ActionType.StatIncreaseManaRegen)
                {
                    return _value + value * 2;
                }
            }

            return _value;
        }

        public override float statCalc(statType statType, float _value)
        {
            if (statType == statType.Speed)
            {
                return _value + value * 2;
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

    public class SpeedBoost : Effect
    {
        public SpeedBoost() 
        {
            EffectName = "Speed Boost";
            effectType = EffectType.positive;

            durationType = EffectDuration.Combat;
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            if (value == 0) yield break;

            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+" + value + " Speed", textAnimType = TextAnimType.Shock });
            yield return null;
        }

        public override float statCalc(statType statType, float _value)
        {
            if (statType == statType.Speed)
            {
                return _value + value;
            }

            return _value;
        }
    }

    public class Mark : Effect
    {
        public Mark()
        {
            EffectName = "Mark";
            effectType = EffectType.malicious;

            durationType = EffectDuration.Round;
            ApplyEffects = OnApply;
        }
        public override IEnumerator OnApply(Character target, float value)
        {
            if (value == 0) yield break;

            EventBus.Raise(new BattleTextEvent { position = target.transform.position, text = "+"  + "Marked!", textAnimType = TextAnimType.Premium });
            yield return null;
        }
    }

    public static Effect GetARandomEffect()
    {
        int r = UnityEngine.Random.Range(0, effectTypes.Count);
        return Activator.CreateInstance(effectTypes[r]) as Effect;
    }
}
