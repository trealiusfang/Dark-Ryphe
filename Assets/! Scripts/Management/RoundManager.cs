using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
public class TurnManager : MonoBehaviour
{
    private Queue<Character> turnQueue = new();

    void Start()
    {
        EventBus.Sub<CombatStartEvent>(OnCombatStart);
        EventBus.UnSub<TurnEndEvent>(OnTurnEnd);
    }

    void OnCombatStart(CombatStartEvent e)
    {
        BuildTurnQueue();
        StartCoroutine(StartNextTurn());
    }

    void BuildTurnQueue()
    {
        Character[] units = FindObjectsByType<Character>(FindObjectsSortMode.None);

        var ordered = units
            .OrderByDescending(u => u.baseStats.speed + Random.Range(1, 8));

        turnQueue = new Queue<Character>(ordered);
    }

    IEnumerator StartNextTurn()
    {
        if (turnQueue.Count == 0)
            BuildTurnQueue();

        Character unit = turnQueue.Dequeue();

        yield return new WaitForSeconds(0.5f); //wait a lil before next guy

        EventBus.Raise(new TurnStartEvent { unit = unit });
    }

    void OnTurnEnd(TurnEndEvent e)
    {
        StartCoroutine(StartNextTurn());
    }
}