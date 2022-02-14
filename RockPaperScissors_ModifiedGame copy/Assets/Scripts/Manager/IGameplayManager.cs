using System;

public interface IGameplayManager : IGameplayEvent
{
    /// <summary>
    /// Initialises the Game play Manager with Game Config.
    /// </summary>
    /// <param name="inGameConfig"> Game Config </param>
    void Init(GameConfig inGameConfig);

    /// <summary>
    /// To be called to Set the Gameplay.
    /// </summary>
    void SetGameplay();

    /// <summary>
    /// To be called to Update Play Timer.
    /// </summary>
    /// <param name="deltaTime">Time between frames</param>
    void UpdateTimer(float deltaTime);

    /// <summary>
    /// To be called to clean up memory.
    /// </summary>
    void Destroy();
}

public interface IGameplayEvent
{
    /// <summary>
    /// Event triggered when AI finishes Hand Selection / Throws Hand.
    /// </summary>
    event Action<int, Action<int>> OnAIHandThrown;
    
    /// <summary>
    /// Event triggered when Result is decided. i.e., Soon after user clicks/ play time expires.
    /// </summary>
    event Action<ResultType, int, int, int, bool, Action> OnResultDecided;
    
    /// <summary>
    /// Event triggered to update timer information.
    /// </summary>
    event Action<float> OnTimerUpdated;
}
