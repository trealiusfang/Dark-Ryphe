using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
public class TurnManager : MonoBehaviour
{
    private Queue<Character> turnQueue = new();

    void Awake()
    {
        EventBus.Sub<CombatStartEvent>(OnCombatStart);
        EventBus.Sub<TurnEndEvent>(OnTurnEnd);
        EventBus.Sub<UnitDeathEvent>(RemoveDeadUnit);
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
            .OrderByDescending(u => u.baseStats.speed + Random.Range(1, 8));

        turnQueue = new Queue<Character>(ordered);
    }

    IEnumerator StartNextTurn()
    {
        if (turnQueue.Count == 0)
        {
            Debug.Log("New Round");
            BuildTurnQueue();
        }
        Character unit = turnQueue.Dequeue();
        Debug.Log("Now arrives: " + unit.name);

        yield return new WaitForSeconds(0.5f); //wait a lil before next guy

        EventBus.Raise(new TurnStartEvent { unit = unit });
    }

    void OnTurnEnd(TurnEndEvent ev)
    {
        StartCoroutine(StartNextTurn());
    }

    void RemoveDeadUnit(UnitDeathEvent ev)
    {
        List<Character> units = turnQueue.ToList();
        units.Remove(ev.unit);

        turnQueue = new Queue<Character>(units);
    }
}