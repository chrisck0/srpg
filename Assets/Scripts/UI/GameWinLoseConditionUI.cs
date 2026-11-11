using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameWinLoseConditionUI : MonoBehaviour
{
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private TextMeshProUGUI _gameWinConditionText;
    [SerializeField] private TextMeshProUGUI _gameLoseConditionText;

    private void SetData()
    {
        StageData stageData = GameManager.Instance.GetStageData();

        switch (stageData.StageWinCondition)
        {
            case StageWinCondition.Rout:
                _gameWinConditionText.text = "적 전멸";
                break;

            case StageWinCondition.Targets:
                string stageWinText = "";
                List<int> targetIds = stageData.TargetIds;
                for (int i = 0; i < targetIds.Count; i++)
                {
                    if (!_dataManager.UnitDataDict.TryGetValue(targetIds[i], out UnitData targetData)) continue;
                    
                    if (i == stageData.TargetIds.Count - 1) stageWinText += $"{targetData.Name} 처치";
                    else stageWinText += $"{targetData.Name}, ";
                }
                _gameWinConditionText.text = stageWinText;
                break;
        }

        string stageLoseText = "";
        List<int> mainUnitIds = stageData.MainUnitIds;
        for (int i = 0; i < mainUnitIds.Count; i++)
        {
            if (!_dataManager.UnitDataDict.TryGetValue(mainUnitIds[i], out UnitData mainUnitData)) continue;

            if (i == stageData.MainUnitIds.Count - 1) stageLoseText += $"{mainUnitData.Name} 사망";
            else stageLoseText += $"{mainUnitData.Name} 또는 ";
        }
        _gameLoseConditionText.text = stageLoseText;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        SetData();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
