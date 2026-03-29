using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDescriptionHandler : BusRoute
{
    private Character currentCharacter;
    private Ability hoveredAbility;
    private Canvas canvas;
    [Header("Necessities")]
    [SerializeField] private GameObject ExplainerGameObject;
    [SerializeField] private TextMeshProUGUI AbilityTitle;
    [SerializeField] private TextMeshProUGUI AbilityDescription;
    [SerializeField] private RectTransform TargetGroup1;
    [SerializeField] private RectTransform TargetGroup2;
    [Header("Costumizations")]
    public Vector2 explainerOffset;
    [ColorUsage(true,true)] 
    public Color Enemy;
    [ColorUsage(true, true)]
    public Color Activation;
    [ColorUsage(true, true)]
    public Color Allies;
    [ColorUsage(true, true)]
    public Color Empty;
    private void Awake()
    {
        canvas = transform.parent.GetComponent<Canvas>();
        DisableAbilityExplainer();
        Sub<AbilityHoverEvent>(HoverControlling);
    }
    void Update()
    {
        Vector2 mousePos = GameInputManager.mousePosition();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        ExplainerGameObject.GetComponent<RectTransform>().localPosition = localPoint + explainerOffset;
    }

    private void HoverControlling(AbilityHoverEvent ev)
    {
        if (!ev.intoHover)
        {
            DisableAbilityExplainer();
            return;
        }
        currentCharacter = ev.character;
        hoveredAbility = ev.ability;

        SetAbilityExplainer();
    }

    private void SetAbilityExplainer()
    {
        if (currentCharacter == null) return;
        ExplainerGameObject.SetActive(true);

        AbilityTitle.text = hoveredAbility.abilityName;
        SetTargetGroups();
        AbilityDescription.text = hoveredAbility.GetAbilityDescription(currentCharacter);
    }

    private void DisableAbilityExplainer()
    {
        ExplainerGameObject.SetActive(false);
    }

    private void SetTargetGroups()
    {
        if (hoveredAbility == null) return;

        TargetType type = hoveredAbility.targetType;
        short[] targetSpots = hoveredAbility.targetSpots;
        short[] activationSpots = hoveredAbility.activasionSpots;
        bool towardEnemies = type == TargetType.SingleEnemy || type == TargetType.AoEEnemy ? true : false;
        bool isAOE = type == TargetType.AoEEnemy || type == TargetType.AoEAlly || type == TargetType.AoEAll ? true : false;

        if (isntFull(activationSpots) && type != TargetType.Self)
        {
            TargetGroup1.gameObject.SetActive(true);
            TargetGroup2.gameObject.SetActive(true);

            TargetGroup1.localPosition = new Vector3(-75f, 0, 0);
            setGroupColors(TargetGroup1, Activation, activationSpots);

            TargetGroup2.localPosition = new Vector3(75f, 0, 0);
            setGroupColors(TargetGroup2, towardEnemies ? Enemy : Allies, targetSpots);
        }
        else if (type == TargetType.AoEAll)
        {
            TargetGroup1.gameObject.SetActive(true);
            TargetGroup2.gameObject.SetActive(true);

            TargetGroup1.localPosition = new Vector3(-75f, 0, 0);
            setGroupColors(TargetGroup1, Allies, targetSpots);

            TargetGroup2.localPosition = new Vector3(75f, 0, 0);
            setGroupColors(TargetGroup2, Allies, targetSpots);
        } else if (type == TargetType.Self)
        {
            TargetGroup1.gameObject.SetActive(false);
            TargetGroup2.gameObject.SetActive(false);
        } else
        {
            TargetGroup1.gameObject.SetActive(true);
            TargetGroup2.gameObject.SetActive(false);

            TargetGroup1.localPosition = Vector3.zero;
            setGroupColors(TargetGroup1, towardEnemies ? Enemy : Allies, targetSpots);
        }
    }

    private void setGroupColors(RectTransform transform, Color color, short[] shorts)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < 4)
                if (shorts[i] == 1 && transform.GetChild(i).Find("fill") != null)
                {
                    transform.GetChild(i).Find("fill").GetComponent<Image>().color = color;
                }
                else if (transform.GetChild(i).Find("fill") != null)
                {
                    transform.GetChild(i).Find("fill").GetComponent<Image>().color = Empty;
                }
        }
    }

    bool isntFull(short[] shorts)
    {
        bool f = false;

        for (int i = 0; i < shorts.Length; i++)
        {
            if (shorts[i] != 1)
            {
                f = true; break;
            }
        }

        return f;
    }
}
