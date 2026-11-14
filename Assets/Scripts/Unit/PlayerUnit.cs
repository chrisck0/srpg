public class PlayerUnit : UnitBase
{
    private bool _canAction = true;
    public bool CanAction => _canAction;
    public override UnitType UnitType => UnitType.Player;

    public void InitAction()
    {
        _canAction = true;
        OnUnitCanAction?.Invoke();
    }

    public void Action()
    {
        _canAction = false;
        OnUnitActioned?.Invoke();
    }
}
