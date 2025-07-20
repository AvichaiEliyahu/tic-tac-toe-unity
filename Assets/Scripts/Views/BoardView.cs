using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private AssetReferenceT<GameObject> _cellPrefab;

    private CellView[,] _cells;

    public async UniTask InitializeAsync(Action<CellView> onCellClicked)
    {
        int size = GameConsts.BOARD_SIZE;
        _cells = new CellView[size, size];

        InitializeGridLayoutGroup(size);
        await InitializeCells(size, onCellClicked);
    }

    public void DestroyBoard()
    {
        foreach(Transform child in _grid.transform)
        {
            Addressables.ReleaseInstance(child.gameObject);
        }
    }

    private void InitializeGridLayoutGroup(int size)
    {
        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = size;
    }

    private async UniTask InitializeCells(int size, Action<CellView> onCellClicked)
    {
        for (int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                var gridPos = new Vector2Int(x, y);
                var cellGo = await _cellPrefab.InstantiateAsync(_grid.transform);
                var cellView = cellGo.GetComponent<CellView>();
                cellView.Initialize(gridPos, onCellClicked);
                _cells[x, y] = cellView;
            }
        }
    }

    public CellView GetCell(Vector2Int position) => _cells[position.x, position.y];

}
