using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorComponent : MonoBehaviour
{
    [SerializeField] private SelectorType selectorType;
    [SerializeField] private List<sTypeAndColor> typesAndColors = new List<sTypeAndColor>();
    private Image image;
    Character character;


    public void SetSelector(SelectorType type, Character _character)
    {
        selectorType = type;
        foreach (var typeAndColor in typesAndColors)
        {
            if (typeAndColor.selectorType == selectorType)
            {
                buttonImage().color = typeAndColor.color;
                break;
            }
        }

        character = _character;
    }

    public void FireAbility()
    {
        if (selectorType != SelectorType.Self)
        EventBus.Raise(new TargetSelectedEvent { target = character});
    }

    private Image buttonImage()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            return image;
        } else
        {
            return image;
        }
    }
}

[Serializable]
public class sTypeAndColor
{
    public SelectorType selectorType;
    public Color color;
}
