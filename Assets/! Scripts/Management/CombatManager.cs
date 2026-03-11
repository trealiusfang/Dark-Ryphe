using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : BusRoute
{
    List<Character> Lights, Darks;
    public void StartCombat()
    {
        Debug.Log("Combat has started!");
        EventBus.Raise(new CombatStartEvent { });
        SetTeams();
    }

    private void EndCombat(CombatEndEvent ev)
    {
        Debug.Log(ev.winningTeam.ToString() + " wins!");
        EventBus.Raise(new SFXEvent { sfx_string = "Win"});
        EventBus.Raise(new MusicEvent { music_string = "Swaying Daises" });
    }

    private void Start()
    {
        SubnApply<CombatEndEvent>(EndCombat);
        SubnApply<UnitDeathEvent>(OnDeath);
    }
    private void SetTeams()
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

        destroyer.Clear();
        for (int i = 0; i < Lights.Count; i++)
        {
            if (Darks[i].Team != CharacterTeam.Dark)
            {
                destroyer.Add(Darks[i]);
            }
        }
        Darks.RemoveAll(character => destroyer.Contains(character));
    }
    private void OnDeath(UnitDeathEvent ev)
    {
        if (Lights.Contains(ev.unit))
        {
            Lights.Remove(ev.unit);
            if (Lights.Count == 0)
            {
                EventBus.Raise(new CombatEndEvent { winningTeam = CharacterTeam.Dark });
            }
        }
        if (Darks.Contains(ev.unit))
        {
            Darks.Remove(ev.unit);
            if (Darks.Count == 0)
            {
                EventBus.Raise(new CombatEndEvent { winningTeam = CharacterTeam.Light});
            }
        }
    }
}
