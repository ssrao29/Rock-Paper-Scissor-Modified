using System;
using UnityEngine;
using UnityEngine.UI;

public class HandView : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _actionButton;

    private int _handId;
    
    public void Init(HandInfo inHandInfo, Action<int> inClickCallback = null)
    {
        if (inHandInfo == null) return;
        _handId = inHandInfo.Id;
        SetName(inHandInfo.Name);
        SetIcon(inHandInfo.Icon);
        SetActionButton(inClickCallback);
    }

    private void SetName(string inName)
    {
        if (string.IsNullOrEmpty(inName) || _name == null) return;
        _name.text = inName;
    }
    
    private void SetIcon(Sprite inIconSprite)
    {
        if (inIconSprite == null || _icon == null) return;
        _icon.sprite = inIconSprite;
    }

    private void SetActionButton(Action<int> inClickCallback)
    {
        if (_actionButton == null || inClickCallback == null) return;
        _actionButton.onClick.AddListener(()=>
        {
            inClickCallback.Invoke(_handId);
        });
    }
}
