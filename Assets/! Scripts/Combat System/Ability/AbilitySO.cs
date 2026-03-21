
using UnityEngine;

[CreateAssetMenu(fileName = "unassigned_ability", menuName = "AbilitySO")]
public class AbilitySO : ScriptableObject
{
    public Sprite abilitySprite;
    public AudioClip abilitySuccessClip;
    [Header("Optional")]
    public AudioClip abilityAlternativeClip; 
    public AnimationClip abilityEffectClip; //If an ability cast is a ranged spell for example, the spell animation that spawns of on targets are put here.
}
