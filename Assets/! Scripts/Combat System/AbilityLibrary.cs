using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityLibrary;
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
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Error Alarm"});
            yield return new WaitForSeconds(0.3f);
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
            List<Character> targets)
        {
            caster.currentStats.currentMana += caster.baseStats.manaRegen;
            yield return new WaitForSeconds(0.3f);
            EventBus.Raise(new TurnEndEvent { unit = caster });
        }
    }
    public class WickedSlash : Ability
    {
        public WickedSlash()
        {
            abilityName = "Wicked Slash";
            manaCost = 3;
            targetType = TargetType.SingleEnemy;

            AbilityLogic = WickedSlashLogic;
        }
        public static IEnumerator WickedSlashLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Wicked Slice" });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                target.TakeDamage(caster.baseStats.power);
                Debug.Log(target.charData.name + " was attacked, lost " + Mathf.FloorToInt(caster.baseStats.power * .75f) + " damage! By: " + caster.name);
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
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Gaster Blaster" });
            yield return new WaitForSeconds(0.45f);

            foreach (var target in targets)
            {
                target.TakeDamage(Mathf.FloorToInt(caster.baseStats.power * .75f));
                Debug.Log(target.charData.name + " was attacked, lost " + Mathf.FloorToInt(caster.baseStats.power * .75f) + " damage! By: " + caster.name);
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

            AbilityLogic = ManaStealLogic;
        }
        public static IEnumerator ManaStealLogic(
            Character caster,
            List<Character> targets)
        {

            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                target.currentStats.currentMana -= 5;
                caster.currentStats.currentMana += 5;
            }
        }
    }
    public class ToughenUp : Ability
    {
        public ToughenUp()
        {
            abilityName = "Toughen Up";
            manaCost = 3;
            targetType = TargetType.Self;

            AbilityLogic = ToughenUpLogic;
        }
        public static IEnumerator ToughenUpLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Toughen Up" });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                target.currentStats.currentHP += 5;
                target.baseStats.power += 2;
                Debug.Log(target.charData.name + " was buffed, gained 5 HP and 2 power. By: "+ caster.name);
            }
        }
    }
    public class SlickAttack : Ability
    {
        public SlickAttack()
        {
            abilityName = "Slick Attack";
            manaCost = 5;
            targetType = TargetType.SingleEnemy;
            targetSpots = new short[] { 1, 1, 0, 0 };
            AbilityLogic = SlickAttackLogic;
        }
        public static IEnumerator SlickAttackLogic(
            Character caster,
            List<Character> targets)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Slick Attack" });
            yield return new WaitForSeconds(0.1f);

            foreach (var target in targets)
            {
                target.TakeDamage(Mathf.FloorToInt(caster.baseStats.power * 1.2f));
            }
        }
    }

    public static Ability stringToAbility(string abilityName)
    {
        switch (abilityName)
        {
            case "End Turn":
                return new EndTurn();
            case "Wicked Slash":
                return new WickedSlash();
            case "Gaster Blaster":
                return new GasterBlaster();
            case "Mana Steal":
                return new ManaSteal();
            case "Toughen Up":
                return new ToughenUp();
            case "Slick Attack":
                return new SlickAttack();
            default: return new NullAbility();

        }
    }
}

