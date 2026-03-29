using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
public static class AbilityLibrary
{
    public class NullAbility : Ability
    {
        public NullAbility()
        {
            abilityName = "If you see this... There is an error";
            manaCost = 0;

            fireType = AbilityFireType.Instant;
            cooldownType = CooldownType.Round;
            cooldownTime = 1;

            _abilityLogic = AbilityLogic;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
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

            _abilityLogic = AbilityLogic;
        }

        public override string GetAbilityDescription(Character caster)
        {
            int value = EffectSystem.GetEffectCalculation(caster.GetStat(statType.Mana_Regen), caster, new ActionLibrary.ManaIncrease { actionType = ActionType.StatIncreaseManaRegen});
            return $"This unit ends its turn, gaining mana based on mana regen ({value})";
        }

        protected override IEnumerator PostExecute(Character caster, List<Character> targets)
        {
            yield return null;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = caster, value = caster.GetStat(statType.Mana_Regen), target = caster, actionType = ActionType.StatIncreaseManaRegen});
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

            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = caster.GetStatFloor(statType.Power);
            return $"Deals {value} damage, based on units power.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.GetStat(statType.Power), caster = caster});
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

            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = EffectSystem.GetEffectCalculation(caster.GetStat(statType.Power) * 1.5f, caster, new ActionLibrary.DamageAction { });
            return $"Deals heavy power scaling damage ({value})";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(0.55f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.GetStat(statType.Power) * 1.5f, caster = caster});
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
            targetSpots = new short[] { 1, 1, 1, 1 };
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = EffectSystem.GetEffectCalculation(caster.GetStat(statType.Power) * .75f, caster, new ActionLibrary.DamageAction { });
            return $"Deals low AoE damage with high cost ({value})";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(0.75f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.GetStat(statType.Power) * .75f, caster = caster });
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
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = abilityValue;
            return $"Steal {value} mana from an ally";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {

            yield return new WaitForSeconds(0.1f);
            foreach (var target in targets)
            {
                float value = target.GetStat(statType.Mana_Current) < abilityValue ? target.GetStat(statType.Mana_Current) : abilityValue;
                EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { target = caster, value = value, caster = caster });
                target.ChangeStat(statType.Mana_Current,Math.RoundValue(value));
            }
        }

        public override bool unitTargetable(Character target)
        {
            if (target.GetStat(statType.Mana_Current) > 0)
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

            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = 5;
            int value2 = 2;
            return $"Get +{value2} BIGGER, and heal for {value}";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
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
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = EffectSystem.GetEffectCalculation(caster.GetStat(statType.Power) * 1.5f, caster, new ActionLibrary.DamageAction { });
            return $"Deals heavy power scaling damage ({value}), front-line focused attack";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            PlayCharacterAnimation(caster);
            yield return new WaitForSeconds(0.2f);
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(0.2f);
            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { target = target, value = caster.GetStat(statType.Power) * 1.5f, caster = caster });
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

            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            return "Applies Riposte status effect to this unit.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
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
            abilityValue = 4;
            targetSpots = new short[] { 1, 1, 0, 0 };
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = abilityValue;
            return $"Applies AOE +{value} Venom , front-line focused attack.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyEffect(new EffectLibrary.Venom { caster = caster, target = target, value = abilityValue, duration = abilityValue, durationType = EffectDuration.Round });
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
            _abilityLogic = AbilityLogic;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                Effect randomEffect = EffectLibrary.GetARandomEffect();
                randomEffect.caster = caster;
                randomEffect.target = target;
                int value = 1;

                if (randomEffect.effectType == EffectType.both)
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

            _abilityLogic = AbilityLogic;
        }
        protected override IEnumerator PostExecute(Character caster, List<Character> targets)
        {
            yield return null;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            float r = UnityEngine.Random.Range(0, 100);

            foreach (var target in targets)
            {
                if (r < 20)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.Heal { caster = caster, target = target, value = target.GetStat(statType.HP_Max) * .1f});
                }
                else if (r < 40)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = caster, target = target, value = target.GetStat(statType.Mana_Max) * .2f});
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
            EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = caster, target = caster, value = caster.GetStat(statType.Mana_Regen) });

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

            _abilityLogic = AbilityLogic;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
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
                    float effectValue = target.effectHolder.getEffect("BigAndSmall") != null ? -target.effectHolder.getEffect("BigAndSmall").value : 0 ;
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
            _abilityLogic = AbilityLogic;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.4f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyActionImmidiate(new ActionLibrary.DamageAction { caster = caster, target = target, value = abilityValue * 8});
                EffectSystem.ApplyAction(new ActionLibrary.Pray { caster = caster, target = target, value = abilityValue});
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
            _abilityLogic = AbilityLogic;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.3f);

            foreach (var target in targets)
            {
                float r = UnityEngine.Random.value;

                if (r < .5f)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = target.GetStat(statType.Power) / 2});
                    yield return new WaitForSeconds(.3f);
                }
                r = UnityEngine.Random.value;

                if (r < .5f)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = target.GetStat(statType.Power) / 2});
                    yield return new WaitForSeconds(.3f);
                }
                r = UnityEngine.Random.value;

                if (r < .5f)
                {
                    EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = target.GetStat(statType.Power) / 2});
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
            _abilityLogic = AbilityLogic;
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
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

    #region Kool Bird

    public class DamageRush : Ability
    {
        public DamageRush()
        {
            abilityName = "Damage Rush";
            manaCost = 12;
            targetType = TargetType.SingleEnemy;

            targetSpots = new short[] { 1, 0, 0, 0 };
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = EffectSystem.GetEffectCalculation(caster.GetStat(statType.Power) * .33f, caster, new ActionLibrary.DamageAction { });
            return $"Deals {value} damage to all enemy units, scales with power.";
        }

        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            PlayCharacterAnimation(caster, 1);
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.4f);
            EventBus.Raise(new SFXEvent { sfx_clip = abilityAlternativeClip });
            yield return new WaitForSeconds(.3f);
            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = caster.GetStat(statType.Speed) * 2 });
            }
        }
    }

    #endregion
    #region bossboy
    public class RaiseFromTheDark : Ability
    {
        public RaiseFromTheDark()
        {
            abilityName = "Raise From The Dark";
            manaCost = 7;
            targetType = TargetType.AoEEnemy;

            targetSpots = new short[] { 1, 1, 1, 1 };
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = EffectSystem.GetEffectCalculation(caster.GetStat(statType.Power) * .33f, caster, new ActionLibrary.DamageAction { });
            return $"Deals {value} damage to all enemy units, scales with power.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            PlayCharacterAnimation(caster, 1);
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            foreach (var target in targets)
            {
                PlayEffectAnimation(target, new Vector3(0, 4.5f, 0), 5f);
            }
            yield return new WaitForSeconds(.4f);
            EventBus.Raise(new SFXEvent { sfx_clip = abilityAlternativeClip });

            yield return new WaitForSeconds(.3f);
            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = caster.GetStat(statType.Power) * .33f});
            }
        }
    }
    #endregion

    #region Huntress

    public class SubtleClaw : Ability
    {
        public SubtleClaw()
        {
            abilityName = "Subtle Claw";
            manaCost = 4;
            targetType = TargetType.SingleEnemy;
            abilityValue = 1;
            targetSpots = new short[] { 1, 1, 1, 1 };
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = abilityValue * 3;
            int value2 = abilityValue + caster.GetStatFloor(statType.Power);
            return $"Applies +{value} Bleed and deals {value2} damage scaling with power, can attack to any spot.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.5f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = abilityValue + caster.GetStat(statType.Power)});
                EffectSystem.ApplyEffect(new EffectLibrary.Bleed { caster = caster, target = target, value = abilityValue * 3, duration = abilityValue * 3, durationType = EffectDuration.Round });
            }
        }
    }

    public class Ocultarse : Ability
    {
        public Ocultarse()
        {
            abilityName = "Ocultarse";
            manaCost = 3;
            targetType = TargetType.Self;
            abilityValue = 2;

            cooldownTime = 1;
            cooldownType = CooldownType.Round;
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = abilityValue;
            return $"Evade the next enemy attack, castable once per turn.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyEffect(new EffectLibrary.DodgeInstinct { caster = caster, target = target, value = abilityValue });
            }
        }
    }

    public class IntimidatingHowl : Ability
    {
        public IntimidatingHowl()
        {
            abilityName = "Intimidating Howl";
            manaCost = 3;
            targetType = TargetType.AoEEnemy;
            targetSpots = new short[] { 1, 1, 1, 1 };

            cooldownTime = 1;
            cooldownType = CooldownType.Round;
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = abilityValue;
            return $"Lower ALL the enemies damage for 1 turn.";
        }
        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                EffectSystem.ApplyEffect(new EffectLibrary.Weakness { caster = caster, target = target, value = 1, duration = 1, durationType = EffectDuration.Round});
            }
        }
    }

    public class FinishHim : Ability
    {
        public FinishHim()
        {
            abilityName = "Finish Him";
            manaCost = 6;
            targetType = TargetType.SingleEnemy;
            targetSpots = new short[] { 0, 0, 1, 1 };

            cooldownTime = 3;
            cooldownType = CooldownType.Round;
            _abilityLogic = AbilityLogic;
        }
        public override string GetAbilityDescription(Character caster)
        {
            int value = abilityValue + caster.GetStatFloor(statType.Power);
            int value2 = abilityValue+ 2 + caster.GetStatFloor(statType.Power) * 2;
            return $"Deals {value} damage to back-line, deals {value2} damage if the enemy is marked.";
        }

        public override IEnumerator AbilityLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_clip = abilitySuccessClip });
            yield return new WaitForSeconds(.7f);

            foreach (var target in targets)
            {
                float value = target.effectHolder.getEffect("Mark") != null ? abilityValue + 2 + caster.GetStatFloor(statType.Power) * 2 : abilityValue + caster.GetStatFloor(statType.Power);
                EffectSystem.ApplyAction(new ActionLibrary.DamageAction { caster = caster, target = target, value = value, critSound = abilityCritClip });
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

