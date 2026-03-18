using System.Collections;
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
    [SerializeField] private List<Button> AbilityButtons;
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

        CharacterImage.sprite = character.charData.charSprite;

        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            if (AbilityButtons[i] == null) continue;
            Debug.Log("c: " + character.name + ", a: " + character.getAllAbilities().Count);
            if (i < character.getAllAbilities().Count)
            {
                Debug.Log(i + " Ý UHHH," + character.name);
                if (!AbilityButtons[i].gameObject.activeSelf) AbilityButtons[i].gameObject.SetActive(true);
                AbilityButtons[i].GetComponent<Image>().sprite = character.getAllAbilities()[i].sprite;
                AbilityButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = character.getAllAbilities()[i].abilityName;
            } else
            {
                AbilityButtons[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < StatTexts.Count; i++)
        {
            if (StatTexts[i] == null) continue;

            StatTexts[i].text = i == 0 ? character.baseStats.power.ToString() :
                i == 1 ? character.currentStats.currentHP + "/" + character.baseStats.maxHP :
                i == 2 ? character.currentStats.currentMana + "/" + character.baseStats.maxMana :
                i == 3 ? character.baseStats.luck.ToString() :
                i == 4 ? character.baseStats.speed.ToString() :
                i == 5 ? character.baseStats.manaRegen.ToString() : "";
        }

        for (int i = 0; i < Passives.Count; i++)
        {
            //Passives[i].GetComponent...
        }
    }
}
