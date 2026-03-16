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
        CharacterLister lister = GameInitializer.instance._combatManagers.GetComponent<CharacterLister>();

        if (lister == null) return;

        Lights = lister.LightCharacters();
        Darks = lister.DarkCharacters();
    }
    private void OnDeath(UnitDeathEvent ev)
    {
        if (ev.unit.Team == CharacterTeam.Light)
        {
            Lights.Remove(ev.unit);

            if (Lights.Count == 0)
            {
                EventBus.Raise(new CombatEndEvent { winningTeam = CharacterTeam.Dark });
            }
        }
        if (ev.unit.Team == CharacterTeam.Dark)
        {
            Darks.Remove(ev.unit);
            Debug.Log(Darks.Count);
            if (Darks.Count == 0)
            {
                EventBus.Raise(new CombatEndEvent { winningTeam = CharacterTeam.Light});
            }
        }
    }
}
