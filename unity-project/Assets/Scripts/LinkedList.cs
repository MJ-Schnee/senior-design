using System.Collections.Generic;

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

    // Add data to end of list's first iteration
    public void AddToEnd(T data)
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
            while (temp.Next != head)
            {
                temp = temp.Next;
            }
            temp.Next = newNode;
            newNode.Next = head;
        }
    }

    // Add data after existing data is found
    // If first iteration end is found without existing data, throw exception
    public void AddAfter(T newData, T existingData)
    {
        if (head == null)
        {
            throw new System.Exception("existingData not found in CircularLinkedList");
        }

        // Search for the node containing existingData
        Node<T> temp = head;
        do
        {
            if (temp.Data.Equals(existingData))
            {
                // Found the node where we should insert newData after it
                Node<T> newNode = new(newData)
                {
                    Next = temp.Next
                };
                temp.Next = newNode;

                return;
            }
            temp = temp.Next;
        } 
        while (temp != head);

        // If we exit the loop, afterData was not found
        throw new System.Exception("existingData not found in CircularLinkedList");
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

    public T StartNextTurn()
    {
        head = head.Next;

        return head.Data;
    }

    public T GetCurrentTurn()
    {
        return head.Data;
    }

    public List<T> GetTurnOrder()
    {
        List<T> turnOrder = new();

        Node<T> current = head;

        do
        {
            turnOrder.Add(current.Data);
            current = current.Next;
        } while (current != head);

        return turnOrder;
    }
}
