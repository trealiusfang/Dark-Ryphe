using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class CharacterPositioning : MonoBehaviour
{
    private void Awake()
    {
        EventBus.Sub<CombatStartEvent>(OnStart);
        EventBus.Sub<UnitDeathEvent>(OnDeath);
    }
    List<Character> Lights;
    List<Character> Darks;
    private void OnStart(CombatStartEvent ev)
    {
        UpdatePositionings();
    }
    private void OnDeath(UnitDeathEvent ev)
    {
        if (Lights.Contains(ev.unit))
        {
            Lights.Remove(ev.unit);
        }
        if (Darks.Contains(ev.unit))
        {
            Darks.Remove(ev.unit);
        }
    }
    private void UpdatePositionings()
    {
        Character[] units = FindObjectsByType<Character>(FindObjectsSortMode.None);

        Lights = units.ToList();
        Darks = units.ToList();

        List<Character> destroyer = new List<Character>();
        for (int i = 0; i < Lights.Count; i++)
        {
            if (Lights[i].Team != CharacterTeam.Light)
            {
                destroyer.Add(Lights[i]);
            }
        }
        Lights.RemoveAll(character => destroyer.Contains(character));
        Lights = Lights.OrderByDescending(character => character.transform.position.x).ToList();

        Darks.RemoveAll(character => Lights.Contains(character));
        Darks = Darks.OrderBy(character => character.transform.position.x).ToList();

        Debug.Log("Lights: " + Lights.Count + ", Darks: " + Darks.Count);
    }

    public List<Character> LightCharacters()
    {
        return Lights ?? new List<Character>();
    }

    public List<Character> DarkCharacters()
    {
        return Darks ?? new List<Character>();
    }
}
