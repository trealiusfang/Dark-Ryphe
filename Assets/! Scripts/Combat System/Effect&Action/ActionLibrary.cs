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

            actionLogic = ActionLogic;
        }

        public override IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            if (target == null || target.isDead() || caster == null || caster.isDead()) yield break;

            if (!isCrit)
            {
                target.TakeDamage(Math.RoundValue(value), caster);

                Debug.Log(target.charData.name + " was attacked, lost " + value + " health! By: " + (caster != null ? caster.name : ""));
                
                if (hitSound != null)
                    EventBus.Raise(new SFXEvent { sfx_clip = hitSound });
                EventBus.Raise(new BattleTextEvent { text = "" + "/value", character = target, textAnimType = TextAnimType.Damage, value = Math.RoundValue(value)});
            } else
            {
                target.TakeDamage(Math.RoundValue(value * 1.5f), caster);
                Debug.Log(target.charData.name + " was attacked, lost " + Mathf.FloorToInt(value * 1.5f) + " health! By: " + (caster != null ? caster.name : ""));
                //Call effects
                if (critSound == null)EventBus.Raise(new SFXEvent { sfx_string = "Critical" });
                else EventBus.Raise(new SFXEvent { sfx_clip = critSound });
                EventBus.Raise(new BattleTextEvent { text = "" + "/value", character = target, textAnimType = TextAnimType.Critical, value = Mathf.FloorToInt(value * 1.5f) });
            }
            yield return null;
        }
    }

    public class VenomDamage : Action
    {
        public VenomDamage()
        {
            actionType = ActionType.DamageMagic;

            actionLogic = ActionLogic;
        }
        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            if (target == null || target.isDead()) yield break;

            target.TakeDamage(Math.RoundValue(value), caster);
            Debug.Log(target.charData.name + " lost " + value + " health, by venom!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "" + "/value", character = target, textAnimType = TextAnimType.Venom, value = Math.RoundValue(value) });
            yield return null;
        }

    }
    public class ManaIncrease : Action
    {
        public ManaIncrease()
        {
            actionType = ActionType.StatIncreaseMana;

            actionLogic = ActionLogic;
        }

        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            int actualValue = (target.GetStatFloor(statType.Mana_Max) - target.GetStatFloor(statType.Mana_Current) - Mathf.Round(value)) < 0 ? (target.GetStatFloor(statType.Mana_Max) - target.GetStatFloor(statType.Mana_Current)) : Mathf.FloorToInt(value);
            actualValue = (target.GetStatFloor(statType.Mana_Current) + actualValue) < 0 ? -target.GetStatFloor(statType.Mana_Current) : actualValue;

            target.ChangeStat(statType.Mana_Current, actualValue);
            Debug.Log(target.name + " gained " + value + " mana!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "+" + "/value" + " MANA", character = target, textAnimType = TextAnimType.Freeze, value = Math.RoundValue(actualValue) });
            if (actionType != ActionType.StatIncreaseManaRegen) EventBus.Raise(new AbilitySetChanged { unit = target, selectionEnabled = true});
            yield return null;
        }
    }

    public class Heal : Action
    {
        public Heal()
        {
            actionType = ActionType.Heal;

            actionLogic = ActionLogic;
        }
        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            value = isCrit ? value * 1.5f : value;
            int actualValue = 0;

            if (target.GetStatFloor(statType.HP_Max) - target.GetStatFloor(statType.HP_Current) - Mathf.Round(value) < 0)
            {
                actualValue = target.GetStatFloor(statType.HP_Max) - target.GetStatFloor(statType.HP_Current);
            } else
            {
                actualValue =  Mathf.FloorToInt(value);
            }
            if (actualValue <= 0) yield break;

            target.ChangeStat(statType.HP_Current, actualValue);
            Debug.Log(caster.name + " gained " + actualValue + " HP!");
            //Call effects

            EventBus.Raise(new BattleTextEvent { text = "+" + "/value" + " HP", character = target, textAnimType = TextAnimType.Heal, isCrit = isCrit, value = Math.RoundValue(actualValue) });
            EventBus.Raise(new AbilitySetChanged { unit = target, selectionEnabled = true });
            yield return null;
        }
    }
    public class Pray : Action
    {
        public Pray()
        {
            actionType = ActionType.StatIncreaseLuck;

            actionLogic = ActionLogic;
        }

        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            float actualValue = (target.GetStat(statType.Luck) + value) < 0 ? -target.GetStat(statType.Luck) : value;

            target.ChangeStat(statType.Luck, Math.RoundValue(actualValue));

            Debug.Log(target.name + " gained " + value + " luck!");
            //Call effects
            EventBus.Raise(new BattleTextEvent { text = "+" + "/value" + " Luck", character = target, textAnimType = TextAnimType.Shock });
            yield return null;
        }
    }

    public class SpendOrEarnMoney : Action
    {
        public SpendOrEarnMoney()
        {
            actionType = ActionType.None;

            actionLogic = ActionLogic;
        }

        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            if (value < 0)
            EventBus.Raise(new BattleTextEvent { text = "-" + Math.RoundValue(Mathf.Abs(value)) + " Money", character = target, textAnimType = TextAnimType.Metallic });
            if (value > 0)
            EventBus.Raise(new BattleTextEvent { text = "+" + "/value" + " Money", character = target, textAnimType = TextAnimType.Premium, value = Math.RoundValue(Mathf.Abs(value)) });
            yield return null;
        }
    }

    public class Dodge : Action
    {
        public Dodge() 
        {
            actionType = ActionType.None;


            actionLogic = ActionLogic;
        }

        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            EventBus.Raise(new BattleTextEvent { text = "DODGE!", character = target, textAnimType = TextAnimType.Metallic });

            yield return null;
        }
    }
    public class Miss : Action
    {
        public Miss()
        {
            actionType = ActionType.None;

            actionLogic = ActionLogic;
        }

        public override  IEnumerator ActionLogic(Character caster, Character target, float value, bool isCrit)
        {
            EventBus.Raise(new BattleTextEvent { text = "MISS!", character = target, textAnimType = TextAnimType.Metallic });

            yield return null;
        }
    }
}
