using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component, IPoolable
{
    private List<T> _prefabList;
    private Transform _parent;
    private Dictionary<T, Stack<T>> _objectPoolDict = new();
    private Dictionary<T, T> _prefabDict = new();
    private Dictionary<T, Transform> _poolParentDict = new();
    private Dictionary<T, int> _poolCount = new();

    private const int INITIAL_POOL_SIZE = 1;
    private bool _hasPoolParent;

    public ObjectPool(List<T> prefabList, Transform parent, bool hasPoolParent = true)
    {
        _prefabList = prefabList;
        _parent = parent;
        _hasPoolParent = hasPoolParent;
        CreatePool();
    }

    /// <summary>
    /// 오브젝트 풀에서 특정 블록 종류의 IPoolable 꺼내기
    /// </summary>
    /// <param name="blockType"> 블록 종류 </param>
    /// <returns></returns>
    public T Take(T prefab)
    {
        if (_objectPoolDict.ContainsKey(prefab))
        {
            Stack<T> stack = _objectPoolDict[prefab];

            if (stack.Count > 0)
            {
                T obj = _objectPoolDict[prefab].Pop();
                obj.Activate();
                return obj;
            }
            else
            {
                T newObj = Object.Instantiate(_prefabDict[prefab], _poolParentDict[prefab]);
                _poolCount[prefab] = _poolCount[prefab] + 1;
                newObj.gameObject.name = $"{newObj.gameObject.name} {_poolCount[prefab]}";
                newObj.Activate();
                return newObj;
            }
        }
        return null;
    }

    /// <summary>
    /// IPoolable 오브젝트를 오브젝트 풀에 반환
    /// </summary>
    /// <param name="poolable"> IPoolable 오브젝트 </param>
    public void Return(T obj)
    {
        if (!_objectPoolDict.ContainsKey(obj)) return;

        _objectPoolDict[obj].Push(obj);

        obj.transform.SetParent(_poolParentDict[obj]);
        obj.Deactivate();
    }

    private void CreatePool()
    {
        foreach (T prefab in _prefabList)
        {
            Stack<T> objectPoolStack = new(INITIAL_POOL_SIZE);

            _objectPoolDict[prefab] = objectPoolStack;

            if (!_prefabDict.ContainsKey(prefab))
            {
                _prefabDict[prefab] = prefab;
            }

            Transform newPoolParentTransform = _parent;

            if (_hasPoolParent)
            {
                GameObject newPoolParent = new GameObject
                {
                    name = $"{prefab.name} Pool"
                };

                newPoolParentTransform = newPoolParent.transform;

                newPoolParentTransform.SetParent(_parent);
            }

            _poolParentDict[prefab] = newPoolParentTransform;

            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
            {
                T newObject = Object.Instantiate(prefab, newPoolParentTransform);
                newObject.Deactivate();
                newObject.name = $"{newObject.name} {i + 1}";
                objectPoolStack.Push(newObject);
            }
            _poolCount[prefab] = INITIAL_POOL_SIZE;
        }
    }
}
