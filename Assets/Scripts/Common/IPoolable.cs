/// <summary>
/// 오브젝트 풀에서 꺼낼 수 있는 오브젝트
/// </summary>
public interface IPoolable
{
    public void Activate();
    public void Deactivate();
    public void ReturnToPool();
}
