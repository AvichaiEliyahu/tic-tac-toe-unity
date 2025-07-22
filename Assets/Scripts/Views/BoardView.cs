using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// Class that handles the board view.
/// </summary>
public class BoardView : MonoBehaviour
{
    private const string X_SPRITE_ADDRESSABLE_KEY = "Assets/Sprites/TicTacToeAssets.png[TicTacToeAssets_1]";
    private const string O_SPRITE_ADDRESSABLE_KEY = "Assets/Sprites/TicTacToeAssets.png[TicTacToeAssets_2]";
    private const string EMPTY_SPRITE_ADDRESSABLE_KEY = "Assets/Sprites/TicTacToeAssets.png[TicTacToeAssets_4]";
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private AssetReferenceT<GameObject> _cellPrefab;
    
    private CellView[,] _cells;
    private Sprite _spriteX;
    private Sprite _spriteO;
    private Sprite _spriteEmptyCell;

    /// <summary>
    /// Initialize the board.
    /// </summary>
    /// <returns></returns>
    public async UniTask InitializeAsync(int boardSize)
    {
        _cells = new CellView[boardSize, boardSize];

        InitializeGridLayoutGroup(boardSize);
        await UniTask.WhenAll(
            InitializeCells(boardSize),
            LoadSprites()
            );
    }

    /// <summary>
    /// Draw the board based on the given cells data
    /// </summary>
    /// <param name="cells">Cells data</param>
    public void DrawBoard(CellState[,] cells)
    {
        int sizeX = cells.GetLength(0);
        int sizeY = cells.GetLength(1);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                var cellState = cells[x, y];
                Sprite spriteToUse = cellState switch
                {
                    CellState.PlayerX => _spriteX,
                    CellState.PlayerO => _spriteO,
                    _ => _spriteEmptyCell
                };
                _cells[x, y].SetState(spriteToUse);
            }
        }
    }

    /// <summary>
    /// Wait for human interaction.
    /// </summary>
    /// <param name="availableCells">Availble cells from which the user can select.</param>
    /// <returns>Awaitable UniTask, completes upon user press</returns>
    public async UniTask<(int x, int y)> WaitForPress(bool[,] availableCells)
    {
        var tcs = new UniTaskCompletionSource<(int x, int y)>();
        var size = availableCells.GetLength(0);

        void OnCellClicked(CellView clickedCell)
        {
            for (int x = 0; x < size; x++)
                for(int y = 0; y < size; y++)
                {
                    _cells[x, y].SetInteractable(false);
                    _cells[x, y].SetClickCallback(null);
                }

            tcs.TrySetResult((clickedCell.GridPosition.x, clickedCell.GridPosition.y));
        }

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                _cells[x, y].SetInteractable(availableCells[x, y]);
                _cells[x, y].SetClickCallback(availableCells[x,y] ? OnCellClicked : null);
            }
        return await tcs.Task.AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
    }

    /// <summary>
    /// Release addressables before destroying the board object.
    /// </summary>
    public void DestroyBoard()
    {
        foreach(Transform child in _grid.transform)
        {
            Addressables.ReleaseInstance(child.gameObject);
        }
        ReleaseSprites();
    }

    /// <summary>
    /// Initialization for the GridLayoutGroup
    /// </summary>
    /// <param name="size">Grid size</param>
    private void InitializeGridLayoutGroup(int size)
    {
        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = size;
    }

    /// <summary>
    /// Handles cells instantiation and initialization
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private async UniTask InitializeCells(int size)
    {
        for (int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                var gridPos = new Vector2Int(x, y);
                var cellGo = await _cellPrefab.InstantiateAsync(_grid.transform);
                var cellView = cellGo.GetComponent<CellView>();
                cellView.Initialize(gridPos);
                _cells[x, y] = cellView;
            }
        }
    }

    /// <summary>
    /// Load sprites from addressables
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadSprites()
    {
        _spriteX = await AddressablesLoader.LoadSpriteAsync(X_SPRITE_ADDRESSABLE_KEY).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
        _spriteO = await AddressablesLoader.LoadSpriteAsync(O_SPRITE_ADDRESSABLE_KEY).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
        _spriteEmptyCell = await AddressablesLoader.LoadSpriteAsync(EMPTY_SPRITE_ADDRESSABLE_KEY).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
    }

    /// <summary>
    /// Release sprites loaded from addressables
    /// </summary>
    private void ReleaseSprites()
    {
        AddressablesLoader.ReleaseSprite(X_SPRITE_ADDRESSABLE_KEY);
        AddressablesLoader.ReleaseSprite(O_SPRITE_ADDRESSABLE_KEY);
        AddressablesLoader.ReleaseSprite(EMPTY_SPRITE_ADDRESSABLE_KEY);
    }
}
