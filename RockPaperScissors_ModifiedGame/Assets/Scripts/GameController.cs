using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameConfig _gameConfig;
    
    [SerializeField]
    private GameScreenView _gameScreenView;

    private IGameplayManager _gameplayManager;
    private bool _isInitialised;
    
    private void Awake()
    {
        if(_gameConfig == null) Debug.LogError("No Game Config provided!");
        
        _gameplayManager = GameplayManager.Instance();
        _gameplayManager.Init(_gameConfig);
        _gameScreenView.Init(_gameConfig, _gameplayManager, QuitGame);
        _isInitialised = true;
    }

    private void Start()
    {
        if (_isInitialised)
        {
            _gameScreenView.SetMenuScreen(_gameplayManager.SetGameplay);
        }
    }

    private void Update()
    {
        if (_isInitialised)
        {
            var deltaTime = Time.deltaTime;
            _gameplayManager.UpdateTimer(deltaTime);
        }
    }

    private void OnDestroy()
    {
        _isInitialised = false;
        _gameplayManager.Destroy();
    }
    
    private void QuitGame()
    {
        _isInitialised = false;
        Application.Quit();
    }
}
