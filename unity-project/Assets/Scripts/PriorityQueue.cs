using System.Collections.Generic;

public class PriorityQueue<T>
{
    public int Count => elements.Count;

    private List<(T item, float priority)> elements = new();

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public T Dequeue()
    {
        T bestItem = elements[0].item;
        elements.RemoveAt(0);
        return bestItem;
    }
}
