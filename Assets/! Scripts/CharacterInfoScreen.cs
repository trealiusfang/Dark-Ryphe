using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoScreen : BusRoute
{
    [Header("Main")]
    [SerializeField] private GameObject InfoPanel;
    [Header("Vars")]
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI AlternativeInfo;
    [SerializeField] private Image CharacterImage;
    [SerializeField] private List<AbilityButton> AbilityButtons;
    [SerializeField] private List<GameObject> Passives;
    [SerializeField] private List<TextMeshProUGUI> StatTexts;
    [Header("Debug")]
    [SerializeField] private Character TestCharacter;
    private void Awake()
    {
        InfoPanel.SetActive(false);
        Sub<InspectedCharacterEvent>(SetScreen);
    }

    private void SetScreen(InspectedCharacterEvent ev)
    {
        InfoPanel.SetActive(true);

        Character character = ev.character; 

        Name.text = character.charData.characterName;
        AlternativeInfo.text = character.charData.alternativeInfo;

        CharacterImage.sprite = character.charData.baseSprite;

        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            if (AbilityButtons[i] == null) continue;
            Debug.Log("c: " + character.name + ", a: " + character.getAllAbilities().Count);
            if (i < character.getAllAbilities().Count)
            {
                if (!AbilityButtons[i].gameObject.activeSelf) AbilityButtons[i].gameObject.SetActive(true);
                AbilityButtons[i].SetAbilityInfo(character.getAllAbilities()[i], character);
            } else
            {
                AbilityButtons[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < StatTexts.Count; i++)
        {
            if (StatTexts[i] == null) continue;

            StatTexts[i].text = i == 0 ? character.GetBaseStat(statType.Power).ToString() :
                i == 1 ? character.GetBaseStat(statType.HP_Current) + "/" + character.GetBaseStat(statType.HP_Max) :
                i == 2 ? character.GetBaseStat(statType.Mana_Current) + "/" + character.GetBaseStat(statType.Mana_Max) :
                i == 3 ? character.GetBaseStat(statType.Luck).ToString() :
                i == 4 ? character.GetBaseStat(statType.Speed).ToString() :
                i == 5 ? character.GetBaseStat(statType.Mana_Regen).ToString() : "";
        }

        List<Passive> passives = character.passiveHolder.GetPassives();
        for (int i = 0; i < Passives.Count; i++)
        {
            if (i < passives.Count)
            {
                Passives[i].SetActive(true);
                Passives[i].transform.GetChild(0).GetComponentInChildren<Image>().sprite = passives[i].sprite;
            } else
            {
                Passives[i].SetActive(false);
            }
        }
    }
}
