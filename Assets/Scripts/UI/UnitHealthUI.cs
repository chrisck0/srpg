using UnityEngine;
using UnityEngine.UI;

public class UnitHealthUI : MonoBehaviour
{
    [SerializeField] private UnitStat _ownerStat;
    [SerializeField] private Image _healthImage;
    private Camera _camera;

    private void Awake() => CacheComponents();
    private void OnEnable() => BindUnitEvents();
    private void Update() => LookAtCamera();
    private void OnDisable() => UnbindUnitEvents();

    private void LookAtCamera()
    {
        transform.rotation = _camera.transform.rotation;
        transform.Rotate(0f, 180f, 0f);
    }

    private void SetHealth(int health)
    {
        _healthImage.transform.localScale = new Vector3(
            (float)health / _ownerStat.Data.Health,
            _healthImage.transform.localScale.y,
            _healthImage.transform.localScale.z
            );
    }

    private void BindUnitEvents()
    {
        _ownerStat.OnHealthChanged += SetHealth;
    }

    private void UnbindUnitEvents()
    {
        _ownerStat.OnHealthChanged -= SetHealth;
    }

    private void CacheComponents()
    {
        _camera = Camera.main;
    }
}
