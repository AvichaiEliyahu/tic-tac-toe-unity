using System;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    Action<CellView> _onClick;

    public Vector2Int GridPosition { get; private set; }

    public void Initialize(Vector2Int position)
    {
        GridPosition = position;
        InitializeButton();
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
    }

    public void SetClickCallback(Action<CellView> callback)
    {
        _onClick = callback;
    }

    public void SetState(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }

    private void InitializeButton()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _onClick?.Invoke(this));
    }

}
