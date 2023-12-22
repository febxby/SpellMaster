using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> heap = new List<T>();

    // 获取队列元素数量
    public int Count
    {
        get { return heap.Count; }
    }

    /// <summary>
    /// 入队操作
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(T item)
    {
        heap.Add(item);
        HeapifyUp(); // 上溯操作，维护最小堆性质
    }

    /// <summary>
    /// 出队操作
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        if (heap.Count == 0)
        {
            return default(T);
        }

        T result = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        if (heap.Count > 1)
        {
            HeapifyDown(); // 下溯操作，维护最小堆性质
        }

        return result;
    }
    /// <summary>
    /// 出队
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryDequeue(out T result)
    {
        if (heap.Count == 0)
        {
            result = default(T);
            return false;
        }
        result = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        if (heap.Count > 1)
        {
            HeapifyDown(); // 下溯操作，维护最小堆性质
        }

        return true;
    }


    // 上溯操作，维护最小堆性质
    private void HeapifyUp()
    {
        int currentIndex = heap.Count - 1;

        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (heap[currentIndex].CompareTo(heap[parentIndex]) >= 0)
            {
                break;
            }

            Swap(currentIndex, parentIndex);
            currentIndex = parentIndex;
        }
    }

    // 下溯操作，维护最小堆性质
    private void HeapifyDown()
    {
        int currentIndex = 0;
        int leftChildIndex, rightChildIndex, swapIndex;

        while (true)
        {
            leftChildIndex = 2 * currentIndex + 1;
            rightChildIndex = 2 * currentIndex + 2;
            swapIndex = -1;

            if (leftChildIndex < heap.Count)
            {
                if (heap[leftChildIndex].CompareTo(heap[currentIndex]) < 0)
                {
                    swapIndex = leftChildIndex;
                }
            }

            if (rightChildIndex < heap.Count)
            {
                if (heap[rightChildIndex].CompareTo(heap[currentIndex]) < 0 && (swapIndex == -1 || heap[rightChildIndex].CompareTo(heap[leftChildIndex]) < 0))
                {
                    swapIndex = rightChildIndex;
                }
            }

            if (swapIndex == -1)
            {
                break;
            }

            Swap(currentIndex, swapIndex);
            currentIndex = swapIndex;
        }
    }

    // 交换两个元素的位置
    private void Swap(int index1, int index2)
    {
        T temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }
    public void Clear()
    {
        heap.Clear();
    }
}
