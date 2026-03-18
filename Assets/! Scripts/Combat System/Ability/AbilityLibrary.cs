using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class AbilityLibrary
{
    public class NullAbility : Ability
    {
        public NullAbility()
        {
            abilityName = "Nulll";
            manaCost = 0;

            fireType = AbilityFireType.Instant;
            cooldownType = CooldownType.Round;
            cooldownTime = 1;

            AbilityLogic = EndTurnLogic;
        }
        public static IEnumerator EndTurnLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Error Alarm"});
            yield return new WaitForSeconds(0.5f);
            EventBus.Raise(new TurnEndEvent { unit = caster });
        }
    }
    public class EndTurn : Ability
    {
        public EndTurn()
        {
            abilityName = "End Turn";
            manaCost = 0;

            fireType = AbilityFireType.Instant;
            cooldownType = CooldownType.Round;
            cooldownTime = 1;

            AbilityLogic = EndTurnLogic;
        }
        protected override IEnumerator PostExecute(Character caster, List<Character> targets)
        {
            yield return null;
        }
        public static IEnumerator EndTurnLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = caster, value = caster.baseStats.manaRegen, target = caster});
            yield return new WaitForSeconds(0.3f);
            EventBus.Raise(new TurnEndEvent { unit = caster });
        }
    }
    public class WickedSlash : Ability
    {
        public WickedSlash()
        {
            abilityName = "Wicked Slash";
            manaCost = 4;
            targetType = TargetType.SingleEnemy;
            targetSpots = new short[] { 1, 1, 0, 0 };

            AbilityLogic = WickedSlashLogic;
        }
        public static IEnumerator WickedSlashLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.baseStats.power, caster = caster});
            }
        }
    }
    public class HeavySlash : Ability
    {
        public HeavySlash()
        {
            abilityName = "Heavy Slash";
            manaCost = 4;
            targetType = TargetType.SingleEnemy;
            targetSpots = new short[] { 1, 0, 0, 1 };

            AbilityLogic = HeavySlashLogic;
        }
        public static IEnumerator HeavySlashLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.baseStats.power * 1.5f, caster = caster});
            }
        }
    }
    public class GasterBlaster : Ability
    {
        public GasterBlaster()
        {
            abilityName = "Gaster Blaster";
            manaCost = 10;
            targetType = TargetType.AoEEnemy;
            targetSpots = new short[] { 1, 1, 0, 0 };
            AbilityLogic = GasterBlasterLogic;
        }
        public static IEnumerator GasterBlasterLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(0.75f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.baseStats.power * .75f, caster = caster });
            }
        }
    }
    public class ManaSteal : Ability
    {
        public ManaSteal()
        {
            abilityName = "Mana Steal";
            manaCost = 0;
            targetType = TargetType.SingleAlly;

            cooldownTime = 1;
            cooldownType = CooldownType.Round;

            abilityValue = 5;
            AbilityLogic = ManaStealLogic;
        }
        public static IEnumerator ManaStealLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {

            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { target = caster, value = ability.abilityValue, caster = caster });
            }
        }

        public override bool unitTargetable(Character target)
        {
            if (target.currentStats.currentMana > 0)
            {
                return true;
            }

            return false;
        }
    }
    public class ToughenUp : Ability
    {
        public ToughenUp()
        {
            abilityName = "Toughen Up";
            manaCost = 4;
            targetType = TargetType.Self;

            AbilityLogic = ToughenUpLogic;
        }
        public static IEnumerator ToughenUpLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.Heal { caster = caster, value = 5, target = caster});
                EffectSystem.ApplyEffect(new EffectLibrary.BigAndSmall { caster = caster, value = 2, target = caster });
            }
        }
    }
    public class SlickAttack : Ability
    {
        public SlickAttack()
        {
            abilityName = "Slick Attack";
            manaCost = 7;
            targetType = TargetType.SingleEnemy;
            targetSpots = new short[] { 1, 0, 0, 0 };
            AbilityLogic = SlickAttackLogic;
        }
        public static IEnumerator SlickAttackLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.baseStats.power * 1.5f, caster = caster });
            }
        }
    }

    public class DaringStep : Ability
    {
        public DaringStep()
        {
            abilityName = "Daring Step";
            manaCost = 5;
            targetType = TargetType.Self;
            cooldownTime = 1;
            cooldownType = CooldownType.Combat;

            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(4f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyEffect(new EffectLibrary.Riposte { caster = caster, target = target});
            }
        }
    }

    public class ChemicalThrow : Ability
    {
        public ChemicalThrow()
        {
            abilityName = "Chemical Throw";
            manaCost = 6;
            targetType = TargetType.AoEEnemy;
            abilityValue = 3;
            targetSpots = new short[] { 1, 1, 0, 0 };
            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyEffect(new EffectLibrary.Venom { caster = caster, target = target, value = ability.abilityValue, duration = 3, durationType = EffectDuration.Round });
            }
        }
    }

    #region GAMBLER class

    public class Autar : Ability
    {
        public Autar()
        {
            abilityName = "Autar";
            manaCost = 2;
            targetType = TargetType.AoEAll;
            abilityValue = 1;
            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                Effect randomEffect = EffectLibrary.GetARandomEffect();
                randomEffect.caster = caster;
                randomEffect.target = target;
                int value = 1;

                if (randomEffect.effectType == EffectType.All)
                {
                    float r = UnityEngine.Random.value;

                    if (r < .5f) value = -1;
                
                }
                randomEffect.value = value;

                EffectSystem.ApplyEffect(randomEffect);
            }
        }
    }
    public class TimeToRest : Ability
    {
        public TimeToRest()
        {
            abilityName = "Time To Rest";
            manaCost = 0;
            targetType = TargetType.Self;
            fireType = AbilityFireType.Instant;

            AbilityLogic = _AbilityLogic;
        }
        protected override IEnumerator PostExecute(Character caster, List<Character> targets)
        {
            yield return null;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            float r = UnityEngine.Random.Range(0, 100);

            foreach (var target in targets)
            {
                if (r < 20)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.Heal { caster = caster, target = target, value = target.baseStats.maxHP * .1f});
                }
                else if (r < 40)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = caster, target = target, value = target.baseStats.maxMana * .2f});
                }
                else if (r < 60)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.SpendOrEarnMoney { caster = caster, target = caster,value = 2 });
                }
                else if (r >= 60)
                {
                    //Do nothing
                }
            }
            yield return new WaitForSeconds(.3f);
            EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = caster, target = caster, value = caster.baseStats.manaRegen });

            EventBus.Raise(new TurnEndEvent { unit = caster });

        }
    }
    public class HeadsOrTails : Ability
    {
        public HeadsOrTails()
        {
            abilityName = "Heads Or Tails";
            manaCost = 2;
            targetType = TargetType.Self;

            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.4f);

            float r = UnityEngine.Random.value;

            foreach (var target in targets)
            {
                //heads
                if (r < .5f)
                {
                    EffectSystem.ApplyEffect(new EffectLibrary.BigAndSmall { caster = caster, target = target, value = 1});
                } else // tails
                {
                    float effectValue = target.getEffect("BigAndSmall") != null ? -target.getEffect("BigAndSmall").value : 0 ;
                    EffectSystem.ApplyEffect(new EffectLibrary.BigAndSmall { caster = caster, target = target, value = effectValue });
                }
            }

        }
    }
    public class SacrificialBlood : Ability
    {
        public SacrificialBlood()
        {
            abilityName = "Sacrificial Blood";
            manaCost = 3;
            targetType = TargetType.SingleAlly;
            abilityValue = 1;
            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.4f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyActionImmidiate(new ActionLibrary.DamageAction { caster = caster, target = target, value = ability.abilityValue * 8});
                EffectSystem.ApplyAction(new ActionLibrary.Pray { caster = caster, target = target, value = ability.abilityValue});
            }

        }
    }
    public class BasicBet : Ability
    {
        public BasicBet()
        {
            abilityName = "Basic Bet";
            manaCost = 0;
            targetType = TargetType.SingleEnemy;

            cooldownTime = 1;
            cooldownType = CooldownType.Round;
            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.3f);

            foreach (var target in targets)
            {
                float r = UnityEngine.Random.value;

                if (r < .5f)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = target.baseStats.power / 2});
                    yield return new WaitForSeconds(.3f);
                }
                r = UnityEngine.Random.value;

                if (r < .5f)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = target.baseStats.power / 2});
                    yield return new WaitForSeconds(.3f);
                }
                r = UnityEngine.Random.value;

                if (r < .5f)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = target.baseStats.power / 2});
                    yield return new WaitForSeconds(.3f);
                }
            }

        }
    }

    public class PoliticalSpeech : Ability
    {
        public PoliticalSpeech()
        {
            abilityName = "PoliticalSpeech";
            manaCost = 7;
            targetType = TargetType.AoEAll;
            abilityValue = 1;
            AbilityLogic = _AbilityLogic;
        }
        public static IEnumerator _AbilityLogic(
            Character caster,
            List<Character> targets, Ability ability)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = ability.abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                Effect effect = null;
                float r = UnityEngine.Random.value;
                if (target.Team == caster.Team)
                {
                    effect = new EffectLibrary.HealthRegen {caster = caster, target = target, value = 2, duration = 2 };
                    if (r < .5f)
                    {
                        EffectSystem.ApplyEffect(effect);
                    }
                    effect = new EffectLibrary.Power { caster = caster, target = target, value = 1, duration = 2, durationType = EffectDuration.Round };
                    if (r < .5f)
                    {
                        EffectSystem.ApplyEffect(effect);
                    }
                } else
                {
                    if (r < .5f)
                    {
                        if (r < .25f)
                        {
                            effect = new EffectLibrary.Confused { caster = caster, target = target, value = 1 };
                            EffectSystem.ApplyEffect(effect);
                        } else
                        {
                            effect = new EffectLibrary.Anger { caster = caster, target = target, value = 1 };
                            EffectSystem.ApplyEffect(effect);
                        }
                    }
                }
            }
        }
    }


    #endregion


    public static Ability StringToAbility(string abilityName)
    {
        abilityName = abilityName.Replace(" ", "");

        string fullName = $"AbilityLibrary+{abilityName}";
        var type = Type.GetType(fullName);

        if (type == null)
            return new NullAbility();

        if (!typeof(Ability).IsAssignableFrom(type))
            return new NullAbility();

        if (type.IsAbstract)
            return new NullAbility();

        return (Ability)Activator.CreateInstance(type);
    }
}

