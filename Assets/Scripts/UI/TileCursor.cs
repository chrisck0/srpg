using UnityEngine;

public class TileCursor : MonoBehaviour
{
    /// <summary>
    /// 커서 위치 설정
    /// </summary>
    public void SetPosition(Vector2Int coord)
    {
        transform.position = coord.CoordToWorld();
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
