using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitCommandUI : MonoBehaviour
{
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _waitButton;

    public event Action OnAttack;
    public event Action OnWait;

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    private void Attack()
    {
        OnAttack?.Invoke();
    }

    private void Wait()
    {
        OnWait?.Invoke();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void BindButtonEvents()
    {
        _attackButton.onClick.AddListener(Attack);
        _waitButton.onClick.AddListener(Wait);
    }

    private void UnbindButtonEvents()
    {
        _attackButton.onClick.RemoveListener(Attack);
        _waitButton.onClick.RemoveListener(Wait);
    }
}
