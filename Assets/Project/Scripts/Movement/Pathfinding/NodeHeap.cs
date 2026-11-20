// its a binary min heap.
public class NodeHeap
{
    private Node[] nodes;
    private int currentItemCount;

    public int Count => currentItemCount;
    public int Capacity => nodes.Length;

    public NodeHeap(int maxHeapSize)
    {
        nodes = new Node[maxHeapSize];
    }

    public void Add(Node node)
    {
        // add the new node at the end of the heap.
        // its heap index will be the current number of nodes.
        node.HeapIndex = currentItemCount;

        nodes[currentItemCount] = node;
        currentItemCount++;

        // we sort up so that the node keeps moving upwards if it has a,
        // lower cost than its parent until it gets to its spot in heap.
        SortUp(node);
    }

    public Node RemoveFirst()
    {
        // the first node is always the node with the lowest cost.
        Node firstNode = nodes[0];

        currentItemCount--;

        // moving the last node to the root position.
        // we then need to move it down to its correct spot in the heap.
        if (currentItemCount > 0)
        {
            nodes[0] = nodes[currentItemCount];
            nodes[0].HeapIndex = 0;

            SortDown(nodes[0]);
        }

        // remove the old last node from the array.
        nodes[currentItemCount] = null;

        // mark the removed node as no longer belonging to the heap.
        firstNode.HeapIndex = -1;

        return firstNode;
    }

    public bool Contains(Node node)
    {
        // checking if node hasnt been removed, its index is within current heap,
        // and making sure that node at that position is actually the same object.

        return
            node.HeapIndex >= 0 &&
            node.HeapIndex < currentItemCount &&
            nodes[node.HeapIndex] == node;
    }

    public void UpdateItem(Node node)
    {
        // the nodes cost has changed so it might not be in the correct position.
        // we only update a node when its cost decreases therefore it should move upwards.
        // eg if nodes cost goes from 52 to 48 it will need to move up the heap to a new spot,
        // we never have a situation where a cost can go from 52 to lets say 66.
        SortUp(node);
    }

    public void Clear()
    {
        // reset the heap index of every node currently in the heap.
        // this means they are no longer considered part of the heap.
        for (int i = 0; i < currentItemCount; i++)
        {
            nodes[i].HeapIndex = -1;
            nodes[i] = null;
        }

        currentItemCount = 0;
    }

    private void SortUp(Node node)
    {
        // keep moving the node up until it gets to correct position.
        while (true)
        {
            // if nodes heap index is 0 it must be at the root.
            // so no need to sort it up anymore.
            if (node.HeapIndex == 0) break;

            // in binary heaps the parent index is (child index - 1) / 2.
            int parentIndex = (node.HeapIndex - 1) / 2;

            Node parentNode = nodes[parentIndex];

            // if the current node has a higher cost than its parent,
            // its in its correct spot already.
            if (Compare(node, parentNode) <= 0) break;

            Swap(node, parentNode);
        }
    }

    private void SortDown(Node node)
    {
        // keep moving the node down until it gets to its correct position.
        while (true)
        {
            // in a binary heaps:
            // left child index = (parent index * 2) + 1
            // right child index = (parent index * 2) + 2.
            int leftChildIndex = (node.HeapIndex * 2) + 1;
            int rightChildIndex = (node.HeapIndex * 2) + 2;

            // if there is no left child, there cannot be a right child either so we can return.
            if (leftChildIndex >= currentItemCount) return;

            // we start by assuming left child should be swapped with parent.
            // then we check if the right child has a lower cost than left child,
            // if it does we swap with right child instead as we always want to swap with lowest cost child.
            int swapIndex = leftChildIndex;

            if (rightChildIndex < currentItemCount && Compare(nodes[rightChildIndex], nodes[leftChildIndex]) > 0)
            {
                swapIndex = rightChildIndex;
            }

            // if the parent already has a lower cost than both left and right child node,
            // there is no need to swap them as its in the right place.
            if (Compare(node, nodes[swapIndex]) >= 0) return;

            Swap(node, nodes[swapIndex]);
        }
    }

    private int Compare(Node nodeA, Node nodeB)
    {
        // compare f costs first.
        // compare node b to node a so that a lower f cost has higher priority.
        // positive = node a has lower cost,
        // negative = node b has lower cost,
        // zero = both nodes have same cost.
        int comparison = nodeB.FCost.CompareTo(nodeA.FCost);

        // compare h costs if f costs are the same.
        // same as above.
        if (comparison == 0)
        {
            comparison = nodeB.HCost.CompareTo(nodeA.HCost);
        }

        return comparison;
    }

    private void Swap(Node nodeA, Node nodeB)
    {
        nodes[nodeA.HeapIndex] = nodeB;
        nodes[nodeB.HeapIndex] = nodeA;

        int nodeAIndex = nodeA.HeapIndex;

        nodeA.HeapIndex = nodeB.HeapIndex;
        nodeB.HeapIndex = nodeAIndex;
    }
}