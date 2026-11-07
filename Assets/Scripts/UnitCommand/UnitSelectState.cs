using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private DataManager _dataManager => _controller.DataManager;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;

    public UnitSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cursorPosition,
        LayerMask ground)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
        BindInputActions();
    }

    public override void Enter()
    {
        Debug.Log("Select Unit");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        UnbindInputActions();
    }

    private void Select(InputAction.CallbackContext context)
    {
        Vector2 screen = _cursorPosition.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _ground))
        {
            Debug.Log(hit.collider.name);
            Vector2Int coord = hit.transform.position.WorldToCoord();
            UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

            PlayerUnit playerUnit = (unit as PlayerUnit);
            
            if (playerUnit != null)
            {
                _controller.SetSelectedPlayerUnit(playerUnit);
                _controller.SetState(_controller.UnitMoveState);
            }
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
