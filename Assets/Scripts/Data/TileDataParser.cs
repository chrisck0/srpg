using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TileDataParser : DataParserBase
{
    [SerializeField] private string _tileDataUrl;
    [SerializeField] private TextAsset _tileDataFile;
    [SerializeField] private List<TileData> _tileDataList;
    private const int COLUMN = 4;
    private const int START_ROW_INDEX = 2;

    /// <summary>
    /// TileData 로드
    /// </summary>
    public override async UniTask LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(_tileDataUrl);
        www.timeout = 5;
        await www.SendWebRequest();

        string text;

        if (www.result != UnityWebRequest.Result.Success)
        {
            text = _tileDataFile.text;
        }
        else
        {
            text = www.downloadHandler.text;
        }

        ParseCSV(text);
        Debug.Log("TileData Loaded");
    }

    private void ParseCSV(string text)
    {
        string[] lines = text.Split(
            new char[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
            );

        for (int i = START_ROW_INDEX; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(',');

            if (cells.Length != COLUMN)
            {
                Debug.LogError("Cell split Error");
                continue;
            }

            string name, description;
            TileType tileType;
            int cost;

            if (!Enum.TryParse<TileType>(cells[0], out tileType)
                || !int.TryParse(cells[2], out cost)
                )
            {
                Debug.LogError("Parse Error");
                continue;
            }

            name = cells[1];
            description = cells[3];

            if (i - START_ROW_INDEX >= _tileDataList.Count)
            {
                Debug.LogError("Unit Data List Length Error");
                break;
            }

            _tileDataList[i - START_ROW_INDEX].SetData(
                tileType, name, cost, description);
        }
    }
}
