using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TileBase : MonoBehaviour, IPoolable
{
    private ObjectPool<TileBase> _objectPool;

    [SerializeField] private TileData _tileData;
    [SerializeField] private GameObject _transparentTile;
    [SerializeField] private TextMeshProUGUI _distanceText;
    private bool _canMove;

    public TileData TileData => _tileData;
    public bool CanMove => _canMove;

    private void Awake() => InitDistance();

    public void ShowDistance(int num)
    {
        _distanceText.gameObject.SetActive(true);
        _distanceText.text = num.ToString();
        _transparentTile.gameObject.SetActive(true);
        _canMove = true;
    }

    public void InitDistance()
    {
        _distanceText.gameObject.SetActive(false);
        _transparentTile.gameObject.SetActive(false);
        _canMove = false;
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
