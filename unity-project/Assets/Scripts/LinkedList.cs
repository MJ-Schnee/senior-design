public class Node<T>
{
    public T Data;
    public Node<T> Next;

    public Node(T data)
    {
        Data = data;
        Next = null;
    }
}

public class CircularLinkedList<T>
{
    private Node<T> head;
    private Node<T> current;

    public void Add(T data)
    {
        Node<T> newNode = new(data);
        if (head == null)
        {
            head = newNode;
            head.Next = head;
        }
        else
        {
            Node<T> temp = head;
            while (temp.Next != head) // Find the last node
            {
                temp = temp.Next;
            }
            temp.Next = newNode;
            newNode.Next = head; // Maintain circular structure
        }
    }

    public bool Remove(T data)
    {
        if (head == null) return false; // Empty list case

        Node<T> temp = head, prev = null;

        // Case 1: Removing the head
        if (head.Data.Equals(data))
        {
            if (head.Next == head) // Only one node
            {
                head = null;
                current = null;
            }
            else
            {
                Node<T> last = head;
                while (last.Next != head) last = last.Next; // Find last node
                head = head.Next; // Move head
                last.Next = head; // Update last node to point to new head
            }

            if (current == temp) current = head; // If current was removed, move to next
            return true;
        }

        // Case 2: Removing non-head nodes
        do
        {
            prev = temp;
            temp = temp.Next;

            if (temp.Data.Equals(data))
            {
                prev.Next = temp.Next; // Skip over the node
                if (current == temp) current = prev.Next; // Move current if needed
                return true;
            }
        } while (temp != head);

        return false; // Node not found
    }

    public T GetNextTurn()
    {
        current ??= head; // Start from first player

        T data = current.Data;
        current = current.Next; // Move to the next player

        return data;
    }

    public T GetCurrentTurn()
    {
        return head.Data;
    }
}
