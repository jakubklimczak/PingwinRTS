using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> heap;
    private HashSet<T> set;

    public int Count => heap.Count;

    public PriorityQueue()
    {
        heap = new List<T>();
        set = new HashSet<T>();
    }

    public void Enqueue(T item)
    {
        if (!set.Contains(item))
        {
            heap.Add(item);
            set.Add(item);
            int currentIndex = heap.Count - 1;
            int parentIndex = (currentIndex - 1) / 2;

            while (currentIndex > 0 && heap[currentIndex].CompareTo(heap[parentIndex]) < 0)
            {
                Swap(currentIndex, parentIndex);
                currentIndex = parentIndex;
                parentIndex = (currentIndex - 1) / 2;
            }
        }
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
        {
            throw new InvalidOperationException("Priority queue is empty.");
        }

        T root = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);
        set.Remove(root);

        int currentIndex = 0;
        int leftChildIndex = 1;
        int rightChildIndex = 2;

        while (leftChildIndex < heap.Count)
        {
            int minChildIndex = leftChildIndex;
            if (rightChildIndex < heap.Count && heap[rightChildIndex].CompareTo(heap[leftChildIndex]) < 0)
            {
                minChildIndex = rightChildIndex;
            }

            if (heap[currentIndex].CompareTo(heap[minChildIndex]) <= 0)
            {
                break;
            }

            Swap(currentIndex, minChildIndex);
            currentIndex = minChildIndex;
            leftChildIndex = 2 * currentIndex + 1;
            rightChildIndex = 2 * currentIndex + 2;
        }

        return root;
    }

    public bool Contains(T item)
    {
        return set.Contains(item);
    }

    private void Swap(int index1, int index2)
    {
        T temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }
}


public class PathfindingAlgorithm : MonoBehaviour
{
    public List<Vector2> FindPath(int[,] map, Vector2 start, Vector2 end)
    {
        // Create a list to store the path.
        List<Vector2> path = new List<Vector2>();

        // Create a set to store the nodes that have already been visited.
        HashSet<Vector2> visited = new HashSet<Vector2>();

        // Create a priority queue to store the nodes that are still being explored.
        PriorityQueue<Node> openSet = new PriorityQueue<Node>();

        // Create the start node and add it to the open set.
        Node startNode = new Node(start);
        startNode.g = 0;
        startNode.h = CalculateHeuristic(start, end);
        openSet.Enqueue(startNode);

        // Create a dictionary to store the parent node for each visited node.
        Dictionary<Vector2, Node> parentNodes = new Dictionary<Vector2, Node>();

        // While the open set is not empty, do the following:
        while (openSet.Count > 0)
        {
            // Get the node with the lowest f value from the open set.
            Node currentNode = openSet.Dequeue();

            // If the node is the goal, then we have found the path.
            if (currentNode.position == end)
            {
                // Reconstruct the path.
                path = ReconstructPath(parentNodes, currentNode);
                return path;
            }

            // Mark the current node as visited.
            visited.Add(currentNode.position);

            // Generate the successors of the current node.
            List<Node> successors = GenerateSuccessors(currentNode, map, end);

            foreach (Node successor in successors)
            {
                // If the successor has already been visited, skip it.
                if (visited.Contains(successor.position))
                {
                    continue;
                }

                // Calculate the cost from the start node to the successor.
                float tentativeG = currentNode.g + Vector2.Distance(currentNode.position, successor.position);

                // If the successor is not in the open set or the new path to the successor is better, update its values.
                if (!openSet.Contains(successor) || tentativeG < successor.g)
                {
                    successor.g = tentativeG;
                    successor.h = CalculateHeuristic(successor.position, end);
                    successor.f = successor.g + successor.h;

                    // Set the parent of the successor node to the current node.
                    parentNodes[successor.position] = currentNode;

                    // If the successor is not in the open set, add it.
                    if (!openSet.Contains(successor))
                    {
                        openSet.Enqueue(successor);
                    }
                }
            }
        }

        // If we reach this point, then we have not found a path.
        return null;
    }

    private List<Node> GenerateSuccessors(Node node, int[,] map, Vector2 end)
    {
        List<Node> successors = new List<Node>();

        // Check for forced neighbors and add them as successors.
        foreach (Vector2 neighbor in GetForcedNeighbors(node.position, node.parent, map))
        {
            Node successor = new Node(neighbor);
            successor.parent = node;
            successors.Add(successor);
        }

        // Check for regular neighbors and add them as successors.
        foreach (Vector2 neighbor in GetNeighbors(node.position, map))
        {
            Node successor = new Node(neighbor);
            successor.parent = node;
            successors.Add(successor);
        }

        return successors;
    }

    private List<Vector2> GetNeighbors(Vector2 position, int[,] map)
    {
        List<Vector2> neighbors = new List<Vector2>();

        int startX = (int)position.x - 1;
        int startY = (int)position.y - 1;
        int endX = (int)position.x + 1;
        int endY = (int)position.y + 1;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (x == position.x && y == position.y)
                {
                    continue;
                }

                if (IsWalkable(x, y, map))
                {
                    neighbors.Add(new Vector2(x, y));
                }
            }
        }

        return neighbors;
    }

    private List<Vector2> GetForcedNeighbors(Vector2 position, Node parent, int[,] map)
    {
        List<Vector2> forcedNeighbors = new List<Vector2>();

        int dx = (int)(position.x - parent.position.x);
        int dy = (int)(position.y - parent.position.y);

        // Diagonal move
        if (dx != 0 && dy != 0)
        {
            // Check for forced neighbors along the diagonal
            if (!IsWalkable((int)position.x - dx, (int)position.y + dy, map))
            {
                forcedNeighbors.Add(new Vector2(position.x - dx, position.y));
            }
            if (!IsWalkable((int)position.x + dx, (int)position.y - dy, map))
            {
                forcedNeighbors.Add(new Vector2(position.x, position.y - dy));
            }
            if (IsWalkable((int)position.x - dx, (int)position.y, map))
            {
                forcedNeighbors.Add(new Vector2(position.x - dx, position.y));
            }
            if (IsWalkable((int)position.x, (int)position.y - dy, map))
            {
                forcedNeighbors.Add(new Vector2(position.x, position.y - dy));
            }
        }

        // Straight move
        if (dx != 0)
        {
            if (!IsWalkable((int)position.x, (int)position.y - 1, map))
            {
                forcedNeighbors.Add(new Vector2(position.x - dx, position.y - 1));
            }
            if (!IsWalkable((int)position.x, (int)position.y + 1, map))
            {
                forcedNeighbors.Add(new Vector2(position.x - dx, position.y + 1));
            }
        }
        if (dy != 0)
        {
            if (!IsWalkable((int)position.x - 1, (int)position.y, map))
            {
                forcedNeighbors.Add(new Vector2(position.x - 1, position.y - dy));
            }
            if (!IsWalkable((int)position.x + 1, (int)position.y, map))
            {
                forcedNeighbors.Add(new Vector2(position.x + 1, position.y - dy));
            }
        }

        return forcedNeighbors;
    }

    private bool IsWalkable(int x, int y, int[,] map)
    {
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            return false;
        }

        return (map[x, y] == 0 || map[x, y] == 4);
    }

    private float CalculateHeuristic(Vector2 start, Vector2 end)
    {
        return Vector2.Distance(start, end);
    }

    private List<Vector2> ReconstructPath(Dictionary<Vector2, Node> parentNodes, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();

        Node currentNode = endNode;
        while (currentNode != null)
        {
            path.Insert(0, currentNode.position);
            currentNode = parentNodes.ContainsKey(currentNode.position) ? parentNodes[currentNode.position] : null;
        }

        return path;
    }

    private class Node : IComparable<Node>
    {
        public Vector2 position;
        public Node parent;
        public float g;
        public float h;
        public float f;

        public Node(Vector2 position)
        {
            this.position = position;
            parent = null;
            g = h = f = 0f;
        }

        public int HeapIndex { get; set; }

        public int CompareTo(Node other)
        {
            int compare = f.CompareTo(other.f);
            if (compare == 0)
            {
                compare = h.CompareTo(other.h);
            }
            return -compare;
        }
    }
}


//Alternative algorithm :))) ;) XD
/*
public class PathfindingAlgorithm : MonoBehaviour
{
    public List<Vector2> FindPath(int[,] map, Vector2 start, Vector2 end)
    {
        // Create a list to store the path.
        List<Vector2> path = new List<Vector2>();

        // Create a set to store the nodes that have already been visited.
        HashSet<Vector2> visited = new HashSet<Vector2>();

        //good
        List<Vector2> emptyTiles = new List<Vector2>();

        // Create a stack to store the nodes that are still being explored.
        Stack<Vector2> stack = new Stack<Vector2>();

        // Add the start node to the stack.
        stack.Push(start);

        // While the stack is not empty, do the following:
        while (stack.Count > 0)
        {
            // Get the next node from the stack.
            Vector2 currentNode = stack.Peek();
            stack.Pop();
            //Debug.Log(currentNode);


            // If the node is the goal, then we have found the path.
            if (currentNode == end)
            {
                int numberOfVisitedNode = 0; 
                foreach (Vector2 v in emptyTiles)
                {
                    int currentX = (int)emptyTiles[numberOfVisitedNode].x;
                    int currentY = (int)emptyTiles[numberOfVisitedNode].y;                   
                }

                Debug.Log("dupa");
                

                // Return the path.
                return path;
            }

            // If the node has not been visited yet, then do the following:
            if (!visited.Contains(currentNode))
            {
                // Mark the node as visited.
                visited.Add(currentNode);
                if (map[(int)currentNode.x, (int)currentNode.y] == 0)
                {
                    emptyTiles.Add(currentNode);
                }
                // Add the node's neighbors to the stack.
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int x = (int)currentNode.x + i;
                        int y = (int)currentNode.y + j;
                        if (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && map[x, y] == 0 && !visited.Contains(new Vector2(x, y)))
                        {
                            //Debug.Log("X: " + x + "Y: " + y);
                            stack.Push(new Vector2(x, y));
                        }
                    }
                }
            }
        }

        // If we reach this point, then we have not found a path.
        return null;
    }

    public List<string> GetBestPath(int[,] map, Vector2 start, Vector2 end)
    {
        // Find the path.
        List<Vector2> path = FindPath(map, start, end);

        // If the path is not found, then return null.
        if (path == null)
        {
            return null;
        }

        // Create a list to store the directions.
        List<string> directions = new List<string>();

        // Iterate through the path, and add the directions to the list.
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.Log(path[i]);

            Vector2 currentPosition = path[i];
            Vector2 nextPosition = path[i + 1];

            // Calculate the direction from the current position to the next position.
            Vector2 direction = new Vector2((int)(nextPosition.x) - (int)(currentPosition.x), ((int)nextPosition.y - (int)(currentPosition.y)));

            // Add the direction to the list.
            directions.Add(GetDirection(direction));
        }

        if(directions.Count == 0)
        {
            Debug.Log("dupa ni mo");
        }
        foreach(string s in directions)
        {
            Debug.Log(s);
        }

        // Return the list of directions.
        return directions;
    }

    private string GetDirection(Vector2 direction)
    {
        if (direction.x == 0 && direction.y == -1)
        {
            return "Up";
        }
        else if (direction.x == 0 && direction.y == 1)
        {
            return "Down";
        }
        else if (direction.x == 1 && direction.y == 0)
        {
            return "Right";
        }
        else if (direction.x == -1 && direction.y == 0)
        {
            return "Left";
        }
        else return "";
    }
}

*/