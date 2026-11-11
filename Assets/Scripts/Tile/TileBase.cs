using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TileBase : MonoBehaviour, IPoolable
{
    private ObjectPool<TileBase> _objectPool;

    [SerializeField] private TileData _tileData;
    [SerializeField] private GameObject _blueTransparentTile;
    [SerializeField] private GameObject _redTransparentTile;
    [SerializeField] private TextMeshProUGUI _distanceText;
    private bool _canSelect;

    public TileData TileData => _tileData;
    public bool CanSelect => _canSelect;

    private void Awake() => InitTile();

    public void SetCanSelect()
    {
        _canSelect = true;
    }

    public void ShowDistance(int num)
    {
        _distanceText.gameObject.SetActive(true);
        _distanceText.text = num.ToString();
        _blueTransparentTile.gameObject.SetActive(true);
    }

    public void ShowRange()
    {
        _redTransparentTile.gameObject.SetActive(true);
        _blueTransparentTile.gameObject.SetActive(false);
    }

    public void HideRange()
    {
        _redTransparentTile.gameObject.SetActive(false);
        if (_canSelect == true) _blueTransparentTile.gameObject.SetActive(true);
    }

    public void InitTile()
    {
        _distanceText.gameObject.SetActive(false);
        _blueTransparentTile.gameObject.SetActive(false);
        _redTransparentTile.gameObject.SetActive(false);
        _canSelect = false;
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
