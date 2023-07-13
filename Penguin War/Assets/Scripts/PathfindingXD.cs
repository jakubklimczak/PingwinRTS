using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jump Point Search algorithm
public class Cell
{
    public int arrayCoordinateX { get; set; }
    public int arrayCoordinateY { get; set; }
    public int costOfMoving { get; set; }
    public int sumOfCosts { get; set; }
    public int costUsingCurrentCell => costOfMoving + sumOfCosts;
    public Cell Parent { get; set; }
    public bool isJumpPoint { get; set; } = false;
}

public class PathfindingXD : MonoBehaviour
{
    private int[,] grid;
    private int gridSize = 200;

    private void Start()
    {
    }

    public List<string> FindPath(int[,] grid, int originX, int originY, int destinationX, int destinationY)
    {
        this.grid = grid;

        Cell origin = new Cell { arrayCoordinateX = originX, arrayCoordinateY = originY, costOfMoving = 0, sumOfCosts = CalculateSumOfCosts(originX, originY, destinationX, destinationY), Parent = null };
        Cell destination = new Cell { arrayCoordinateX = destinationX, arrayCoordinateY = destinationY };

        List<Cell> unevaluatedCells = new List<Cell>();
        List<Cell> evaluatedCells = new List<Cell>();

        unevaluatedCells.Add(origin);

        while (unevaluatedCells.Count > 0)
        {
            Cell currentCell = GetLowestFCell(unevaluatedCells);

            if (currentCell.arrayCoordinateX == destination.arrayCoordinateX && currentCell.arrayCoordinateY == destination.arrayCoordinateY)
            {
                return GeneratePath(currentCell);
            }

            unevaluatedCells.Remove(currentCell);
            evaluatedCells.Add(currentCell);

            List<Cell> neighbors = GetNeighbors(currentCell);

            foreach (Cell neighbor in neighbors)
            {
                if (evaluatedCells.Contains(neighbor) || grid[neighbor.arrayCoordinateX, neighbor.arrayCoordinateY] > 0)
                {
                    continue;
                }

                int tentativeG = currentCell.costOfMoving + 1;

                if (!unevaluatedCells.Contains(neighbor) || tentativeG < neighbor.costOfMoving)
                {
                    neighbor.Parent = currentCell;
                    neighbor.costOfMoving = tentativeG;
                    neighbor.sumOfCosts = CalculateSumOfCosts(neighbor.arrayCoordinateX, neighbor.arrayCoordinateY, destinationX, destinationY);

                    if (!unevaluatedCells.Contains(neighbor))
                    {
                        unevaluatedCells.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateSumOfCosts(int startX, int startY, int endX, int endY)
    {
        return Math.Abs(endX - startX) + Math.Abs(endY - startY);
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        // Check for jump points in all 8 directions
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                {
                    continue;
                }

                int x = cell.arrayCoordinateX + dx;
                int y = cell.arrayCoordinateY + dy;

                Cell jumpPoint = Jump(cell, x, y, dx, dy);

                if (jumpPoint != null)
                {
                    neighbors.Add(jumpPoint);
                }
            }
        }

        return neighbors;
    }

    private Cell Jump(Cell parent, int x, int y, int dx, int dy)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize || grid[x, y] > 0)
        {
            return null;
        }

        Cell currentCell = new Cell { arrayCoordinateX = x, arrayCoordinateY = y, Parent = parent };

        if (currentCell.arrayCoordinateX == parent.arrayCoordinateX && currentCell.arrayCoordinateY == parent.arrayCoordinateY)
        {
            return currentCell;
        }

        if (currentCell.arrayCoordinateX == parent.arrayCoordinateX || currentCell.arrayCoordinateY == parent.arrayCoordinateY || IsDiagonalWalkable(currentCell.arrayCoordinateX, currentCell.arrayCoordinateY, dx, dy))
        {
            return currentCell;
        }

        if (dx != 0 && dy != 0)
        {
            // Check for forced neighbors
            if ((grid[x - dx, y + dy] == 0 && grid[x - dx, y] > 0) || (grid[x + dx, y - dy] == 0 && grid[x, y - dy] > 0))
            {
                return currentCell;
            }

            // Recurse in cardinal directions
            if (Jump(currentCell, x + dx, y, dx, 0) != null || Jump(currentCell, x, y + dy, 0, dy) != null)
            {
                return currentCell;
            }
        }
        else
        {
            // Recurse diagonally
            if (Jump(currentCell, x + dx, y, dx, 0) != null && Jump(currentCell, x, y + dy, 0, dy) != null)
            {
                return currentCell;
            }
        }

        return Jump(currentCell, x + dx, y + dy, dx, dy);
    }

    private bool IsDiagonalWalkable(int x, int y, int dx, int dy)
    {
        return (x >= 0 && x < gridSize && y >= 0 && y < gridSize && grid[x, y] == 0 && grid[x - dx, y] > 0 && grid[x, y - dy] > 0);
    }

    private Cell GetLowestFCell(List<Cell> cells)
    {
        Cell lowestFCell = cells[0];

        for (int i = 1; i < cells.Count; i++)
        {
            if (cells[i].costUsingCurrentCell < lowestFCell.costUsingCurrentCell)
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
            int deltaX = currentCell.arrayCoordinateX - currentCell.Parent.arrayCoordinateX;
            int deltaY = currentCell.arrayCoordinateY - currentCell.Parent.arrayCoordinateY;

            if (deltaX == -1 && deltaY == 0)
            {
                path.Add("Left");
            }
            else if (deltaX == 1 && deltaY == 0)
            {
                path.Add("Right");
            }
            else if (deltaY == -1 && deltaX == 0)
            {
                path.Add("Up");
            }
            else if (deltaY == 1 && deltaX == 0)
            {
                path.Add("Down");
            }

            currentCell = currentCell.Parent;
        }

        path.Reverse();
        return path;
    }
}




// Alternative A* algorithm
/*
public class Cell
{
    public int arrayCoordinateX { get; set; }
    public int arrayCoordinateY { get; set; }
    public int costOfMoving { get; set; }
    public int sumOfCosts { get; set; }
    public int costUsingCurrentCell => costOfMoving + sumOfCosts;
    public Cell Parent { get; set; }
}

public class PathfindingXD : MonoBehaviour
{
    private int gridSize = 200;

    private void Start()
    {
    }

    public List<string> FindPath(int [,] grid, int originX, int originY, int destinationX, int destinationY)
    {
        Cell origin = new Cell { arrayCoordinateX = originX, arrayCoordinateY = originY, costOfMoving = 0, sumOfCosts = CalculateSumOfCosts(originX, originY, destinationX, destinationY), Parent = null };
        Cell destination = new Cell { arrayCoordinateX = destinationX, arrayCoordinateY = destinationY };

        List<Cell> unevaluatedCells = new List<Cell>();
        List<Cell> evaluatedCells = new List<Cell>();

        unevaluatedCells.Add(origin);

        while (unevaluatedCells.Count > 0) 
        {
            Cell currentCell = GetLowestFCell(unevaluatedCells);

            if (currentCell.arrayCoordinateX == destination.arrayCoordinateX && currentCell.arrayCoordinateY == destination.arrayCoordinateY)
            {
                return GeneratePath(currentCell);
            }

            unevaluatedCells.Remove(currentCell);
            evaluatedCells.Add(currentCell);

            List<Cell> neighbors = GetNeighbors(currentCell);

            foreach (Cell neighbor in neighbors)
            {
                if (evaluatedCells.Contains(neighbor) || grid[neighbor.arrayCoordinateX, neighbor.arrayCoordinateY] > 0)
                {
                    continue;
                }

                int tentativeG = currentCell.costOfMoving + 1; //tu pokombinowaæ +0.001

                if (!unevaluatedCells.Contains(neighbor) || tentativeG < neighbor.costOfMoving)
                {
                    neighbor.Parent = currentCell;
                    neighbor.costOfMoving = tentativeG;
                    neighbor.sumOfCosts = CalculateSumOfCosts(neighbor.arrayCoordinateX, neighbor.arrayCoordinateY, destinationX, destinationY);

                    if (!unevaluatedCells.Contains(neighbor))
                    {
                        unevaluatedCells.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateSumOfCosts(int startX, int startY, int endX, int endY)
    {
        return Math.Abs(endX - startX) + Math.Abs(endY - startY);
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        if (cell.arrayCoordinateX > 0)
        {
            neighbors.Add(new Cell { arrayCoordinateX = cell.arrayCoordinateX - 1, arrayCoordinateY = cell.arrayCoordinateY });
        }

        if (cell.arrayCoordinateX < gridSize - 1)
        {
            neighbors.Add(new Cell { arrayCoordinateX = cell.arrayCoordinateX + 1, arrayCoordinateY = cell.arrayCoordinateY });
        }

        if (cell.arrayCoordinateY > 0)
        {
            neighbors.Add(new Cell { arrayCoordinateX = cell.arrayCoordinateX, arrayCoordinateY = cell.arrayCoordinateY - 1 });
        }

        if (cell.arrayCoordinateY < gridSize - 1)
        {
            neighbors.Add(new Cell { arrayCoordinateX = cell.arrayCoordinateX, arrayCoordinateY = cell.arrayCoordinateY + 1 });
        }

        return neighbors;
    }

    private Cell GetLowestFCell(List<Cell> cells)
    {
        Cell lowestFCell = cells[0];

        for (int i = 1; i < cells.Count; i++)
        {
            if (cells[i].costUsingCurrentCell < lowestFCell.costUsingCurrentCell)
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
            int deltaX = currentCell.arrayCoordinateX - currentCell.Parent.arrayCoordinateX;
            int deltaY = currentCell.arrayCoordinateY - currentCell.Parent.arrayCoordinateY;
            
            //Debug.Log("DeltaX: " + deltaX + ", DeltaY: " + deltaY);

            if (deltaX == -1 && deltaY == 0)
            {
                path.Add("Left");
                //Debug.Log("lewo");
            }
            else if (deltaX == 1 && deltaY == 0)
            {
                path.Add("Right");
                //Debug.Log("prawo");
            }
            else if (deltaY == -1 && deltaX == 0)
            {
                path.Add("Up");
                //Debug.Log("góra");
            }
            else if (deltaY == 1 && deltaX == 0)
            {
                path.Add("Down");
                //Debug.Log("dó³");
            }

            currentCell = currentCell.Parent;
        }

        path.Reverse();
        return path;
    }
}
*/