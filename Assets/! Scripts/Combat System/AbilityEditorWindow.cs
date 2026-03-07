using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AbilityEditorWindow : EditorWindow
{
    private CharacterData selectedCharacter;
    private Vector2 abilityListScroll;
    private Vector2 assignedListScroll;
    private Vector2 assignedScroll;
    private List<AbilitySO> abilitySOs = new List<AbilitySO>();

    [MenuItem("Window/Ability Editor")]
    public static void ShowWindow()
    {
        GetWindow<AbilityEditorWindow>("Ability Editor");
    }

    private void OnEnable()
    {
         abilitySOs = Resources.LoadAll<AbilitySO>("Abilities").ToList(); // Put assets in Resources/Abilities
    }
    void DrawAssignedAbilities()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(350));

        assignedScroll = EditorGUILayout.BeginScrollView(assignedScroll);

        List<AbilitySO> abilitys = selectedCharacter.Abilities;
        for (int i = 0; i < abilitys.Count; i++)
        {
            var ability = abilitys[i];
            if (ability == null) continue;

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(ability.name, GUILayout.Width(150));

            // Move Up
            GUI.enabled = i > 0;
            if (GUILayout.Button("▲", GUILayout.Width(30)))
            {
                Undo.RecordObject(selectedCharacter, "Move Ability Up");

                var temp = abilitys[i - 1];
                    abilitys[i - 1] = ability;
                abilitys[i] = temp;

                EditorUtility.SetDirty(selectedCharacter);
            }

            // Move Down
            GUI.enabled = i < abilitys.Count - 1;
            if (GUILayout.Button("▼", GUILayout.Width(30)))
            {
                Undo.RecordObject(selectedCharacter, "Move Ability Down");

                var temp = abilitys[i + 1];
                abilitys[i + 1] = ability;
                abilitys[i] = temp;

                EditorUtility.SetDirty(selectedCharacter);
            }

            GUI.enabled = true;

            // Remove
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                Undo.RecordObject(selectedCharacter, "Remove Ability");

                abilitys.RemoveAt(i);

                EditorUtility.SetDirty(selectedCharacter);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Character Ability Assignment", EditorStyles.boldLabel);

        selectedCharacter = (CharacterData)EditorGUILayout.ObjectField(
            "Select Character", selectedCharacter, typeof(CharacterData), true);

        if (selectedCharacter == null)
        {
            EditorGUILayout.HelpBox("Please select a character to assign abilities.", MessageType.Info);
            return; //Stop drawing GUI if no character is selected
        }

        EditorGUILayout.BeginHorizontal();

        // All abilities panel
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.LabelField("All Abilities", EditorStyles.boldLabel);
        abilityListScroll = EditorGUILayout.BeginScrollView(abilityListScroll);

        List<AbilitySO> abilitys = selectedCharacter.Abilities;
        if (abilitySOs != null)
        {
            foreach (var ability in abilitySOs)
            {
                if (ability == null) continue;

                if (GUILayout.Button(ability.name))
                {
                    if (!abilitys.Contains(ability))
                    {
                        Undo.RecordObject(selectedCharacter, "Add Ability"); // Support undo
                        abilitys.Add(ability);
                        EditorUtility.SetDirty(selectedCharacter);
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        // Assigned abilities panel
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.LabelField("Assigned Abilities", EditorStyles.boldLabel);
        assignedListScroll = EditorGUILayout.BeginScrollView(assignedListScroll);
        if (abilitys != null)
        {
            DrawAssignedAbilities();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}