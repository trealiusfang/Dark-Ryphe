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
                result.Add(caster);
                return result;

            case TargetType.SingleEnemy:
                 groupWanted = caster.Team == CharacterTeam.Light ? poses.DarkCharacters() : poses.LightCharacters();
                for (int i = 0; i < groupWanted.Count; i++)
                {
                    if (ability.targetSpots[i] == 1)
                    {
                        result.Add(groupWanted[i]);
                        break;
                    }
                }
                return result;

            case TargetType.AoEEnemy:
                groupWanted = caster.Team == CharacterTeam.Light ? poses.DarkCharacters() : poses.LightCharacters();
                for (int i = 0; i < groupWanted.Count; i++)
                {
                    if (ability.targetSpots[i] == 1)
                    {
                        result.Add(groupWanted[i]);
                    }
                }
                return result;

            case TargetType.SingleAlly:
                groupWanted = caster.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();
                for (int i = 0; i < groupWanted.Count; i++)
                {
                    if (ability.targetSpots[i] == 1 && groupWanted[i] != caster)
                    {
                        result.Add(groupWanted[i]);
                        break;
                    }
                }
                return result;

            case TargetType.AoEAlly:
                groupWanted = caster.Team == CharacterTeam.Light ? poses.LightCharacters() : poses.DarkCharacters();
                for (int i = 0; i < groupWanted.Count; i++)
                {
                    if (ability.targetSpots[i] == 1)
                    {
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
