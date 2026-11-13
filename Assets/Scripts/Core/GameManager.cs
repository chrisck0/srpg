using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageData _stageData;

    public StageData StageData => _stageData;

    public static GameManager Instance;

    private void Awake() => SetSingleton();

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public StageData GetStageData()
    {
        return _stageData;
    }

    public void SetStageData(StageData stageData)
    {
        _stageData = stageData;
    }
}
