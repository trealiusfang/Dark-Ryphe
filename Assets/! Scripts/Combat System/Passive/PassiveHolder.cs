using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveHolder : BusRoute
{
    private List<PassiveSO> passiveSOs = new List<PassiveSO>();
    private List<Passive> Passives = new List<Passive>();
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();

        if (character == null) { Debug.LogError($"CHARACTER IS NULL! \"{transform.name}\""); }

        passiveSOs = character.charData.Passives;

        for (int i = 0; i< passiveSOs.Count; i++)
        {
            Passive passive;
            if (passiveSOs[i] == null)
            {
                passive = PassiveLibrary.StringToPassive("NullPassive");
            } else
            {
                passive = PassiveLibrary.StringToPassive(passiveSOs[i].PassiveName);
            }

            Passives.Add(passive);
            passive.SetPassive(passiveSOs[i], character);

            Debug.Log(passive.passiveName + ", " + passive.assignedUnit.charData.characterName);
        }

        Sub<CombatStartEvent>(OnCombatStart);
        Sub<CombatEndEvent>(OnCombatEnd);
    }

    private void OnCombatStart(CombatStartEvent ev)
    {
        foreach (Passive p in Passives) 
        {
            foreach (var route in p.eventRoutes)
            {
                if (p.effectivenessType != PassiveEffectivenessType.Combat) continue;
                var method = typeof(EventBus)
                    .GetMethod(nameof(EventBus.Sub))
                    .MakeGenericMethod(route.type);

                method.Invoke(null, new object[] { route.action });
            }
        }
    }

    private void OnCombatEnd(CombatEndEvent ev)
    {
        foreach (Passive p in Passives)
        {
            foreach (var route in p.eventRoutes)
            {
                if (p.effectivenessType != PassiveEffectivenessType.Combat) continue;
                var method = typeof(EventBus)
                    .GetMethod(nameof(EventBus.UnSub))
                    .MakeGenericMethod(route.type);

                method.Invoke(null, new object[] { route.action });
            }
        }
    }

    protected override void OnDisable()
    {
        OnCombatEnd(null);
        base.OnDisable();
    }

    public List<Passive> GetPassives()
    {
        return Passives;
    }
}
