using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class ShowLoadingScreenDisposable : IDisposable
{
    private readonly LoadingScreen _loadingScreen;

    public ShowLoadingScreenDisposable(LoadingScreen loadingScreen, Camera _mainCamera)
    {
        _loadingScreen = loadingScreen;
        _loadingScreen.Show(_mainCamera);
    }

    public void SetLoadingBarPercent(float percent)
    {
        _loadingScreen.SetBarPercent(percent);
    }

    public void Dispose()
    {
        _loadingScreen.Hide();
    }
}
