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
        DisplayText(ev.text, ev.position + totalOffset, ev.textAnimType);
    }

    private void DisplayText(string text, Vector2 position, TextAnimType textAnimType)
    {
        textSpawnPosition = position;
        textText = text;    
        switch(textAnimType)
        {
            case TextAnimType.KO:
                DisplayKO();
                break;
            case TextAnimType.LvlUp:
                DisplayLvlUp();
                break;
            case TextAnimType.Premium:
                DisplayPremium();
                break;
            case TextAnimType.Spooky:
                DisplaySpooky();
                break;
            case TextAnimType.Venom:
                DisplayVenom();
                break;
            case TextAnimType.pyro:
                DisplayPyro();
                break;
            case TextAnimType.Shock:
                DisplayShock();
                break;
            case TextAnimType.Freeze:
                DisplayFreeze();
                break;
            case TextAnimType.Heal:
                DisplayHeal();
                break;
            case TextAnimType.Critical:
                DisplayCrit();
                break;
            default: DisplayDamage(); break;
        }
    }
    public void DisplayPremium()
    {
        PixelBattleTextController.DisplayText(
            textText,
            premium,
            textSpawnPosition);

    }

    public void DisplaySpooky()
    {
        PixelBattleTextController.DisplayText(
            textText,
            spooky,
            textSpawnPosition);

    }

    public void DisplayPyro()
    {
        PixelBattleTextController.DisplayText(
            textText,
            pyro,
            textSpawnPosition);

    }

    public void DisplayMetallic()
    {
        PixelBattleTextController.DisplayText(
            textText,
            metallic,
            textSpawnPosition);

    }

    public void DisplayFreeze()
    {
        PixelBattleTextController.DisplayText(
            textText,
            freeze,
            textSpawnPosition);

    }

    public void DisplayShock()
    {
        PixelBattleTextController.DisplayText(
            textText,
            shock,
            textSpawnPosition);

    }

    public void DisplayLvlUp()
    {
        PixelBattleTextController.DisplayText(
            textText,
            lvlUp,
            textSpawnPosition);

    }

    public void DisplayDamage()
    {
        PixelBattleTextController.DisplayText(
            textText,
            damage,
            textSpawnPosition);

    }

    public void DisplayKO()
    {
        PixelBattleTextController.DisplayText(
            textText,
            ko,
            textSpawnPosition);

    }

    public void DisplayVenom()
    {
        PixelBattleTextController.DisplayText(
            textText,
            venom,
            textSpawnPosition);

    }

    public void DisplayHeal()
    {
        PixelBattleTextController.DisplayText(
            textText,
            heal,
            textSpawnPosition);

    }

    public void DisplayCrit()
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

