using System;
using Objects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class TileSpawner : SingletonMonoBehaviour<TileSpawner>
    {
        public static event Action OnTileSpawnCompleted;

        [SerializeField] private Tile tilePrefab; // Prefab of the tile to spawn at each cell
        private Tile[,] _tiles; // Array to store the positions of each cell in the grid

        public Tile[,] Tiles => _tiles;

        #region Init

        private void OnEnable()
        {
            GridGenerator.OnGridGenerationCompleted += OnGridGenerationCompleted;
        }

        private void OnDisable()
        {
            GridGenerator.OnGridGenerationCompleted -= OnGridGenerationCompleted;
        }
        
        #endregion

        #region Callbacks

        private void OnGridGenerationCompleted()
        {
            SpawnTiles();
            SetNeighbours();
            OnTileSpawnCompleted?.Invoke();
        }

        #endregion
        
        private void SpawnTiles()
        {
            var rows = GridGenerator.Instance.Rows;
            var columns = GridGenerator.Instance.Columns;
            
            _tiles = new Tile[rows, columns];

            // Spawn a tile at each cell position in the grid
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    var tilePosition = GridGenerator.Instance.CellPositions[i, j];
                    var tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                    tile.SetText(i + "_" + j);
                    _tiles[i, j] = tile;
                
                }
            }
        }

        private void SetNeighbours()
        {
            var rows = GridGenerator.Instance.Rows;
            var columns = GridGenerator.Instance.Columns;
            
            // Spawn a tile at each cell position in the grid
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    var tile = _tiles[i, j];
                
                    // Add left neighbour
                    if (j - 1 >= 0)
                        tile.AddNeighbour(_tiles[i, j - 1]);
                
                    // Add right neighbour
                    if (j + 1 < columns)
                        tile.AddNeighbour(_tiles[i, j + 1]);

                    int leftIndex = j - 1;
                    int rightIndex = j + 1;

                    if (i % 2 == 0)
                        rightIndex = j;
                    else
                        leftIndex = j;

                    // Add top left neighbour
                    if (i + 1 < rows && leftIndex >= 0)
                        tile.AddNeighbour(_tiles[i + 1, leftIndex]);
                
                    // Add top right neighbour
                    if (i + 1 < rows && rightIndex < columns)
                        tile.AddNeighbour(_tiles[i + 1, rightIndex]);
                
                    // Add bottom left neighbour
                    if (i - 1 >= 0 && leftIndex >= 0)
                        tile.AddNeighbour(_tiles[i - 1, leftIndex]);
                
                    // Add bottom right neighbour
                    if (i - 1 >= 0 && rightIndex < columns)
                        tile.AddNeighbour(_tiles[i - 1, rightIndex]);
                }
            }
        }
    }
}