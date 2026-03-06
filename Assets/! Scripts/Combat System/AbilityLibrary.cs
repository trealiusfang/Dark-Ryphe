using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class AbilityLibrary
{
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
            yield return new WaitForSeconds(0.3f); 

            foreach (var target in targets)
            {
                target.TakeDamage(caster.baseStats.power);
            }

            yield return new WaitForSeconds(0.2f);
            Debug.Log("You can use other abilities");
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
        public static IEnumerator EndTurnLogic(
            Character caster,
            List<Character> targets)
        {
            yield return new WaitForSeconds(0.3f);

            EventBus.Raise(new TurnEndEvent { unit = caster });
        }
    }
}

