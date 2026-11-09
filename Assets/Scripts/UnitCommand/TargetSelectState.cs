using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TargetSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;
    private CommandUIController _commandUIController;
    private TileInfoUI _tileInfoUI;
    private UnitInfoUI _unitInfoUI;

    public TargetSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cancelAction,
        InputActionReference cursorPosition,
        LayerMask ground,
        CommandUIController commandUIController)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cancelAction = cancelAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
        _commandUIController = commandUIController;
        _tileInfoUI = commandUIController.TileInfoUI;
        _unitInfoUI = commandUIController.UnitInfoUI;
    }

    public override void Enter()
    {
        BindInputActions();
        Debug.Log("Target Select");
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
        _controller.HideCursor();
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
            EnemyUnit enemyUnit = (unit as EnemyUnit);

            if (enemyUnit == null) return;

            Debug.Log($"Selected Weapon {_controller.SelectedWeaponData.Name}");
            unit.Weapon.Attack(_controller.SelectedWeaponData, enemyUnit);
            _controller.SetState(_controller.CommandEndState);
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        _controller.SetState(_controller.UnitCommandState);
        _controller.HideCursor();
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
