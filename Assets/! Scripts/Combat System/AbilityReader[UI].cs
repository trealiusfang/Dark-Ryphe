using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityReader : MonoBehaviour
{
    private void Awake()
    {
        EventBus.Sub<TurnStartEvent>(ReadAbility);
    }

    void ReadAbility(TurnStartEvent ev)
    {
        Character character = ev.unit;
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(character.abilities[i].abilityName);
        }
    }
}
