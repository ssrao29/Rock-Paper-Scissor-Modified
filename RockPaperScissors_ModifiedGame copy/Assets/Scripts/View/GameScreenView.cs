using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenView : MonoBehaviour
{
    private const string TRIGGER_SHOW_RESULT = "Show_Result";
    
    [SerializeField] private Animator _screenAnimator;
    [SerializeField] private Button _closeButton;
    
    [Header("Menu")]
    [SerializeField] private GameObject _menuScreen;
    [SerializeField] private Button _playButton;
    
    [Header("Gameplay")]
    [SerializeField] private GameObject _gameplayScreen;
    [SerializeField] private HandView _computerHand;
    [SerializeField] private List<HandView> _playerHands;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Slider _timer;

    [Header("Result")]
    [SerializeField] private Text _resultText;
    
    private GameConfig _gameConfig;
    private IGameplayEvent _gameplayEvent;
    private Action<int> _onPlayerSelectHand;
    private Action _onResultShown;
    private bool _isGameEnded;
    
    public void Init(GameConfig inGameConfig, IGameplayEvent inGameplayEvent, Action inQuitGameCallback)
    {
        _gameConfig = inGameConfig;
        _gameplayEvent = inGameplayEvent;
        
        _gameplayEvent.OnAIHandThrown += SetGameplayScreen;
        _gameplayEvent.OnResultDecided += SetResultScreen;
        _gameplayEvent.OnTimerUpdated += UpdatePlayTimer;
        
        if (_closeButton != null && inQuitGameCallback != null)
        {
            _closeButton.onClick.AddListener(inQuitGameCallback.Invoke);
        }
    }

    public void SetMenuScreen(Action inPlayClickCallback = null)
    {
        if (_menuScreen != null)
        {
            _menuScreen.SetActive(true);
        }
        if (_gameplayScreen != null)
        {
            _gameplayScreen.SetActive(false);
        }

        if (_playButton != null && inPlayClickCallback != null)
        {
            _playButton.onClick.AddListener(inPlayClickCallback.Invoke);
        }
    }

    private void SetGameplayScreen(int inAIHand, Action<int> inPlayerThrowsHand)
    {
        _onPlayerSelectHand = inPlayerThrowsHand;
        if (_menuScreen != null)
        {
            _menuScreen.SetActive(false);
        }
        if (_gameplayScreen != null)
        {
            _gameplayScreen.SetActive(true);
        }
        PopulateHands();
        SetAIHand(inAIHand);
    }
    
    private void PopulateHands()
    {
        if (_playerHands == null || _gameConfig.ThrowHands == null) return;
        var handCount = _gameConfig.ThrowHands.Count;
        var viewCount = _playerHands.Count;
        if (viewCount > 0 && viewCount == handCount)
        {
            for (int i = 0; i < viewCount; i++)
            {
                _playerHands[i]?.Init(_gameConfig.ThrowHands[i], _onPlayerSelectHand);
            }
        }
    }

    private void SetAIHand(int inHandId)
    {
        if (_computerHand == null) return;
        _computerHand.Init(_gameConfig.GetHandInfo(inHandId));
    }

    private void UpdatePlayTimer(float inTimer)
    {
        if (_timer != null)
        {
            _timer.value = inTimer / _gameConfig.PlayTimer;
        }
    }
    
    private void SetResultScreen(ResultType inType, int inPlayerHandId, int inAIHandId, 
        int inScore, bool inIsGameEnded, Action inOnResultShown)
    {
        if (_resultText == null) return;

        var resultWord = _gameConfig.GetResultWord(inType);
        var defeatDesc = string.Empty;
        if (inType == ResultType.eWin)
        {
            defeatDesc = _gameConfig.GetDefeatDescription(inPlayerHandId, inAIHandId);
        }
        else if (inType == ResultType.eLose)
        {
            defeatDesc = _gameConfig.GetDefeatDescription(inAIHandId, inPlayerHandId);
        }

        _resultText.text = $"<color='#00ff00'>{resultWord}</color>\n{defeatDesc}";
        _isGameEnded = inIsGameEnded;
        _onResultShown = inOnResultShown;
        
        if(_screenAnimator != null)
            _screenAnimator.SetTrigger(TRIGGER_SHOW_RESULT);

        if (_scoreText != null)
            _scoreText.text = $"Score: {inScore}";
    }

    /* Result Shown Complete - Animation Event */
    private void OnResultShown()
    {
        if (_isGameEnded)
        {
            _isGameEnded = false;
            SetMenuScreen();
        }
        _onResultShown?.Invoke();
    }
    
    private void OnDestroy()
    {
        _gameplayEvent.OnAIHandThrown -= SetGameplayScreen;
        _gameplayEvent.OnResultDecided -= SetResultScreen;
        _gameplayEvent.OnTimerUpdated -= UpdatePlayTimer;
    }
}
