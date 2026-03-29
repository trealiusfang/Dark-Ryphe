using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive 
{
    public Sprite sprite;
    public Character assignedUnit;
    public string passiveName;
    public PassiveEffectivenessType effectivenessType;

    public List<(Type type, Delegate action)> eventRoutes = new();

    public void SetPassive(PassiveSO passiveSO, Character _assignedUnit)
    {
        if (passiveSO == null) return;
        sprite = passiveSO.PassiveSprite;
        assignedUnit = _assignedUnit;
        EnablePassive();
    }
    public virtual void EnablePassive()
    {
        // Sub<CombatStartEvent>(OnCombatStart)
    }

    protected void Sub<T>(Action<T> action) where T : EventData
    {
        eventRoutes.Add((typeof(T), action));
    }

    public virtual float actionCalc(Action action, float _value, EffectedType type)
    {
        return _value;
    }

    public virtual float statCalc(statType statType, float _value)
    {
        return _value;
    }

    public virtual int statChange(statType statType, int _value)
    {
        //if (statType == statType.Speed)
        //{
        //  if (_value < 0)
        //      _value = 0; => Meaning it doesn't allow the change :) 
        //}
        return _value;
    }

    public virtual IEnumerator Execute(Action action, EffectedType type)
    {
        yield return null;
    }

    //Although these functions below are not needed, they are an easy way to get started on a passive and help our coders understand what they are capable of.
    public virtual void OnCombatStart(CombatStartEvent ev)
    {
        // EffectSystem.ApplyEffect(new EffectLibrary.Brace { });
    }
    public virtual void OnCombatEnd(CombatEndEvent ev)
    {
        // EffectSystem.ApplyAction(new ActionLibrary.Heal { value = 5 });
    }
    public virtual void OnRoundStart(RoundStartEvent ev)
    {
        
    }
    public virtual void OnRoundEnd(RoundEndEvent ev)
    {
        
    }
    public virtual void OnTurnStart(TurnStartEvent ev)
    {
        
    }
    public virtual void OnTurnEnd(TurnEndEvent ev)
    {
        
    }
    public virtual void OnUnitDeath(UnitDeathEvent ev)
    {
        if (ev.causer == assignedUnit)
        {
            //I have killed him!!
        }
    }

    public virtual void OnAction(ActionHappenedEvent ev)
    {
        if (ev.action.target == assignedUnit)
        {
            //oh I have taken damage, let's buff me :)
        }
    }
}

public enum PassiveEffectivenessType
{
    Combat,
    General //Lowering shop costs etc.
}
