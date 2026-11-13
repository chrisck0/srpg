using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;
    [SerializeField] private Material _activeMaterial;
    [SerializeField] private Material _inactiveMaterial;
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenderer;
    [SerializeField] private MeshRenderer[] _meshRenderer;

    private void OnEnable() => BindUnitEvents();
    private void OnDisable() => UnbindUnitEvents();

    public void SetWhite()
    {
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in _skinnedMeshRenderer)
        {
            skinnedMeshRenderer.material = _activeMaterial;
        }
        foreach (MeshRenderer meshRenderer in _meshRenderer)
        {
            meshRenderer.material = _activeMaterial;
        }
    }

    public void SetGray()
    {
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in _skinnedMeshRenderer)
        {
            skinnedMeshRenderer.material = _inactiveMaterial;
        }
        foreach (MeshRenderer meshRenderer in _meshRenderer)
        {
            meshRenderer.material = _inactiveMaterial;
        }
    }

    private void BindUnitEvents()
    {
        _owner.OnUnitCanAction += SetWhite;
        _owner.OnUnitActioned += SetGray;
    }

    private void UnbindUnitEvents()
    {
        _owner.OnUnitCanAction -= SetWhite;
        _owner.OnUnitActioned -= SetGray;
    }
}
