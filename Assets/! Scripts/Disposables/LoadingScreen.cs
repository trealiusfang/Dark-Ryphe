using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject holder;

    public void Show(Camera _mainCamera)
    {
        player.targetCamera = _mainCamera;
        holder.SetActive(true);
    }

    public void SetBarPercent(float percent)
    {
        slider.value = percent;
    }

    public void Hide()
    {
        holder.SetActive(false);
    }
}
