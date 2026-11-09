using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class UnitSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;
    private CommandUIController _commandUIController;
    private TileInfoUI _tileInfoUI;
    private UnitInfoUI _unitInfoUI;

    public UnitSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cursorPosition,
        LayerMask ground,
        CommandUIController commandUIController)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
        _commandUIController = commandUIController;
        _tileInfoUI = commandUIController.TileInfoUI;
        _unitInfoUI = commandUIController.UnitInfoUI;
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
            _tileInfoUI.Hide();
            _unitInfoUI.Hide();
            return;
        }

        TileBase tile = hit.collider.gameObject.GetComponent<TileBase>();

        if (tile == null || tile == _currentTile) return;

        _currentTile = tile;

        ShowTileInfo(tile);
        ShowUnitInfo(tile);

        Vector2Int coord = hit.transform.position.WorldToCoord();
        _controller.ShowCursor(coord);
    }

    public override void Exit()
    {
        UnbindInputActions();
        HideUI();
    }

    private void ShowTileInfo(TileBase tile)
    {
        _tileInfoUI.Show();
        TileData tileData = tile.TileData;
        _tileInfoUI.SetData(tileData.Name, tileData.Description);
    }

    private void ShowUnitInfo(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            _unitInfoUI.Hide();
            return;
        }

        _unitInfoUI.Show();
        UnitStat unitStat = unit.UnitStat;
        _unitInfoUI.SetData(unitStat.UnitData.Name,
            $"HP {unitStat.Health}/{unitStat.UnitData.Health}",
            $"EN {unitStat.Energy}/{unitStat.UnitData.Energy}",
            unit.Weapon.WeaponDataList[0].Name); // 임시로 첫번째 무기 등록
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_currentTile == null) return;

        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit != null)
        {
            PlayerUnit playerUnit = (unit as PlayerUnit);

            if (playerUnit == null || !playerUnit.CanAction) return;

            _controller.SetSelectedPlayerUnit(playerUnit);
            _controller.SetState(_controller.UnitMoveState);
        }
    }

    private void HideUI()
    {
        _controller.HideCursor();
        _tileInfoUI.Hide();
        _unitInfoUI.Hide();
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
