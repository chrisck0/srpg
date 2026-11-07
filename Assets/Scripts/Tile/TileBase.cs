using UnityEngine;

public abstract class TileBase : MonoBehaviour, IPoolable
{
    private ObjectPool<TileBase> _objectPool;
    
    public abstract TileType TileType { get; }
    public abstract int MoveCost { get; }

    public void ShowInfo()
    {
        Debug.Log($"Selected Tile is {gameObject.name}");
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
