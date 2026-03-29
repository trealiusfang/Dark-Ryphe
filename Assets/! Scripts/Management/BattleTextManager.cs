using PixelBattleText;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BattleTextManager : BusRoute
{
    private void Awake()
    {
        Sub<BattleTextEvent>(BattleTextEvent);
    }
    public TextAnimation ko;
    public TextAnimation lvlUp;
    public TextAnimation premium;
    public TextAnimation spooky;
    public TextAnimation venom;

    public TextAnimation pyro;
    public TextAnimation shock;
    public TextAnimation freeze;

    public TextAnimation metallic;
    public TextAnimation critical;
    public TextAnimation damage;
    public TextAnimation heal;

    public TMP_InputField input;

    private Vector3 textSpawnPosition = new Vector3(.5f, .65f, 0);
    private string textText;
    public Vector2 offset;
    private Vector3 criticalOffset = new Vector3(0, 3);

    private List<textInfo> textInfos = new List<textInfo>();

    private void BattleTextEvent(BattleTextEvent ev)
    {
        textInfo newTextInfo = new textInfo();
        newTextInfo.text = ev.text;
        newTextInfo.character = ev.character;
        newTextInfo.isCrit = ev.isCrit;
        newTextInfo.position = ev.position;
        newTextInfo.textAnimType = ev.textAnimType;
        newTextInfo.value = ev.value;

        bool found = false;
        for (int i = 0; i < textInfos.Count; i++)
        {
            if (textInfos[i].text == newTextInfo.text)
            {
                if (textInfos[i].character != null && textInfos[i].character == newTextInfo.character)
                {
                    textInfos[i].value += ev.value;
                    found = true;
                } else if (textInfos[i].character == null && newTextInfo.character == null && textInfos[i].position == newTextInfo.position)
                {
                    textInfos[i].value += ev.value;
                    found = true;
                }
            }
        }

        if (!found)
        {
            textInfos.Add(newTextInfo);
        }
    }

    private IEnumerator Start()
    {
        while(true)
        {
            while(textInfos.Count > 0)
            {
                yield return new WaitForSeconds(.05f);
                setAndFireText(textInfos[0]);
                textInfos.RemoveAt(0);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void setAndFireText(textInfo textInfo)
    {
        Vector2 totalOffset = new Vector2(UnityEngine.Random.value + offset.x, UnityEngine.Random.value * 2 + offset.y);
        Vector2 CharacterPosition = Vector2.zero;

        if (textInfo.character != null)
        {
            CharacterPosition = GameInitializer.instance._combatManagers.GetComponent<CombatPositioner>().getPosition(textInfo.character);
        }

        textInfo.text = textInfo.text.Replace("/value", textInfo.value.ToString());

        DisplayText(textInfo.text, textInfo.position + CharacterPosition + totalOffset, textInfo.textAnimType);
    }

    private void DisplayText(string text, Vector2 position, TextAnimType textAnimType, bool isCrit = false)
    {
        textSpawnPosition = position;
        textText = text;    
        switch(textAnimType)
        {
            case TextAnimType.KO:
                DisplayKO(isCrit);
                break;
            case TextAnimType.LvlUp:
                DisplayLvlUp(isCrit);
                break;
            case TextAnimType.Premium:
                DisplayPremium(isCrit);
                break;
            case TextAnimType.Spooky:
                DisplaySpooky(isCrit);
                break;
            case TextAnimType.Venom:
                DisplayVenom(isCrit);
                break;
            case TextAnimType.pyro:
                DisplayPyro(isCrit);
                break;
            case TextAnimType.Shock:
                DisplayShock(isCrit);
                break;
            case TextAnimType.Freeze:
                DisplayFreeze(isCrit);
                break;
            case TextAnimType.Heal:
                DisplayHeal(isCrit);
                break;
            case TextAnimType.Critical:
                DisplayCrit(isCrit);
                break;
            default: DisplayDamage(isCrit); break;
        }
    }
    public void DisplayPremium(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            premium,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                premium,
                textSpawnPosition + criticalOffset);
    }

    public void DisplaySpooky(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            spooky,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                spooky,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayPyro(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            pyro,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                pyro,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayMetallic(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            metallic,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                metallic,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayFreeze(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            freeze,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                freeze,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayShock(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            shock,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                shock,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayLvlUp(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            lvlUp,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                lvlUp,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayDamage(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            damage,
            textSpawnPosition);
    }

    public void DisplayKO(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            ko,
            textSpawnPosition);

        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                ko,
                textSpawnPosition + criticalOffset);
    }

    public void DisplayVenom(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            venom,
            textSpawnPosition);
        if (isCrit)
            PixelBattleTextController.DisplayText(
                "CRITICAL!",
                venom,
                textSpawnPosition + criticalOffset);

    }

    public void DisplayHeal(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            heal,
            textSpawnPosition);

        if (isCrit)
        PixelBattleTextController.DisplayText(
            "CRITICAL!",
            heal,
            textSpawnPosition + criticalOffset);

    }

    public void DisplayCrit(bool isCrit)
    {
        PixelBattleTextController.DisplayText(
            textText,
            critical,
            textSpawnPosition);

        PixelBattleTextController.DisplayText(
            "CRITICAL!",
            critical,
            textSpawnPosition + criticalOffset);

    }
}

[Serializable]
public class textInfo
{
    public string text;
    public int value;
    public Vector2 position;
    public Character character;
    public TextAnimType textAnimType;
    public bool isCrit;
}

public enum TextAnimType
{
    KO,
    LvlUp,
    Premium,
    Spooky,
    Venom,
    pyro,
    Shock,
    Freeze,
    Metallic,
    Critical,
    Damage,
    Heal,
}

