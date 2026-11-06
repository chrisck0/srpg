using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StageDataParser : DataParserBase
{
    [SerializeField] private List<string> _stageDataUrlList;
    [SerializeField] private List<TextAsset> _stageDataFileList;
    [SerializeField] private List<StageData> _stageDataList;

    private List<UniTask> _loadingTasks = new();
    private const int STAGE_INFO_ROW_INDEX = 2;
    private const int START_ROW_INDEX = 5;
    private const int START_COLUMN_INDEX = 1;
    private const int UNIT_POSITION_START_ROW_OFFSET = 3;

    public int StageCount => _stageDataUrlList.Count;

    public override async UniTask LoadData()
    {
        for (int i = 0; i < _stageDataUrlList.Count; i++)
        {
            _loadingTasks.Add(LoadStageData(i));
        }

        await UniTask.WhenAll(_loadingTasks);
        Debug.Log("StageData Loaded");
    }

    /// <summary>
    /// StageData 로드
    /// </summary>
    public async UniTask LoadStageData(int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(_stageDataUrlList[index]);
        www.timeout = 5;
        await www.SendWebRequest();

        string text;

        if (www.result != UnityWebRequest.Result.Success)
        {
            text = _stageDataFileList[index].text;
        }
        else
        {
            text = www.downloadHandler.text;
        }

        ParseCSV(text, index);
    }

    private void ParseCSV(string text, int index)
    {
        string[] lines = text.Split(
            new char[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
            );


        int stageId, mapSizeX, mapSizeY;
        string stageName;
        TileType[,] map;
        List <UnitPosition> unitPositionList = new();

        string[] stageInfos = lines[STAGE_INFO_ROW_INDEX].Split(',');

        if (!int.TryParse(stageInfos[0], out stageId)
            || !int.TryParse(stageInfos[2], out mapSizeX)
            || !int.TryParse(stageInfos[3], out mapSizeY)
            )
        {
            Debug.LogError("Stage Info Parse Error");
            return;
        }

        stageName = stageInfos[1];

        map = new TileType[mapSizeX, mapSizeY];

        for (int i = START_ROW_INDEX; i < START_ROW_INDEX + mapSizeY; i++)
        {
            string[] cells = lines[i].Split(',');

            for (int j = START_COLUMN_INDEX; j < cells.Length; j++)
            {
                if (!Enum.TryParse<TileType>(cells[j], out TileType tileType))
                {
                    Debug.LogError("Stage Table Parse Error");
                    return;
                }

                map[j - START_COLUMN_INDEX, mapSizeY + START_ROW_INDEX - i - 1] = tileType;
            }

        }

        for (int i = START_ROW_INDEX + mapSizeY + UNIT_POSITION_START_ROW_OFFSET; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(',');

            int unitId, unitPositionX, unitPositionY;
            Vector2Int unitPosition;
            string[] unitPositionCoord = cells[1].Split('.');

            if (!int.TryParse(cells[0], out unitId)
                || !int.TryParse(unitPositionCoord[0], out unitPositionX)
                || !int.TryParse(unitPositionCoord[1], out unitPositionY)
                )
            {
                Debug.LogError("UnitPosition Parse Error");
                continue;
            }

            unitPosition = new Vector2Int(unitPositionX, unitPositionY);
            unitPositionList.Add(new UnitPosition(unitId, unitPosition));
        }

        _stageDataList[index].SetData(stageId, stageName, map, unitPositionList);
        // _stageDataList[index].PrintMap();
    }
}
