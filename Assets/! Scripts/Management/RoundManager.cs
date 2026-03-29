using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
public class TurnManager : BusRoute
{
    private Queue<Character> turnQueue = new();
    private Character currentUnit;
    void Awake()
    {
        Sub<CombatStartEvent>(OnCombatStart);
        Sub<TurnEndEvent>(OnTurnEnd);
        Sub<UnitDeathEvent>(RemoveDeadUnit);
    }

    void OnCombatStart(CombatStartEvent e)
    {
        BuildTurnQueue();
        StartCoroutine(StartNextTurn());
    }

    void BuildTurnQueue()
    {
        Character[] units = FindObjectsByType<Character>(FindObjectsSortMode.None);

        if (units.Length == 0) { Debug.Log("There isn't anybody"); }

        var ordered = units
            .OrderByDescending(u => u.GetStat(statType.Speed));

        turnQueue = new Queue<Character>(ordered);
    }

    IEnumerator beforeNextTurn(TurnEndEvent ev)
    {
        yield return ResolveTurnEnd(ev.unit);
        yield return StartNextTurn();
    }

    IEnumerator StartNextTurn()
    {
        yield return ResolveTurnEnd(currentUnit);
        if (turnQueue.Count == 0)
        {
            Debug.Log("New Round");
            EventBus.Raise(new RoundEndEvent { });
            BuildTurnQueue();
        }
        currentUnit = turnQueue.Dequeue();
        Debug.Log("Now arrives: " + currentUnit.name);

        yield return new WaitForSeconds(0.5f); //wait a lil before next guy

        EventBus.Raise(new TurnStartEvent { unit = currentUnit });
        yield return ResolveTurnStart(currentUnit);
    }

    void OnTurnEnd(TurnEndEvent ev)
    {
        StartCoroutine(StartNextTurn());
    }

    void RemoveDeadUnit(UnitDeathEvent ev)
    {
        if (ev.unit ==  currentUnit)
        {
            AbilityFirer.StopLastUsedAbility();
            StartCoroutine(StartNextTurn());
        } else
        {
            List<Character> units = turnQueue.ToList();
            units.Remove(ev.unit);

            turnQueue = new Queue<Character>(units);
        }
    }
    public IEnumerator ResolveTurnStart(Character unit)
    {
        if (unit == null) yield break;
        TurnStartEvent ev = new TurnStartEvent { unit = unit };

        //collect effects
        List<Effect> effects = new List<Effect>(unit.effectHolder.GetEffects());

        foreach (var effect in effects)
        {
            yield return effect.OnTurnStart(ev);
        }

        //AFTER all effects finish
        EventBus.Raise(new UnitReadyEvent { unit = unit });
    }

    public IEnumerator ResolveTurnEnd(Character unit)
    {
        if (unit == null) yield break;
        TurnEndEvent ev = new TurnEndEvent { unit = unit };

        //collect effects
        List<Effect> effects = new List<Effect>(unit.effectHolder.GetEffects());

        foreach (var effect in effects)
        {
            yield return effect.OnTurnEnd(ev);
        }
    }
}

public class UnitReadyEvent : EventData
{
    public Character unit;
}