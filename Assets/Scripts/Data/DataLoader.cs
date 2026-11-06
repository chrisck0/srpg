using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [SerializeField] DataParserBase[] _dataParsers;
    private List<UniTask> _loadingTasks = new();

    public event Action OnDataLoaded;

    private void Start() => LoadDatas().Forget();

    private async UniTask LoadDatas()
    {
        foreach (DataParserBase dataParser in _dataParsers)
        {
            _loadingTasks.Add(dataParser.LoadData());
        }

        await UniTask.WhenAll(_loadingTasks);

        OnDataLoaded?.Invoke();
    }
}
