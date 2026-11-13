using TMPro;
using UnityEngine;

public class TileInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tileNameText;
    [SerializeField] private TextMeshProUGUI _tileDescriptionText;

    public void SetData(TileBase tile)
    {
        _tileNameText.text = tile.TileData.Name;
        _tileDescriptionText.text = tile.TileData.Description;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
