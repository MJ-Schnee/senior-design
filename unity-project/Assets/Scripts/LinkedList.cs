using System.Collections.Generic;
using UnityEngine;
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
        while (head.Data == null)
        {
            Remove(head.Data);
        }
        return head.Data;
    }

    public T GetCurrentTurn()
    {
        while (head.Data == null)
        {
            Remove(head.Data);
        }
        return head.Data;
    }

    public List<T> GetTurnOrder()
    {
        List<T> turnOrder = new();

        ClearNull();

        Node<T> current = head;
        do
        {
            turnOrder.Add(current.Data);
            current = current.Next;
        } while (current != head);

        return turnOrder;
    }

    public void Clear()
    {
        head = null;
        current = null;
    }

    public void ClearNull()
    {
        Node<T> temp = head.Next, prev = head;
        do
        { 
            if (temp.Data == null)
            {
                prev.Next = temp.Next;
                if (head == temp) head = temp.Next;
            }
            prev = temp;
            temp = temp.Next;
        }
        while(temp != head.Next);
    }
}
