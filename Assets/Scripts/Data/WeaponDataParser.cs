using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponDataParser : DataParserBase
{
    [SerializeField] private string _weaponDataUrl;
    [SerializeField] private TextAsset _weaponDataFile;
    [SerializeField] private List<WeaponData> _weaponDataList;
    private const int COLUMN = 9;
    private const int START_ROW_INDEX = 2;

    /// <summary>
    /// WeaponData 로드
    /// </summary>
    public override async UniTask LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(_weaponDataUrl);
        www.timeout = 5;
        await www.SendWebRequest();

        string text;

        if (www.result != UnityWebRequest.Result.Success)
        {
            text = _weaponDataFile.text;
        }
        else
        {
            text = www.downloadHandler.text;
        }

        ParseCSV(text);
        Debug.Log("WeaponData Loaded");
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
            int id, attack, energyCost, minRange, maxRange, explosionRange;
            WeaponType weaponType;
            WeaponRangeShape weaponRangeShape;

            if (!int.TryParse(cells[0], out id)
                || !int.TryParse(cells[2], out attack)
                || !int.TryParse(cells[3], out energyCost)
                || !Enum.TryParse<WeaponType>(cells[4], out weaponType)
                || !Enum.TryParse<WeaponRangeShape>(cells[5], out weaponRangeShape)
                || !int.TryParse(cells[6], out minRange)
                || !int.TryParse(cells[7], out maxRange)
                || !int.TryParse(cells[8], out explosionRange)
                )
            {
                Debug.LogError("Parse Error");
                continue;
            }

            name = cells[1];

            if (i - START_ROW_INDEX >= _weaponDataList.Count)
            {
                Debug.LogError("Weapon Data List Length Error");
                break;
            }

            _weaponDataList[i - START_ROW_INDEX].SetData(
                id, name, attack, energyCost, weaponType, weaponRangeShape, minRange, maxRange, explosionRange);
        }
    }
}
