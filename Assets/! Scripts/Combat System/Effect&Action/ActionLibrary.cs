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

        public static IEnumerator newActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            if (target == null || target.isDead() || caster == null || caster.isDead()) yield break;

            if (!isCrit)
            {
                target.TakeDamage(Mathf.FloorToInt(value));
                Debug.Log(target.charData.name + " was attacked, lost " + value + " health! By: " + (caster != null ? caster.name : ""));
                //Call effects
                EventBus.Raise(new BattleTextEvent { text = "" + Mathf.FloorToInt(value), character = target, textAnimType = TextAnimType.Damage });
            } else
            {
                target.TakeDamage(Mathf.FloorToInt(value * 1.5f));
                Debug.Log(target.charData.name + " was attacked, lost " + Mathf.FloorToInt(value * 1.5f) + " health! By: " + (caster != null ? caster.name : ""));
                //Call effects
                EventBus.Raise(new SFXEvent { sfx_string = "Critical" });
                EventBus.Raise(new BattleTextEvent { text = "" + Mathf.FloorToInt(value * 1.5f), character = target, textAnimType = TextAnimType.Critical });
            }
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
        public static IEnumerator newActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            if (target == null || target.isDead()) yield break;

            target.TakeDamage(Mathf.FloorToInt(value));
            Debug.Log(target.charData.name + " lost " + value + " health, by venom!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "" + Mathf.FloorToInt(value), character = target, textAnimType = TextAnimType.Venom });
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

        public static IEnumerator newActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            int actualValue = (target.baseStats.maxMana - target.currentStats.currentMana - Mathf.FloorToInt(value)) < 0 ? target.baseStats.maxMana - target.currentStats.currentMana : Mathf.FloorToInt(value);
            actualValue = (target.currentStats.currentMana + actualValue) < 0 ? -target.currentStats.currentMana : actualValue;

            target.currentStats.currentMana += actualValue;

            Debug.Log(target.name + " gained " + value + " mana!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "+" + Mathf.FloorToInt(actualValue) + " MANA", character = target, textAnimType = TextAnimType.Freeze });
            yield return null;
        }
    }

    public class Heal : Action
    {
        public Heal()
        {
            actionType = ActionType.Heal;

            ActionLogic = newActionLogic;
        }
        public static IEnumerator newActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            value = isCrit ? value * 1.5f : value;

            int actualValue = (target.baseStats.maxHP - target.currentStats.currentHP - Mathf.FloorToInt(value)) < 0 ? target.baseStats.maxHP - target.currentStats.currentHP : Mathf.FloorToInt(value);
            if (actualValue < 0) actualValue = 0;

            target.currentStats.currentHP += actualValue;
            Debug.Log(caster.name + " gained " + actualValue + " HP!");
            //Call effects

            EventBus.Raise(new BattleTextEvent { text = "+" + Mathf.FloorToInt(actualValue) + " HP", character = target, textAnimType = TextAnimType.Heal, isCrit = isCrit });
            yield return null;
        }
    }
    public class Pray : Action
    {
        public Pray()
        {
            actionType = ActionType.StatIncreaseLuck;

            ActionLogic = newActionLogic;
        }

        public static IEnumerator newActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            float actualValue = (target.baseStats.luck + value) < 0 ? -target.baseStats.luck : value;

            target.baseStats.luck += (short)Mathf.FloorToInt(actualValue);

            Debug.Log(target.name + " gained " + value + " luck!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "+" + Mathf.FloorToInt(actualValue) + " Luck", character = target, textAnimType = TextAnimType.Shock });
            yield return null;
        }
    }

    public class SpendOrEarnMoney : Action
    {
        public SpendOrEarnMoney()
        {
            actionType = ActionType.None;

            ActionLogic = newActionLogic;
        }

        public static IEnumerator newActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            if (value < 0)
            EventBus.Raise(new BattleTextEvent { text = "-" + Mathf.FloorToInt(Mathf.Abs(value)) + " Money", character = target, textAnimType = TextAnimType.Metallic });
            if (value > 0)
            EventBus.Raise(new BattleTextEvent { text = "+" + Mathf.FloorToInt(value) + " Money", character = target, textAnimType = TextAnimType.Premium });
            yield return null;
        }
    }
}
