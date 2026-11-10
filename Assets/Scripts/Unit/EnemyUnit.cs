using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EnemyAI))]
public class EnemyUnit : UnitBase
{
    [SerializeField] private EnemyAI _enemyAI;

    public EnemyAI EnemyAI => _enemyAI;
    public override UnitType UnitType => UnitType.Enemy;
    public Action OnEnemyActioned;
}
