using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCamera : BusRoute
{
    public float distancePerUnit = .1f;
    public float rotationPerUnit = 5;
    public float unitPerSpeed = 2.5f;
    private Character currentCharacter;

    private void Awake()
    {
        Sub<TurnStartEvent>(FocusCharacter);
    }

    private Coroutine focusRoutine;

    private void FocusCharacter(TurnStartEvent ev)
    {
        if (focusRoutine != null)
            StopCoroutine(focusRoutine);

        currentCharacter = ev.unit;

        focusRoutine = StartCoroutine(StartFocus());
    }

    private IEnumerator StartFocus()
    {
        float duration = 1f;
        float timeElapsed = 0f;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = new Vector3(currentCharacter.transform.position.x * distancePerUnit, 0, -10);

        Quaternion targetRot = Quaternion.Euler(0,currentCharacter.transform.position.x * rotationPerUnit,0);

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            timeElapsed += Time.deltaTime * unitPerSpeed;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}
