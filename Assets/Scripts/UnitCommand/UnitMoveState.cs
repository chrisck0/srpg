using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class UnitMoveState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;

    public UnitMoveState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cancelAction,
        InputActionReference cursorPosition,
        LayerMask ground)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cancelAction = cancelAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
    }

    public override void Enter()
    {
        Debug.Log("Move Selected Unit");
        BindInputActions();
        ShowMovableTiles();
    }

    public override void Update()
    {
        Vector2 screen = _cursorPosition.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _ground))
        {
            _controller.HideCursor();
            _currentTile = null;
            return;
        }

        TileBase tile = hit.collider.gameObject.GetComponent<TileBase>();

        if (tile == null || tile == _currentTile) return;

        _currentTile = tile;
        Vector2Int coord = hit.transform.position.WorldToCoord();

        _controller.ShowCursor(coord);
    }

    public override void Exit()
    {
        UnbindInputActions();
        InitTiles();
        _controller.HideCursor();
    }

    private void InitTiles()
    {
        TileBase[,] tileMap = _mapManager.TileMap;

        for (int i = 0; i < _mapManager.TileMap.GetLength(0); i++)
        {
            for (int j = 0; j < _mapManager.TileMap.GetLength(1); j++)
            {
                tileMap[j, i].InitDistance();
            }
        }
    }

    private void ShowMovableTiles()
    {
        PlayerUnit selectedUnit = _controller.SelectedPlayerUnit;
        Vector2Int currentCoord = selectedUnit.transform.position.WorldToCoord();
        _mapManager.UnitMovement.Dijkstra(currentCoord, selectedUnit.UnitStat.UnitData.Move);
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_currentTile == null) return;
        PlayerUnit selectedUnit = _controller.SelectedPlayerUnit;
        Vector2Int currentCoord = selectedUnit.transform.position.WorldToCoord();
        Vector2Int nextCoord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[nextCoord.x, nextCoord.y];

        if (unit != null || !_mapManager.TileMap[nextCoord.x, nextCoord.y].CanMove)
        {
            Debug.Log("Cannot Move Unit");
        }
        else
        {
            MoveUnit(selectedUnit, currentCoord, nextCoord);
        }
    }

    private void MoveUnit(PlayerUnit playerUnit, Vector2Int prevCoord, Vector2Int nextCoord)
    {
        playerUnit.transform.position = nextCoord.CoordToWorld();
        _mapManager.UnitMap[prevCoord.x, prevCoord.y] = null;
        _mapManager.UnitMap[nextCoord.x, nextCoord.y] = playerUnit;

        _controller.SetState(_controller.UnitCommandState);
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        _controller.SetState(_controller.UnitSelectState);
    }

    private void BindInputActions()
    {
        _selectAction.action.Enable();
        _selectAction.action.performed += Select;
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Cancel;
        _cursorPosition.action.Enable();
    }

    private void UnbindInputActions()
    {
        _selectAction.action.Disable();
        _selectAction.action.performed -= Select;
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Cancel;
        _cursorPosition.action.Disable();
    }
}
