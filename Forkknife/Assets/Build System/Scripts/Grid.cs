using System;
using UnityEngine;

public class Grid<TGridObject>
{

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;

    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    private bool showDebug = true;

    // Properties for encapsulation
    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public Vector3 OriginPosition => originPosition;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        InitializeGrid(createGridObject);
        if (showDebug) DebugDrawGrid();
    }

    private void InitializeGrid(Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    private void DebugDrawGrid()
    {
        // Drawing debug lines
        for (int x = 0; x <= Width; x++)
        {
            Debug.DrawLine(GetWorldPosition(x, 0), GetWorldPosition(x, Height), Color.white, 100f);
        }

        for (int y = 0; y <= Height; y++)
        {
            Debug.DrawLine(GetWorldPosition(0, y), GetWorldPosition(Width, y), Color.white, 100f);
        }

        OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
            // Update debug display if necessary
        };
    }


    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXY(worldPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public Vector3 GetOriginPosition()
    {
        return originPosition;
    }

    public bool IsCellValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
}
