using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatEffectSpawner : BusRoute
{
    public GameObject effectPrefab;

    private void Awake()
    {
        Sub<BattleEffectEvent>(SpawnEffectObject);
    }

    public void SpawnEffectObject(BattleEffectEvent ev)
    {
        AnimationClip clip = ev.effectAnimation; Vector3 position = ev.position;
        Vector2 size = new Vector2(ev.effectSize, ev.effectSize); 

        GameObject baseObject = Instantiate(effectPrefab, position, Quaternion.identity, transform);
        baseObject.transform.localScale = size;

        Animator animator = baseObject.GetComponentInChildren<Animator>();
        AnimationClipOverrides clipOverrides;
        AnimatorOverrideController animatorController;


        animatorController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        clipOverrides = new AnimationClipOverrides(animatorController.overridesCount);

        animator.runtimeAnimatorController = animatorController;

        animatorController.GetOverrides(clipOverrides);

        clipOverrides["Effect_Base"] = clip;
        animatorController.ApplyOverrides(clipOverrides);

        Destroy(baseObject, clip.length);
    }
}
