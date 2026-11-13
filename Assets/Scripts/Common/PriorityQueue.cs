using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    private List<TileCost> _priorityQueue = new();
    private Dictionary<TileCost, int> _priorityQueueIndex = new();

    public Dictionary<TileCost, int> PriorityQueueIndex => _priorityQueueIndex;

    /// <summary>
    /// MinHeap에 넣기
    /// </summary>
    public void Add(TileCost tileCost)
    {
        _priorityQueue.Add(tileCost);
        int index = _priorityQueue.Count - 1;
        _priorityQueueIndex[tileCost] = index;

        CompareWithParent(tileCost);
    }

    /// <summary>
    /// MinHeap에서 루트 꺼내기
    /// </summary>
    public TileCost Remove()
    {

        if (_priorityQueue.Count == 0) return null;

        TileCost result = _priorityQueue[0];

        _priorityQueue[0] = _priorityQueue[_priorityQueue.Count - 1];
        _priorityQueue.RemoveAt(_priorityQueue.Count - 1);

        int index = 0;

        while (true)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallestIndex = index;

            if (leftChildIndex < _priorityQueue.Count
                && _priorityQueue[leftChildIndex].Distance < _priorityQueue[smallestIndex].Distance)
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex < _priorityQueue.Count
                && _priorityQueue[rightChildIndex].Distance < _priorityQueue[smallestIndex].Distance)
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index) break;

            TileCost temp = _priorityQueue[smallestIndex];

            _priorityQueue[smallestIndex] = _priorityQueue[index];
            _priorityQueue[index] = temp;

            _priorityQueueIndex[_priorityQueue[smallestIndex]] = smallestIndex;
            _priorityQueueIndex[_priorityQueue[index]] = index;

            index = smallestIndex;
        }

        return result;
    }

    public void Clear()
    {
        _priorityQueue.Clear();
        _priorityQueueIndex.Clear();
    }

    public int Count()
    {
        return _priorityQueue.Count;
    }

    public void CompareWithParent(TileCost tileCost)
    {
        int index = _priorityQueueIndex[tileCost];

        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (_priorityQueue[index].Distance >= _priorityQueue[parentIndex].Distance) break;

            TileCost temp = _priorityQueue[parentIndex];
            _priorityQueue[parentIndex] = _priorityQueue[index];
            _priorityQueue[index] = temp;

            _priorityQueueIndex[_priorityQueue[parentIndex]] = parentIndex;
            _priorityQueueIndex[_priorityQueue[index]] = index;

            index = parentIndex;
        }
    }
}
