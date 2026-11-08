using System;
using TMPro;
using UnityEngine;

public abstract class TileBase : MonoBehaviour, IPoolable
{
    private ObjectPool<TileBase> _objectPool;

    [SerializeField] private TileData _tileData;
    [SerializeField] private TextMeshProUGUI _distanceText;

    public TileData TileData => _tileData;

    public void ShowInfo()
    {
        Debug.Log($"Selected Tile is {gameObject.name}");
    }

    public void ShowDistance(int num)
    {
        _distanceText.text = num.ToString();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        _objectPool.Return(this);
    }
}
