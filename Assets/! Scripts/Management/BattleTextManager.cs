using PixelBattleText;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

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


    private void BattleTextEvent(BattleTextEvent ev)
    {
        Vector2 totalOffset = new Vector2(UnityEngine.Random.value + offset.x, UnityEngine.Random.value * 2 + offset.y);
        Vector2 CharacterPosition = Vector2.zero;
        
        if (ev.character != null)
        {
            CharacterPosition = GameInitializer.instance._combatManagers.GetComponent<CombatPositioner>().getPosition(ev.character);
        }

        DisplayText(ev.text, ev.position + CharacterPosition + totalOffset, ev.textAnimType);
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));
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
                textSpawnPosition + new Vector3(0, 1, 0));

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
            textSpawnPosition + new Vector3(0, 1, 0));

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
            textSpawnPosition + new Vector3(0, 1, 0));

    }
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

