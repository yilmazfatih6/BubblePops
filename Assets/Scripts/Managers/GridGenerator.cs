using System;
using Objects;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class GridGenerator : SingletonMonoBehaviour<GridGenerator>
    {
        public static event Action OnGridGenerationCompleted;
        private Vector2[,] _cellPositions; // Array to store the positions of each cell in the grid

        public Vector2[,] CellPositions => _cellPositions;

        private void Start()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            var columns = GameData.Instance.GridColumns;
            var rows = GameData.Instance.GridRows;
            var cellSize = GameData.Instance.CellSize;
            
            // Calculate the center point of the grid based on the position of the GameObject this script is attached to
            var centerPoint = transform.position;

            // Calculate the starting point of the grid based on the center point and the number of rows and columns
            var startX = centerPoint.x - ((columns / 2f) * cellSize);
            var startY = centerPoint.y - (rows * cellSize);

            // Initialize the cellPositions array with the correct dimensions
            _cellPositions = new Vector2[rows, columns];

            // Draw the grid lines and store the position of each cell
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    // Calculate the position of the current cell
                    var xPos = startX + (j * cellSize) + (cellSize / 2f);
                    var yPos = startY + (i * cellSize) + (cellSize / 2f);
                    var cellPosition = new Vector2(xPos, yPos);

                    // Store the position of the current cell in the cellPositions array
                    _cellPositions[i, j] = cellPosition;
                }
            }
            
            OnGridGenerationCompleted?.Invoke();
        }
    }
}
