using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int rows = 10; // Number of rows in the grid
    [SerializeField] private int columns = 10; // Number of columns in the grid
    [SerializeField] private float offset = .5f; 
    [SerializeField] private float cellSize = 1f; // Size of each cell in the grid
    [SerializeField] private Tile tilePrefab; // Prefab of the tile to spawn at each cell
    private Vector2[,] cellPositions; // Array to store the positions of each cell in the grid
    private Tile[,] tiles; // Array to store the positions of each cell in the grid

    private void Start()
    {
        GenerateGrid();
        SpawnTiles();
        SetNeighbours();
    }

    private void GenerateGrid()
    {
        // Calculate the center point of the grid based on the position of the GameObject this script is attached to
        var centerPoint = transform.position;

        // Calculate the starting point of the grid based on the center point and the number of rows and columns
        var startX = centerPoint.x - ((columns / 2f) * cellSize) - offset / 2;
        var startY = centerPoint.y - ((rows / 2f) * cellSize);

        // Initialize the cellPositions array with the correct dimensions
        cellPositions = new Vector2[rows, columns];

        // Draw the grid lines and store the position of each cell
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                // Calculate the position of the current cell
                var xPos = startX + (j * cellSize) + (cellSize / 2f);
                if (i % 2 == 1) xPos += offset;
                var yPos = startY + (i * cellSize) + (cellSize / 2f);
                var cellPosition = new Vector2(xPos, yPos);

                // Store the position of the current cell in the cellPositions array
                cellPositions[i, j] = cellPosition;
            }
        }
    }

    private void SpawnTiles()
    {
        tiles = new Tile[rows, columns];

        // Spawn a tile at each cell position in the grid
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var tilePosition = cellPositions[i, j];
                var tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                tile.SetText(i + "_" + j);
                tiles[i, j] = tile;
                
            }
        }
    }

    private void SetNeighbours()
    {
        // Spawn a tile at each cell position in the grid
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var tile = tiles[i, j];
                
                // Add left neighbour
                if (j - 1 >= 0)
                    tile.AddNeighbour(tiles[i, j - 1]);
                
                // Add right neighbour
                if (j + 1 < columns)
                    tile.AddNeighbour(tiles[i, j + 1]);

                int leftIndex = j - 1;
                int rightIndex = j + 1;

                if (i % 2 == 0)
                    rightIndex = j;
                else
                    leftIndex = j;

                // Add top left neighbour
                if (i + 1 < rows && j - 1 >= 0)
                    tile.AddNeighbour(tiles[i + 1, leftIndex]);
                
                // Add top right neighbour
                if (i + 1 < rows && j + 1 < columns)
                    tile.AddNeighbour(tiles[i + 1, rightIndex]);
                
                // Add bottom left neighbour
                if (i - 1 >= 0 && j - 1 >= 0)
                    tile.AddNeighbour(tiles[i - 1, leftIndex]);
                
                // Add bottom right neighbour
                if (i - 1 >= 0 && j + 1 < columns)
                    tile.AddNeighbour(tiles[i - 1, rightIndex]);
            }
        }
    }
    
    
}
