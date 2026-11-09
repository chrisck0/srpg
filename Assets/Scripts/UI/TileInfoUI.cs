using TMPro;
using UnityEngine;

public class TileInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tileNameText;
    [SerializeField] private TextMeshProUGUI _tileDescriptionText;

    public void SetData(string tileName, string tileDescription)
    {
        _tileNameText.text = tileName;
        _tileDescriptionText.text = tileDescription;
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
