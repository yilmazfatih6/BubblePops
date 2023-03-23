using System;
using DG.Tweening;
using Objects;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class TileSpawner : SingletonMonoBehaviour<TileSpawner>
    {
        #region Events

        public static event Action OnTileSpawnCompleted;

        #endregion

        #region Data

        [SerializeField] private Tile tilePrefab;
        private Tile[][] _tiles;
        private bool _isTopAlignedLeft;

        #endregion

        #region Accessors

        public Tile[][] Tiles => _tiles;
        public Tile[] TopTiles => _tiles[GameData.Instance.GridRows - 1];
        public Tile[] BottomTiles => _tiles[0];

        #endregion

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

        #region Public Methods
        
        public bool IsRowEmpty(int index)
        {
            bool isEmpty = true;

            foreach (var tile in _tiles[index])
            {
                if (tile.Bubble == null) continue;
                isEmpty = false;
                break;
            }
            
            return isEmpty;
        }

        [Button]
        public Tween AddRow()
        {
            var columns = GameData.Instance.GridColumns;
            var rows = GameData.Instance.GridRows;
            
            // Move all tiles except bottom
            for (int i = 1; i < GameData.Instance.GridRows; i++)
            {
                for (int j = 0; j < GameData.Instance.GridColumns; j++)
                {
                    _tiles[i][j].transform.DOMoveY(-GameData.Instance.CellSize, GameData.Instance.TileMovementDuration).SetRelative();
                }
            }

            #region Teleport tiles from bottom to top

            for (int j = 0; j < GameData.Instance.GridColumns; j++)
            {
                var position = _tiles[GameData.Instance.GridRows - 1][j].transform.position;
                position.x = _tiles[0][j].transform.position.x;
                position.y += GameData.Instance.CellSize;
                _tiles[0][j].transform.position = position;
                
                _tiles[0][j].transform.DOMoveY(-GameData.Instance.CellSize, GameData.Instance.TileMovementDuration).SetRelative();
            }

            BubbleSpawner.Instance.SpawnBubblesToBottom();

            #endregion
            
            #region Swap tiles

            
            return DOVirtual.DelayedCall(GameData.Instance.TileMovementDuration, () =>
            {
                #region Swap Tiles

                // Reset top tile flag
                foreach (var tile in _tiles[rows - 1])
                    tile.SetTopTile(false);
                
                // Swap tiles.
                var temp = new Tile[columns];
                _tiles[0].CopyTo(temp, 0);

                for (var i = 1; i < rows; i++)
                {
                    _tiles[i].CopyTo(_tiles[i - 1], 0);
                }
            
                _tiles[rows - 1] = temp;
                
                // Reset top tile flag
                foreach (var tile in _tiles[rows - 1])
                    tile.SetTopTile(true);

                #endregion

                #region Set Neighbours
                
                _isTopAlignedLeft = !_isTopAlignedLeft;
                SetNeighboursInRow(0);
                SetNeighboursInRow(rows - 1);
                SetNeighboursInRow(rows - 2);

                foreach (var tile in _tiles[0])
                {
                    tile.CheckAndSetCollider();
                }

                foreach (var tile in _tiles[rows - 1])
                {
                    tile.SetNeighbourColliders();
                }

                // foreach (var tile in _tiles[rows - 2])
                //     tile.ResetBubble();
                
                #endregion

            });
       
            #endregion
        }
        
        [Button]
        public Tween RemoveRow()
        {
            var columns = GameData.Instance.GridColumns;
            var rows = GameData.Instance.GridRows;
            
            // Move all tiles.
            for (int i = 0; i < GameData.Instance.GridRows; i++)
            {
                for (int j = 0; j < GameData.Instance.GridColumns; j++)
                {
                    _tiles[i][j].transform.DOMoveY(GameData.Instance.CellSize, GameData.Instance.TileMovementDuration).SetRelative();
                }
            }

            // Set new tile positions.
            return DOVirtual.DelayedCall(GameData.Instance.TileMovementDuration, () =>
            {
                #region Move top tiles to bottom and despawn bubbles.
                
                for (var j = 0; j < columns; j++)
                {
                    var tile = _tiles[rows - 1][j];

                    var tileTransform = tile.transform;
                    var x = tileTransform.position.x;
                    var tilePosition = GridGenerator.Instance.CellPositions[0, j];
                    tilePosition.x = x;
                    tileTransform.position = tilePosition;
                    
                    if (tile.Bubble)
                    {
                        tile.Bubble.transform.SetParent(BubbleSpawner.Instance.transform);
                        tile.Bubble.Despawn();
                        BubbleSpawner.Instance.PoolBubbles.Remove(tile.Bubble);
                    }
                }

                #endregion
                
                #region Swap tiles

                // Reset top tile flag
                foreach (var tile in _tiles[rows - 1])
                    tile.SetTopTile(false);
                
                // Swap tiles.
                var temp = new Tile[columns];
                _tiles[rows - 1].CopyTo(temp, 0);

                for (var i = rows - 2; i >= 0; i--)
                {
                    _tiles[i].CopyTo(_tiles[i + 1], 0);
                }
            
                _tiles[0] = temp;
                
                // Reset top tile flag
                foreach (var tile in _tiles[rows - 1])
                    tile.SetTopTile(true);
                
                #endregion

                #region Set new neighbours

                _isTopAlignedLeft = !_isTopAlignedLeft;
                SetNeighboursInRow(0);
                SetNeighboursInRow(1);
                SetNeighboursInRow(rows - 1);

                // Set colliders
                foreach (var tile in _tiles[0])
                    tile.ResetBubble();
                // foreach (var tile in _tiles[1])
                //     tile.ResetBubble();
                // foreach (var tile in _tiles[rows - 1])
                //     tile.ResetBubble();

                #endregion
            });
        }
        #endregion

        #region Private Methods

        private void SpawnTiles()
        {
            var columns = GameData.Instance.GridColumns;
            var rows = GameData.Instance.GridRows;
            var cellSize = GameData.Instance.CellSize;

            _tiles = new Tile[rows][];

            // Spawn a tile at each cell position in the grid
            for (var i = 0; i < rows; i++)
            {
                _tiles[i] = new Tile[columns];
                
                for (var j = 0; j < columns; j++)
                {
                    var tilePosition = GridGenerator.Instance.CellPositions[i, j];
                    var tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                    
                    // Set position offset.
                    var pos = tile.transform.position;
                    if (i % 2 == 1) pos.x += GameData.Instance.GridOffset / 2;
                    else pos.x -= GameData.Instance.GridOffset / 2;
                    tile.transform.position = pos;
                    
                    // Set data to tile.
                    tile.SetText(i + "_" + j);
                    tile.SetTopTile(i == rows - 1);
                    
                    // Store tile.
                    _tiles[i][j] = tile;
                }
            }

        }

        private void SetNeighbours()
        {
            var columns = GameData.Instance.GridColumns;
            var rows = GameData.Instance.GridRows;
            
            // Spawn a tile at each cell position in the grid
            for (var i = 0; i < rows; i++)
            {
                SetNeighboursInRow(i);
            }
        }

        private void SetNeighboursInRow(int i)
        {
            var columns = GameData.Instance.GridColumns;
            var rows = GameData.Instance.GridRows;

            for (var j = 0; j < columns; j++)
            {
                var tile = _tiles[i][j];
                
                // Clear previous data.
                tile.Neighbours.Clear();
                
                // Add left neighbour
                if (j - 1 >= 0)
                    tile.AddNeighbour(_tiles[i][j - 1]);
                
                // Add right neighbour
                if (j + 1 < columns)
                    tile.AddNeighbour(_tiles[i][j + 1]);

                int leftIndex = j - 1;
                int rightIndex = j + 1;
                int modulusResult = _isTopAlignedLeft ? 1 : 0;
                if (i % 2 == modulusResult)
                    rightIndex = j;
                else
                    leftIndex = j;

                // Add top left neighbour
                if (i + 1 < rows && leftIndex >= 0)
                    tile.AddNeighbour(_tiles[i + 1][leftIndex]);
                
                // Add top right neighbour
                if (i + 1 < rows && rightIndex < columns)
                    tile.AddNeighbour(_tiles[i + 1][rightIndex]);
                
                // Add bottom left neighbour
                if (i - 1 >= 0 && leftIndex >= 0)
                    tile.AddNeighbour(_tiles[i - 1][leftIndex]);
                
                // Add bottom right neighbour
                if (i - 1 >= 0 && rightIndex < columns)
                    tile.AddNeighbour(_tiles[i - 1][rightIndex]);
            }
        }

        #endregion
    }
}