using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class DataParserBase : MonoBehaviour
{
    public virtual async UniTask LoadData() { }
}
