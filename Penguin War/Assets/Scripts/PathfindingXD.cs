using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;
    public Cell Parent { get; set; }
}

public class PathfindingXD : MonoBehaviour
{
    private int[,] grid;
    private int gridSize;


    public PathfindingXD(int[,] grid, int gridSize)
    {
        this.grid = grid;
        this.gridSize = gridSize;
    }

    public List<string> FindPath(int originX, int originY, int destinationX, int destinationY)
    {
        Cell origin = new Cell { X = originX, Y = originY, G = 0, H = CalculateH(originX, originY, destinationX, destinationY), Parent = null };
        Cell destination = new Cell { X = destinationX, Y = destinationY };

        List<Cell> openList = new List<Cell>();
        List<Cell> closedList = new List<Cell>();

        openList.Add(origin);

        while (openList.Count > 0)
        {
            Cell currentCell = GetLowestFCell(openList);

            if (currentCell.X == destination.X && currentCell.Y == destination.Y)
            {
                return GeneratePath(currentCell);
            }

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            List<Cell> neighbors = GetNeighbors(currentCell);

            foreach (Cell neighbor in neighbors)
            {
                if (closedList.Contains(neighbor) || grid[neighbor.X, neighbor.Y] > 0)
                {
                    continue;
                }

                int tentativeG = currentCell.G + 1;

                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.Parent = currentCell;
                    neighbor.G = tentativeG;
                    neighbor.H = CalculateH(neighbor.X, neighbor.Y, destinationX, destinationY);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateH(int startX, int startY, int endX, int endY)
    {
        return Math.Abs(endX - startX) + Math.Abs(endY - startY);
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        if (cell.X > 0)
        {
            neighbors.Add(new Cell { X = cell.X - 1, Y = cell.Y });
        }

        if (cell.X < gridSize - 1)
        {
            neighbors.Add(new Cell { X = cell.X + 1, Y = cell.Y });
        }

        if (cell.Y > 0)
        {
            neighbors.Add(new Cell { X = cell.X, Y = cell.Y - 1 });
        }

        if (cell.Y < gridSize - 1)
        {
            neighbors.Add(new Cell { X = cell.X, Y = cell.Y + 1 });
        }

        return neighbors;
    }

    private Cell GetLowestFCell(List<Cell> cells)
    {
        Cell lowestFCell = cells[0];

        for (int i = 1; i < cells.Count; i++)
        {
            if (cells[i].F < lowestFCell.F)
            {
                lowestFCell = cells[i];
            }
        }

        return lowestFCell;
    }

    private List<string> GeneratePath(Cell destination)
    {
        List<string> path = new List<string>();
        Cell currentCell = destination;

        while (currentCell.Parent != null)
        {
            int deltaX = currentCell.X - currentCell.Parent.X;
            int deltaY = currentCell.Y - currentCell.Parent.Y;

            if (deltaX == -1)
            {
                path.Add("Left");
            }
            else if (deltaX == 1)
            {
                path.Add("Right");
            }
            else if (deltaY == -1)
            {
                path.Add("Up");
            }
            else if (deltaY == 1)
            {
                path.Add("Down");
            }

            currentCell = currentCell.Parent;
        }

        path.Reverse();
        return path;
    }
}
