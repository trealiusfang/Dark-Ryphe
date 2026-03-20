using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterRenderer : MonoBehaviour
{
    [Header("Needs to be applied")]
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Animator movementAnimator;
    [SerializeField] private Animator actionAnimator;
    [Header("From character data")]
    [SerializeField] private List<AnimationClip> AttackAnimations;
    [SerializeField] private List<CharacterAnimationAndFrequency> IdleAnimations;
    [SerializeField] private List<CharacterAnimationAndFrequency> HurtAnimations;
    [Header("Optional")]
    [SerializeField] private List<CharacterAnimationAndFrequency> DeathAnimations;

    private Character character;
    private Material material;

    private AnimationClipOverrides movementOverrides;
    private AnimationClipOverrides actionOverrides;
    private AnimatorOverrideController movementController;
    private AnimatorOverrideController actionController;

    private void Awake()
    {
        SetRenderer();
    }

    void SetRenderer()
    {
        character = GetComponent<Character>();

        if (spriteRenderer == null || movementAnimator == null || actionAnimator == null)
        {
            Debug.LogError("Components are missing for " + character.charData.characterName + "'s renderer!");
            return;
        }

        //Set char data
        CharacterData data = character.charData;
        spriteRenderer.sprite = data.baseSprite;
        material = spriteRenderer.material;

        AttackAnimations = data.AttackAnimations;
        IdleAnimations = data.IdleAnimations;
        HurtAnimations = data.HurtAnimations;
        DeathAnimations = data.DeathAnimations;

        //Set controllers
        movementController = new AnimatorOverrideController(movementAnimator.runtimeAnimatorController);
        actionController = new AnimatorOverrideController(actionAnimator.runtimeAnimatorController);

        //Set overrides
        movementOverrides = new AnimationClipOverrides(movementController.overridesCount);
        actionOverrides = new AnimationClipOverrides(actionController.overridesCount);

        movementAnimator.runtimeAnimatorController = movementController;
        actionAnimator.runtimeAnimatorController = actionController;

        movementController.GetOverrides(movementOverrides);
        actionController.GetOverrides(actionOverrides);

        ApplyOverrides(actionController, actionOverrides);
    }

    private void ApplyOverrides(AnimatorOverrideController controller, AnimationClipOverrides clipOverrides)
    {
        if (AttackAnimations.Count > 0) clipOverrides["AttackState"] = AttackAnimations[0];
        if (IdleAnimations.Count > 0) clipOverrides["IdleState"] = IdleAnimations[0].clip;
        if (HurtAnimations.Count > 0) clipOverrides["HurtState"] = HurtAnimations[0].clip;
        if (DeathAnimations.Count > 0) clipOverrides["DeathState"] = DeathAnimations[0].clip;

        controller.ApplyOverrides(clipOverrides);
    }


    public void PlayActionAnimation(CharAnimationType type, int num)
    {
        AnimationClip clip = null;

        switch (type)
        {
            case CharAnimationType.Idle:
                if (num > IdleAnimations.Count || IdleAnimations.Count == 0) num = IdleAnimations.Count - 1;
                if (num < 0) return;
                clip = IdleAnimations[num].clip;
                break;
            case CharAnimationType.Attack:
                if (num > AttackAnimations.Count || AttackAnimations.Count == 0) num = AttackAnimations.Count - 1;
                if (num < 0) return;
                clip = AttackAnimations[num];
                break;
            case CharAnimationType.Hurt:
                if (num > HurtAnimations.Count || HurtAnimations.Count == 0) num = HurtAnimations.Count - 1;
                if (num < 0) return;
                clip = HurtAnimations[num].clip;
                break;
            case CharAnimationType.Death:
                if (num > DeathAnimations.Count || DeathAnimations.Count == 0) num = DeathAnimations.Count - 1;
                if (num < 0) return;
                clip = DeathAnimations[num].clip;
                break;
        }

        string _type = type.ToString();
        if (type != CharAnimationType.Idle) clip.wrapMode = WrapMode.Once;

        actionOverrides[_type + " State"] = clip;
        actionController.ApplyOverrides(actionOverrides);

        actionAnimator.SetTrigger(_type);
    }
    public IEnumerator DeathVisuals()
    {
        yield return null;

        PlayRandomActionClip(CharAnimationType.Death);
        EventBus.Raise(new SFXEvent { sfx_string = "unit death" });
        float timeElapsed = 0;
        while (material.GetFloat("_Fade") > 0)
        {
            material.SetFloat("_Fade", 1 - timeElapsed);
            timeElapsed += .05f;
            yield return new WaitForSeconds(.05f);
        }
    }

    public void PlayRandomActionClip(CharAnimationType type)
    {
        AnimationClip clip = null;

        if (type == CharAnimationType.Idle)
        {
            clip = RandomClipByFrequency(IdleAnimations);
        }
        if (type == CharAnimationType.Attack)
        {
            int r = Random.Range(0, AttackAnimations.Count - 1);
            clip = AttackAnimations[r];
        }
        if (type == CharAnimationType.Hurt)
        {
            clip = RandomClipByFrequency(HurtAnimations);
        }
        if (type == CharAnimationType.Death)
        {
            if (DeathAnimations.Count > 0)
            clip = RandomClipByFrequency(DeathAnimations);
            else clip = RandomClipByFrequency(HurtAnimations);
        }

        if (clip == null) return;
         
        string _type = type.ToString();
        if (type != CharAnimationType.Idle) clip.wrapMode = WrapMode.Once;

        actionOverrides[_type + " State"] = clip;
        actionController.ApplyOverrides(actionOverrides);

        actionAnimator.SetTrigger(_type);
    }

    private AnimationClip RandomClipByFrequency(List<CharacterAnimationAndFrequency> charAndFreqs)
    {
        if (charAndFreqs == null || charAndFreqs.Count == 0) { return null; }
        float frequencySum = 0;
        List<float> frequencies = new List<float>();
        foreach (CharacterAnimationAndFrequency cf in charAndFreqs)
        {
            frequencySum += cf.frequency;
            frequencies.Add(cf.frequency);
        }

        if (frequencySum == 0)
        {
            return charAndFreqs[0].clip;
        }

        for (int i = 0; i < frequencies.Count; i++)
        {
            frequencies[i] /= frequencySum;
        }
        
        float r = Random.value;

        for (int i = 0; i < frequencies.Count; i++)
        {                                                                             //if r is equal to 1 returns the first animation.
            if (r < freqSigma(frequencies, i - 1) && r <= freqSigma(frequencies, i) || r == 1)
            {
                return charAndFreqs[i].clip;
            }
        }

        return null;
    }

    float freqSigma(List<float> list, int endInteger)
    {
        float sum = 0;
        for (int i = 0; i < endInteger; i++)
        {
            sum += list[i];
        }

        return sum;
    }
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}
