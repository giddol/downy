/*
 * http://cafe.naver.com/unityhub/5393
 * 1. FIFO(First In First Out)방식의 Queue를 이용하여 활성화된 개체를 검색하는데 불필요한 루프를 없앴습니다.
 * 2. 상황에 따라 개체가 더 필요해질 경우 자동으로 Instance를 호출하게 하였습니다.
 * 3. 2의 경우가 과도하게 많아질 경우를 대비하여 개체의 한도를 넣을 수 있게 하였습니다.
 */
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MemoryPool : IEnumerable, System.IDisposable
{
    private List<Floor> items = new List<Floor>();
    private Queue<Floor> queue = new Queue<Floor>();
    private Floor original;
    private int maxCount;

    public IEnumerator GetEnumerator()
    {
        foreach (Floor item in items)
            yield return item;
    }

    public MemoryPool(Floor original, int initialCount, int maxCount)
    {
        this.original = original;
        this.maxCount = maxCount;

        for (int i = 0; i < initialCount && i < maxCount; i++)
        {
            Floor newItem = GameObject.Instantiate(original);
            newItem.gameObject.SetActive(false);
            items.Add(newItem);
            queue.Enqueue(newItem);
        }
    }

    public Floor TakeItem()
    {
        if (queue.Count > 0)
        {
            Floor item = queue.Dequeue();
            item.gameObject.SetActive(true);
            return item;
        }
        else if (items.Count < maxCount)
        {
            Floor newItem = GameObject.Instantiate(original);
            items.Add(newItem);
            return newItem;
        }
        else
        {
            throw new UnityException("Memory Pool의 한도를 초과했습니다.");
        }
    }

    public void PutItem(Floor gameObject)
    {
        if (gameObject == null)
            return;

        gameObject.gameObject.SetActive(false);
        queue.Enqueue(gameObject);
    }

    public void ClearItem()
    {
        foreach (Floor item in items)
            item.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        foreach (Floor item in items)
            GameObject.Destroy(item);

        items.Clear();
        queue.Clear();
        items = null;
        queue = null;
    }
}