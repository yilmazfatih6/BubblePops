using System.Collections.Generic;
using Objects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class BubbleBreaker : SingletonMonoBehaviour<BubbleBreaker>
    {
        private List<Bubble> _disconnectedBubbles = new List<Bubble>();

        public void Break()
        {
            _disconnectedBubbles = new List<Bubble>(BubbleSpawner.Instance.PoolBubbles);

            foreach (var topTile in TileSpawner.Instance.TopTiles)
            {
                topTile.RemoveConnectedBubbles(_disconnectedBubbles, out bool wasBubbleRemoved);
                // if (wasBubbleRemoved) continue;
            }

            foreach (var bubble in _disconnectedBubbles)
            {
                bubble.Fall();
            }
        }
    }
}