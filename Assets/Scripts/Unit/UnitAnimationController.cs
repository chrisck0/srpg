using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitAnimationController : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private Animator _animator;
    [SerializeField] private InputActionReference _selectInput;

    private const string PARAM_IS_FIRST_WEAPON_ATTACK = "IsFirstWeaponAttack";
    private const string PARAM_IS_SECOND_WEAPON_ATTACK = "IsSecondWeaponAttack";
    private const string PARAM_IS_SELECT = "IsSelect";
    private const string PARAM_IS_MOVING = "IsMoving";
    private const string PARAM_IS_VICTORY = "IsVictory";
    private const string PARAM_IS_AVOID = "IsAvoid";
    private const string PARAM_IS_DAMAGE = "IsDamage";
    private const string PARAM_IS_DEAD = "IsDead";

    private readonly int _isFirstWeaponAttackId = Animator.StringToHash(PARAM_IS_FIRST_WEAPON_ATTACK);
    private readonly int _isSecondWeaponAttackId = Animator.StringToHash(PARAM_IS_SECOND_WEAPON_ATTACK);
    private readonly int _isSelectId = Animator.StringToHash(PARAM_IS_SELECT);
    private readonly int _isMovingId = Animator.StringToHash(PARAM_IS_MOVING);
    private readonly int _isVictoryId = Animator.StringToHash(PARAM_IS_VICTORY);
    private readonly int _isAvoidId = Animator.StringToHash(PARAM_IS_AVOID);
    private readonly int _isDamageId = Animator.StringToHash(PARAM_IS_DAMAGE);
    private readonly int _isDeadId = Animator.StringToHash(PARAM_IS_DEAD);

    private bool _isMoving;

    private void OnEnable() => BindUnitEvents();
    private void OnDisable() => UnbindUnitEvents();

    private void Attack(int weaponId)
    {
        if (weaponId == _owner.UnitStat.UnitData.WeaponIdList[0])
        {
            _animator.SetTrigger(_isFirstWeaponAttackId);
        }
        else
        {
            _animator.SetTrigger(_isSecondWeaponAttackId);
        }
    }

    private void Select()
    {
        _animator.SetTrigger(_isSelectId);
    }

    private void Move(bool isMoving)
    {
        _isMoving = isMoving;
        _animator.SetBool(_isMovingId, _isMoving);
    }

    private void Victory()
    {
        _animator.SetTrigger(_isVictoryId);
    }

    private void Avoid()
    {
        _animator.SetTrigger(_isAvoidId);
    }

    private void Damage()
    {
        _animator.SetTrigger(_isDamageId);
    }

    private void Dead(UnitBase unit)
    {
        _animator.SetTrigger(_isDeadId);
    }

    private void BindUnitEvents()
    {
        _owner.OnAttackWeapon += Attack;
        _owner.OnSelect += Select;
        _owner.OnMove += Move;
        _owner.OnVictory += Victory;
        _owner.OnAvoid += Avoid;
        _owner.OnDamage += Damage;
        _owner.OnDead += Dead;
    }

    private void UnbindUnitEvents()
    {
        _owner.OnAttackWeapon += Attack;
        _owner.OnSelect -= Select;
        _owner.OnMove -= Move;
        _owner.OnVictory -= Victory;
        _owner.OnAvoid -= Avoid;
        _owner.OnDamage -= Damage;
        _owner.OnDead -= Dead;
    }
}
