using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles a cell view
/// </summary>
public class CellView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    Action<CellView> _onClick;

    public Vector2Int GridPosition { get; private set; }

    /// <summary>
    /// Initializes the cell view
    /// </summary>
    /// <param name="position">Cell position on the board.</param>
    public void Initialize(Vector2Int position)
    {
        GridPosition = position;
        InitializeButton();
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
    }

    /// <summary>
    /// Set the clock callback
    /// </summary>
    /// <param name="callback">Callback for onClick</param>
    public void SetClickCallback(Action<CellView> callback)
    {
        _onClick = callback;
    }

    /// <summary>
    /// Set the cell image
    /// </summary>
    /// <param name="sprite">new sprite for cell</param>
    public void SetState(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    /// <summary>
    /// Change button interactability
    /// </summary>
    /// <param name="interactable"></param>
    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }

    /// <summary>
    /// Initialize the button component of the cell
    /// </summary>
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
