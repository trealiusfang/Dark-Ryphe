using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private AudioService _audioService;
    [SerializeField] private GameObject Forced, UI, Background, Gameplay;
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] public GameObject _combatManagers;

    public static GameInitializer instance { get; private set; }

    private async void Start()
    {
        //Should be the only instance in the game
        if (instance == null) instance = this; else { Destroy(gameObject); return; }

        BindObjects();

        using (var loadingScreenDisposible = new ShowLoadingScreenDisposable(_loadingScreen, _mainCamera))
        {
            loadingScreenDisposible.SetLoadingBarPercent(0f);
            await Initialization();
            loadingScreenDisposible.SetLoadingBarPercent(.33f);
            await CreateObjects();
            loadingScreenDisposible.SetLoadingBarPercent(.66f);
            PrepareGame();
            loadingScreenDisposible.SetLoadingBarPercent(1f);
            await UniTask.Delay(TimeSpan.FromSeconds(.2f), DelayType.DeltaTime, PlayerLoopTiming.Update);
        }

        await BeginGame();
    }

    private void BindObjects()
    {
        _loadingScreen = Instantiate(_loadingScreen);
        _mainCamera = Instantiate(_mainCamera); 
        _audioService = Instantiate(_audioService);
        _combatManagers = Instantiate(_combatManagers);
    }

    private async UniTask Initialization()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(.1f), DelayType.DeltaTime, PlayerLoopTiming.Update);
        _audioService.Initialize();
        //Services like analytics or new input system should be enabled here
        // await inputSystem.Enabled;
    }

    private async UniTask CreateObjects()
    {
        Forced = Instantiate(Forced);
        UI = Instantiate(UI);
        Background = Instantiate(Background);
        await UniTask.Delay(TimeSpan.FromSeconds(.2f), DelayType.DeltaTime, PlayerLoopTiming.Update);
        Gameplay = Instantiate(Gameplay);
    }

    private void PrepareGame()
    {
        _mainCamera.transform.parent = Forced.transform;
        _loadingScreen.transform.SetParent(UI.transform);
        for (int i = 0; i < UI.transform.childCount; i++) { UI.transform.GetChild(i).GetComponent<Canvas>().worldCamera = _mainCamera; }
    }

    private async UniTask BeginGame()
    {
        _audioService.PlayMusic("Battle Theme");
        await UniTask.Delay(TimeSpan.FromSeconds(.2f), DelayType.DeltaTime, PlayerLoopTiming.Update);
        _combatManagers.GetComponent<CombatManager>().StartCombat();
    }
}
