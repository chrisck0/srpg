using System.Collections;
using UnityEngine;

public abstract class DataParserBase : MonoBehaviour
{
    public abstract IEnumerator LoadDataRoutine();
}
