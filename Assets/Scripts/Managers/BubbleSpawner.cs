using System;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class BubbleSpawner : MonoBehaviour
    {
        [SerializeField] private Bubble bubblePrefab;
        [SerializeField] private int initialRowCount = 3;
        [SerializeField] private int maxExponent = 5;
        [SerializeField] private Transform[] bubbleSpawnPoints;
        
        private List<Bubble> _spawmedBubbles = new List<Bubble>();

        #region Init

        private void OnEnable()
        {
            TileSpawner.OnTileSpawnCompleted += OnTileSpawnCompleted;
        }

        private void OnDisable()
        {
            TileSpawner.OnTileSpawnCompleted -= OnTileSpawnCompleted;
        }
        
        #endregion

        #region Callbacks

        private void OnTileSpawnCompleted()
        { 
            var rows = GridGenerator.Instance.Rows;
            var columns = GridGenerator.Instance.Columns;

            for (int i = rows - 1; i >= rows - initialRowCount; i--)
            {
                for (int j = 0; j < columns; j++)
                {
                    Bubble bubble = Lean.Pool.LeanPool.Spawn(bubblePrefab);
                    _spawmedBubbles.Add(bubble);

                    var tile = TileSpawner.Instance.Tiles[i, j];
            
                    bubble.InjectData(tile: tile, number: (int)Mathf.Pow(2, Random.Range(1, maxExponent)));
                }
            }
            
        }

        #endregion

        #region Private Methods

        private void SpawnBubblesToShoot()
        {
            for (int i = 0; i < GridGenerator.Instance.MagazineSlots.Length; i++)
            {
                var bubble = Lean.Pool.LeanPool.Spawn(bubblePrefab);
                _spawmedBubbles.Add(bubble);
                var number = (int)Mathf.Pow(2, Random.Range(1, maxExponent));
                bubble.InjectData(number: number);
                // bubble.transform.position = bubbleSpawnPoints;
            }
        }

        #endregion
    }
}