using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "StageData", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    [SerializeField] private int _stageId;
    [SerializeField] private string _stageName;
    [SerializeField] private StageWinCondition _stageWinCondition;
    [SerializeField] private List<int> _targetIds;
    [SerializeField] private List<int> _mainUnitIds;
    [SerializeField] private TileType[,] _map;
    [SerializeField] private List<UnitPosition> _unitPositionList;

    public int StageId => _stageId;
    public string StageName => _stageName;
    public StageWinCondition StageWinCondition => _stageWinCondition;
    public List<int> TargetIds => _targetIds;
    public List<int> MainUnitIds => _mainUnitIds;
    public TileType[,] Map => _map;
    public List<UnitPosition> UnitPositionList => _unitPositionList;

    public void SetData(int stageId, string stageName, StageWinCondition stageWinCondition,
        List<int> targetIds, List<int> mainUnitIds,
        TileType[,] map, List<UnitPosition> unitPositionList)
    {
        _stageId = stageId;
        _stageName = stageName;
        _stageWinCondition = stageWinCondition;
        _targetIds = targetIds;
        _mainUnitIds = mainUnitIds;
        _map = map;
        _unitPositionList = unitPositionList;
    }

    // 디버그용
    public void PrintMap()
    {
        for (int i = 0; i < _map.GetLength(0); i++) // y좌표
        {
            string mapData = $"{_stageName} ";

            for (int j = 0; j < _map.GetLength(1); j++) // x좌표
            {
                mapData += $"{_map[j, i]}, ";
            }
            Debug.Log(mapData);
        }
    }
}