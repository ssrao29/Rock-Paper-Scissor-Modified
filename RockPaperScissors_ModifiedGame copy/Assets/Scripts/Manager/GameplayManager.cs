using System;

public enum ResultType
{
    eTie = 0,
    eWin,
    eLose
}

public class GameplayManager : IGameplayManager
{
    private static IGameplayManager _instance;
    private GameConfig _gameConfig;

    public event Action<int, Action<int>> OnAIHandThrown;
    public event Action<ResultType, int, int, int, bool, Action> OnResultDecided;
    public event Action<float> OnTimerUpdated;
    
    private int _playerHand;
    private int _AIHand;
    private int _gameScore;
    private bool _isGameEnded;
    private float _playTimer;
    
    public static IGameplayManager Instance()
    {
        return _instance ?? (_instance = new GameplayManager());
    }

    public void Init(GameConfig inGameConfig)
    {
        _gameConfig = inGameConfig;
    }
    
    public void SetGameplay()
    {
        Reset();
        GenerateAIThrowHand();
        StartTimer();
    }

    private void StartTimer()
    {
        _playTimer = _gameConfig.PlayTimer;
    }
    
    public void UpdateTimer(float deltaTime)
    {
        if (_playTimer > 0)
        {
            _playTimer -= deltaTime;
            if (_playTimer < 0)
            {
                _playTimer = 0;
                OnStopTimer();
            }
            OnTimerUpdated?.Invoke(_playTimer);
        }
    }

    private void StopTimer()
    {
        if (_playTimer > 0)
        {
            _playTimer = 0;
        }
    }

    private void OnStopTimer()
    {
        StopTimer();
        if (_playerHand <= 0)
        {
            _isGameEnded = true;
            OnResultDecided?.Invoke(ResultType.eLose, _playerHand, _AIHand, _gameScore, _isGameEnded, UpdateData);
        }
    }
    
    private void OnPlayerThrowsHand(int inHandId)
    {
        if (_playerHand <= 0)
        {
            _playerHand = inHandId;
            UpdateResult();
            StopTimer();
        }
    }
    
    private void GenerateAIThrowHand()
    {
        var handCount = _gameConfig.HandThrowCount;
        if (_AIHand > 0 || handCount <= 0) return;
        _AIHand = _gameConfig.ThrowHands[UnityEngine.Random.Range(0, handCount)]?.Id ?? 0;
        OnAIHandThrown?.Invoke(_AIHand, OnPlayerThrowsHand);
    }

    private void UpdateResult()
    {
        if (_playerHand <= 0 || _AIHand <= 0) return;
        ResultType result;
        if (_playerHand == _AIHand)
        {
            result = ResultType.eTie;
        }
        else
        {
            var defeatInfo = _gameConfig.GetDefeatInfo(_playerHand, _AIHand);
            result = (defeatInfo == null) ? ResultType.eLose : ResultType.eWin;
        }

        if (result == ResultType.eWin) _gameScore += 1;
        _isGameEnded = result == ResultType.eLose;
        OnResultDecided?.Invoke(result, _playerHand, _AIHand, _gameScore, _isGameEnded, UpdateData);
    }

    private void UpdateData()
    {
        if (_isGameEnded)
        {
            _gameScore = 0;
            Reset();
        }
        else
        {
            SetGameplay();
        }
    }
    
    private void Reset()
    {
        _playerHand = 0;
        _AIHand = 0;
        _isGameEnded = false;
    }
    
    public void Destroy()
    {
        Reset();
        _instance = null;
    }
}
