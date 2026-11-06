using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitDataParser : DataParserBase
{
    [SerializeField] private string _unitDataUrl;
    [SerializeField] private TextAsset _unitDataFile;
    [SerializeField] private List<UnitData> _unitDataList;
    private const int COLUMN = 9;
    private const int START_ROW_INDEX = 2;

    /// <summary>
    /// UnitData 로드
    /// </summary>
    public override async UniTask LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(_unitDataUrl);
        www.timeout = 5;
        await www.SendWebRequest();

        string text;

        if (www.result != UnityWebRequest.Result.Success)
        {
            text = _unitDataFile.text;
        }
        else
        {
            text = www.downloadHandler.text;
        }

        ParseCSV(text);
        Debug.Log("UnitData Loaded");
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

            string name;
            int id, health, energy, move, accuracy, avoid;
            List<int> weaponIdList = new();
            UnitType unitType;

            if (!int.TryParse(cells[0], out id)
                || !int.TryParse(cells[2], out health)
                || !int.TryParse(cells[3], out energy)
                || !int.TryParse(cells[4], out move)
                || !int.TryParse(cells[5], out accuracy)
                || !int.TryParse(cells[6], out avoid)
                || !Enum.TryParse<UnitType>(cells[8], out unitType)
                )
            {
                Debug.LogError("Parse Error");
                continue;
            }

            string[] weaponIds = cells[7].Split('.');

            foreach (string weaponIdString in weaponIds)
            {
                if (!int.TryParse(weaponIdString, out int weaponId))
                {
                    Debug.LogError("Weapon Id Parse Error");
                }
                weaponIdList.Add(weaponId);
            }

            name = cells[1];

            if (i - START_ROW_INDEX >= _unitDataList.Count)
            {
                Debug.LogError("Unit Data List Length Error");
                break;
            }

            _unitDataList[i - START_ROW_INDEX].SetData(
                id, name, health, energy, move, accuracy, avoid, weaponIdList, unitType);
        }
    }
}
