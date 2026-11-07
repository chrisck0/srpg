using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class UnitSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private TileCursor _tileCursor;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;

    public UnitSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cursorPosition,
        LayerMask ground,
        TileCursor tileCursor)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
        _tileCursor = tileCursor;
    }

    public override void Enter()
    {
        Debug.Log("Select Unit");
        BindInputActions();
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
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_currentTile == null) return;

        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit != null)
        {
            unit.ShowInfo(); // TODO: UI 추가
            PlayerUnit playerUnit = (unit as PlayerUnit);

            if (playerUnit == null || !playerUnit.CanAction) return;

            _controller.SetSelectedPlayerUnit(playerUnit);
            _controller.SetState(_controller.UnitMoveState);
        }
        else
        {
            _currentTile.ShowInfo(); // TODO: UI 추가
        }
    }

    private void BindInputActions()
    {
        _selectAction.action.Enable();
        _selectAction.action.performed += Select;
        _cursorPosition.action.Enable();
    }

    private void UnbindInputActions()
    {
        _selectAction.action.Disable();
        _selectAction.action.performed -= Select;
        _cursorPosition.action.Disable();
    }
}
