using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// 월드 위치를 맵 좌표로 변환
    /// </summary>
    /// <param name="worldPosition"> 월드 위치 </param>
    /// <returns></returns>
    public static Vector2Int WorldToCoord(this Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.FloorToInt(worldPosition.z)
        );
    }

    /// <summary>
    /// 맵 좌표를 월드 위치로 변환
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <param name="positionY"> 월드의 Y 위치 </param>
    /// <returns></returns>
    public static Vector3 CoordToWorld(this Vector2Int coord, float positionY = 0f)
    {
        return new Vector3(
            coord.x + .5f,
            positionY,
            coord.y + .5f
        );
    }
}
