using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowAndArrowAnimationController : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private Animator _bowAnimator;
    [SerializeField] private Animator _arrowAnimator;
    // [SerializeField] private InputActionReference _selectInput;

    private const string PARAM_IS_FIRST_WEAPON_ATTACK = "IsFirstWeaponAttack";
    private const string PARAM_IS_SECOND_WEAPON_ATTACK = "IsSecondWeaponAttack";

    private readonly int _isFirstWeaponAttackId = Animator.StringToHash(PARAM_IS_FIRST_WEAPON_ATTACK);
    private readonly int _isSecondWeaponAttackId = Animator.StringToHash(PARAM_IS_SECOND_WEAPON_ATTACK);

    private void OnEnable() => BindUnitEvents();
    private void OnDisable() => UnbindUnitEvents();

    private void Attack(int weaponId)
    {
        if (weaponId == _owner.UnitStat.UnitData.WeaponIdList[0])
        {
            _bowAnimator.SetTrigger(_isFirstWeaponAttackId);
            _arrowAnimator.SetTrigger(_isFirstWeaponAttackId);
        }
        else
        {
            _bowAnimator.SetTrigger(_isSecondWeaponAttackId);
            _arrowAnimator.SetTrigger(_isSecondWeaponAttackId);
        }
    }

    private void BindUnitEvents()
    {
        _owner.OnAttackWeapon += Attack;
    }

    private void UnbindUnitEvents()
    {
        _owner.OnAttackWeapon += Attack;
    }
}
