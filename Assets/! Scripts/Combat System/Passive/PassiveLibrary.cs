using System;
using UnityEngine;
public static class PassiveLibrary 
{
    public class NullPassive : Passive
    {
        public NullPassive()
        {
            passiveName = "Null";
        }

        public override void EnablePassive()
        {
            Sub<CombatStartEvent>(OnCombatStart);
        }

        public override void OnCombatStart(CombatStartEvent ev)
        {
            EventBus.Raise(new SFXEvent { sfx_string = "Error Alarm" });
            Debug.LogWarning("Null Passive!");
        }
    }

    public class LethalInstinct : Passive
    {
        public LethalInstinct() 
        {
            passiveName = "Lethal Instinct";
        }

        public override float actionCalc(Action action, float _value, EffectedType type)
        {
            if (type == EffectedType.Dealer)
            {
                if (action.target.GetStat(statType.HP_Current) < 15)
                {
                    _value += _value * .33f;
                }
            }

            return _value;
        }
    }

    public class Momentum : Passive
    {
        public Momentum() 
        {
            passiveName = "Momentum";
        }

        public override void EnablePassive()
        {

            Sub<UnitDeathEvent>(OnUnitDeath);
        }

        public override void OnUnitDeath(UnitDeathEvent ev)
        {
            if (ev.causer == assignedUnit)
            {
                EffectSystem.ApplyAction(new ActionLibrary.ManaIncrease { caster = assignedUnit, value = 3, target = assignedUnit});
                EffectSystem.ApplyAction(new ActionLibrary.Heal { caster = assignedUnit, value = 10, target = assignedUnit});
            }
        }
    }



    public static Passive StringToPassive(string abilityName)
    {
        abilityName = abilityName.Replace(" ", "");

        string fullName = $"PassiveLibrary+{abilityName}";
        var type = typeof(PassiveLibrary).GetNestedType(abilityName);

        if (type == null)
            return new NullPassive();

        if (!typeof(Passive).IsAssignableFrom(type))
            return new NullPassive();

        if (type.IsAbstract)
            return new NullPassive();

        return (Passive)Activator.CreateInstance(type);
    }
}
