using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math : MonoBehaviour
{
    public static int RoundValue(float value)
    {
        if (value < RoundDown(value) + .5f)
        {
            return (int)RoundDown(value);
        } else
        {
            return (int)RoundUp(value);
        }
    }

    private static float RoundDown(float value)
    {
        return ((int)value);
    }
    private static float RoundUp(float value)
    {
        float lowValue = ((int)value);
        if (lowValue == value) return value;
        else return (lowValue + 1);
    }
}
