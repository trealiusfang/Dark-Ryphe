using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CombatQueue : MonoBehaviour
{
    private static Queue<IEnumerator> queue = new Queue<IEnumerator>();
    private static bool running = false;

    public static void Enqueue(IEnumerator routine)
    {
        queue.Enqueue(routine);

        if (!running)
            GameInitializer.instance.StartCoroutine(Process());
    }

    private static IEnumerator Process()
    {
        running = true;

        while (queue.Count > 0)
        {
            yield return GameInitializer.instance.StartCoroutine(queue.Dequeue());
        }

        running = false;
    }
}