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
            .OrderByDescending(u => u.baseStats.speed);

        turnQueue = new Queue<Character>(ordered);
    }

    IEnumerator StartNextTurn()
    {
        if (turnQueue.Count == 0)
        {
            Debug.Log("New Round");
            BuildTurnQueue();
        }
        currentUnit = turnQueue.Dequeue();
        Debug.Log("Now arrives: " + currentUnit.name);

        yield return new WaitForSeconds(0.5f); //wait a lil before next guy

        EventBus.Raise(new TurnStartEvent { unit = currentUnit });
    }

    void OnTurnEnd(TurnEndEvent ev)
    {
        StartCoroutine(StartNextTurn());
    }

    void RemoveDeadUnit(UnitDeathEvent ev)
    {
        if (ev.unit ==  currentUnit)
        {
            StartCoroutine(StartNextTurn());
        } else
        {
            List<Character> units = turnQueue.ToList();
            units.Remove(ev.unit);

            turnQueue = new Queue<Character>(units);
        }
    }
}