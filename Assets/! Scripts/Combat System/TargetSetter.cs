using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

public static class TargetSetter 
{
    public static List<Character> SetTarget(Character caster, Ability ability)
    {
        CharacterPositioning poses = GameInitializer.instance._combatManagers.GetComponent<CharacterPositioning>();
        TargetType type = ability.targetType;
        List<Character> result = new List<Character>();

        List<Character> groupWanted = new List<Character>(); 
        switch (type) {

            case TargetType.Self:
                if (ability.unitTargetable(caster))
                    result.Add(caster);
                return result;

            case TargetType.SingleEnemy or TargetType.AoEEnemy:
                 groupWanted = caster.Team == CharacterTeam.Light ? poses.DarkCharacters() : poses.LightCharacters();
                for (int i = 0; i < groupWanted.Count; i++)
                {
                    if (ability.targetSpots[i] == 1)
                    {
                        if (ability.unitTargetable(groupWanted[i]))
                            result.Add(groupWanted[i]);
                    }
                }
                return result;

            case TargetType.SingleAlly or TargetType.AoEAlly:
                groupWanted = caster.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();
                for (int i = 0; i < groupWanted.Count; i++)
                {
                    if (ability.targetSpots[i] == 1 && groupWanted[i] != caster)
                    {
                        if (ability.unitTargetable(groupWanted[i]))
                            result.Add(groupWanted[i]);
                    }
                }
                return result;

            default:
                Debug.Log("used default");
                result.Add(caster);
                return result;

        }
    }
}
