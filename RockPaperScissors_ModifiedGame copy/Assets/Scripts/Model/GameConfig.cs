using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [SerializeField]
    private int _playTimer;
    
    [SerializeField]
    private List<HandInfo> _throwHands;

    [SerializeField]
    private List<ResultInfo> _results;
    
    public int PlayTimer => _playTimer;
    public List<HandInfo> ThrowHands => _throwHands;
    public int HandThrowCount => _throwHands?.Count ?? 0;
    
    public HandInfo GetHandInfo(int inHandId)
    {
        if (_throwHands == null) return null;
        var handCount = _throwHands.Count;
        for (var i = 0; i < handCount; i++)
        {
            var handInfo = _throwHands[i];
            if (handInfo?.Id == inHandId)
                return handInfo;
        }
        return null;
    }
    
    public DefeatInfo GetDefeatInfo(int inHandId, int inDefeatId)
    {
        var handInfo = GetHandInfo(inHandId);
        if (handInfo?.Defeats == null) return null;
        var defeatCount = handInfo.Defeats.Count;
        for (var i = 0; i < defeatCount; i++)
        {
            var defeatInfo = handInfo.Defeats[i];
            if (defeatInfo?.Id == inDefeatId)
                return defeatInfo;
        }
        return null;
    }
    
    public string GetResultWord(ResultType inType)
    {
        if (_results == null) return string.Empty;
        var resultCount = _results.Count;
        for (var i = 0; i < resultCount; i++)
        {
            var resultInfo = _results[i];
            if (resultInfo?.Type == inType)
                return resultInfo.Word;
        }
        return string.Empty;
    }

    public string GetDefeatDescription(int inWinnerId, int inLoserId)
    {
        var defeatInfo = GetDefeatInfo(inWinnerId, inLoserId);
        if (defeatInfo == null) return string.Empty;
        var winnerHandName = GetHandInfo(inWinnerId)?.Name;
        var loserHandName = GetHandInfo(inLoserId)?.Name;
        return $"{winnerHandName} {defeatInfo.Word} {loserHandName}";
    }
}

[Serializable]
public class HandInfo
{
    public int Id;
    public string Name;
    public Sprite Icon;
    public List<DefeatInfo> Defeats;
}

[Serializable]
public class DefeatInfo
{
    public int Id;
    public string Word;
}

[Serializable]
public class ResultInfo
{
    public ResultType Type;
    public string Word;
}
