using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    [SerializeField] private int _stageId;
    [SerializeField] private int[,] _map;
    [SerializeField] private List<UnitPosition> _unitPositionList;
}