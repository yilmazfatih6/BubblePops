using System;
using System.Collections.Generic;
using Lean.Pool;
using Objects;
using ScriptableObjects;
using UI;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public class BubbleSpawner : SingletonMonoBehaviour<BubbleSpawner>
    {
        [SerializeField] private Bubble bubblePrefab;
        
        private List<Bubble> _poolBubbles = new List<Bubble>();
        private List<Bubble> _shotBubbles = new List<Bubble>();

        public Bubble FirstShotBubble => _shotBubbles[0];

        public List<Bubble> PoolBubbles => _poolBubbles;

        #region Init

        private void OnEnable()
        {
            TileSpawner.OnTileSpawnCompleted += OnTileSpawnCompleted;
            Bubble.OnExploded += OnBubbleExploded;
        }

        private void OnDisable()
        {
            TileSpawner.OnTileSpawnCompleted -= OnTileSpawnCompleted;
            Bubble.OnExploded -= OnBubbleExploded;
        }

        #endregion

        #region Callbacks

        private void OnBubbleExploded(Bubble obj)
        {
            if (_poolBubbles.Contains(obj))
            {
                _poolBubbles.Remove(obj);
                if (_poolBubbles.Count == 0)
                    NotificationText.Instance.Display(GameData.Instance.PoolClearMessage);
            }
        }

        #endregion

        #region Callbacks

        private void OnTileSpawnCompleted()
        { 
            SpawnPoolBubbles();
            SpawnInitialShotBubbles();
        }

        #endregion

        #region Public Methods

        // Spawns a new shot bubble and swaps secondary to first
        public void SpawnNewShotBubble()
        {
            _shotBubbles.RemoveAt(0);
            FirstShotBubble.Move(BubbleShooter.Instance.Magazines[0].position);
            FirstShotBubble.ResetScale();
            SpawnShotBubble(1);
        }
        
        #endregion

        #region Private Methods

        private void SpawnPoolBubbles()
        {
            var rows = GridGenerator.Instance.Rows;
            var columns = GridGenerator.Instance.Columns;

            for (int i = rows - 1; i >= rows - GameData.Instance.InitialRowCount; i--)
            {
                for (int j = 0; j < columns; j++)
                {
                    Bubble bubble = Lean.Pool.LeanPool.Spawn(bubblePrefab);
                    _poolBubbles.Add(bubble);

                    var tile = TileSpawner.Instance.Tiles[i, j];
            
                    bubble.InjectData(tile: tile, number: Random.Range(1, GameData.Instance.MaxBubbleExponent));
                }
            }
        }

        private void SpawnInitialShotBubbles()
        {
            for (int i = 0; i < BubbleShooter.Instance.Magazines.Length; i++)
            {
                SpawnShotBubble(i);
            }
        }

        private void SpawnShotBubble(int index)
        {
            var bubble = LeanPool.Spawn(bubblePrefab, transform);
            _shotBubbles.Add(bubble);
            var number = (int)Random.Range(1, GameData.Instance.MaxBubbleExponent);
            bubble.InjectData(number: number, isColliderActive:false);
            bubble.SetTransform(BubbleShooter.Instance.Magazines[index], index == 0);
        }

        #endregion

    }
}