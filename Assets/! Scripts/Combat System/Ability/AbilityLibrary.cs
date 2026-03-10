using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public static class AbilityLibrary
{
    public class NullAbility : Ability
    {
        public NullAbility()
        {
            abilityName = "Nulll";
            manaCost = 0;
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
                EffectSystem.ApplyEffect(new EffectLibrary.Power { caster = caster, value = 2, target = caster });
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
    }//stringToAbility

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

