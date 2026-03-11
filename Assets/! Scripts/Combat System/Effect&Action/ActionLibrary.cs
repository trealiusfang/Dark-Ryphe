using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionLibrary
{
    public class DamageAction : Action
    {
        public DamageAction()
        {
            actionType = ActionType.DamagePhysical;

            ActionLogic = newActionLogic;
        }

        public static IEnumerator newActionLogic(Character caster, Character target, float value)
        {
            if (target == null || target.isDead() || caster == null || caster.isDead()) yield break;

            target.TakeDamage(Mathf.FloorToInt(value));
            Debug.Log(target.charData.name + " was attacked, lost " + value + " health! By: " + (caster != null ? caster.name : ""));
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "" + Mathf.FloorToInt(value), position = target.transform.position, textAnimType = TextAnimType.Damage });
            yield return null;
        }
    }

    public class CritAction : Action
    {
        public CritAction()
        {
            actionType = ActionType.DamagePhysical;

            ActionLogic = newActionLogic;
        }
        public static IEnumerator newActionLogic(Character caster, Character target, float value)
        {
            if (target == null || target.isDead() || caster == null || caster.isDead()) yield break;

            target.TakeDamage(Mathf.FloorToInt(value * 1.5f));
            Debug.Log(target.charData.name + " was attacked, lost " + value + " health! By: " + (caster != null ? caster.name : ""));
            //Call effects
            EventBus.Raise(new SFXEvent { sfx_string = "Critical" });
            EventBus.Raise(new BattleTextEvent { text = "" + Mathf.FloorToInt(value * 1.5f), position = target.transform.position, textAnimType = TextAnimType.Critical });
            yield return null;
        }
    }

    public class VenomDamage : Action
    {
        public VenomDamage()
        {
            actionType = ActionType.DamageMagic;

            ActionLogic = newActionLogic;
        }
        public static IEnumerator newActionLogic(Character caster, Character target, float value)
        {
            if (target == null || target.isDead()) yield break;

            target.TakeDamage(Mathf.FloorToInt(value));
            Debug.Log(target.charData.name + " lost " + value + " health, by venom!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "" + Mathf.FloorToInt(value), position = target.transform.position, textAnimType = TextAnimType.Venom });
            yield return null;
        }

    }
    public class ManaIncrease : Action
    {
        public ManaIncrease()
        {
            actionType = ActionType.StatIncreaseMana;

            ActionLogic = newActionLogic;
        }

        public static IEnumerator newActionLogic(Character caster, Character target, float value)
        {
            int actualValue = (target.baseStats.maxMana - target.currentStats.currentMana - Mathf.FloorToInt(value)) < 0 ? target.baseStats.maxMana - target.currentStats.currentMana : Mathf.FloorToInt(value);
            actualValue = (target.currentStats.currentMana + actualValue) < 0 ? -target.currentStats.currentMana : actualValue;

            target.currentStats.currentMana += actualValue;

            Debug.Log(target.name + " gained " + value + " mana!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "+" + Mathf.FloorToInt(actualValue) + " MANA", position = target.transform.position, textAnimType = TextAnimType.Freeze });
            yield return null;
        }
    }

    public class Heal : Action
    {
        public Heal()
        {
            actionType = ActionType.Heal;

            ActionLogic = HealLogic;
        }
        public static IEnumerator HealLogic(Character caster, Character target, float value)
        {
            int actualValue = (target.baseStats.maxHP - target.currentStats.currentHP - Mathf.FloorToInt(value)) < 0 ? target.baseStats.maxHP - target.currentStats.currentHP : Mathf.FloorToInt(value);
            if (actualValue < 0) actualValue = 0;

            target.currentStats.currentHP += actualValue;
            Debug.Log(caster.name + " gained " + actualValue + " HP!");
            //Call effects

            EventBus.Raise(new BattleTextEvent { text = "+" + Mathf.FloorToInt(actualValue) + " HP", position = target.transform.position, textAnimType = TextAnimType.Heal });
            yield return null;
        }
    }
}
