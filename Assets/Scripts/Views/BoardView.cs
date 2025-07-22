using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

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

    public async UniTask InitializeAsync()
    {
        int size = GameConsts.BOARD_SIZE;
        _cells = new CellView[size, size];

        InitializeGridLayoutGroup(size);
        await UniTask.WhenAll(
            InitializeCells(size),
            LoadSprites()
            );
    }

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

    public void DestroyBoard()
    {
        foreach(Transform child in _grid.transform)
        {
            Addressables.ReleaseInstance(child.gameObject);
        }
        ReleaseSprites();
    }

    private void InitializeGridLayoutGroup(int size)
    {
        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = size;
    }

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

    private async UniTask LoadSprites()
    {
        _spriteX = await AddressablesLoader.LoadSpriteAsync(X_SPRITE_ADDRESSABLE_KEY).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
        _spriteO = await AddressablesLoader.LoadSpriteAsync(O_SPRITE_ADDRESSABLE_KEY).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
        _spriteEmptyCell = await AddressablesLoader.LoadSpriteAsync(EMPTY_SPRITE_ADDRESSABLE_KEY).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
    }

    private void ReleaseSprites()
    {
        AddressablesLoader.ReleaseSprite(X_SPRITE_ADDRESSABLE_KEY);
        AddressablesLoader.ReleaseSprite(O_SPRITE_ADDRESSABLE_KEY);
        AddressablesLoader.ReleaseSprite(EMPTY_SPRITE_ADDRESSABLE_KEY);
    }
}
