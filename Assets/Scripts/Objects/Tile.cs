using System.Collections.Generic;
using ScriptableObjects;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Objects
{
    public class Tile : MonoBehaviour
    {
        #region Data

        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer spriteRenderer_2;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField, ReadOnly] private List<Tile> neighbours = new List<Tile>();
        private Bubble _bubble;

        #endregion

        #region Accessors

        public Bubble Bubble => _bubble;
        public List<Tile> Neighbours => neighbours;

        #endregion

        #region Init

        private void OnEnable()
        {
            SetColliderActive(false);
        }

        #endregion

        #region Public Methods

        public void SetBubble(Bubble bubble)
        {
            if (bubble == null) return;
            
            _bubble = bubble;

            SetColliderActive(false);

            foreach (var neighbour in neighbours)
                if(neighbour.Bubble == null) neighbour.SetColliderActive(true);
        }

        public void ResetBubble()
        {
            _bubble = null;

            CheckAndSetCollider();

            // // Check neighbour tiles and disable collider.
            // foreach (var neighbour in neighbours)
            // {
            //     if (neighbour.Bubble == null)
            //         neighbour.CheckDisableTile();
            // }
            
        }

        public void SetText(string text)
        {
            textMeshPro.text = text;
            gameObject.name = "Tile_" + text;
        }

        public void SetSpriteRendererActive(bool isActive, Color color = default)
        {
            spriteRenderer.gameObject.SetActive(isActive);
            if (isActive)
            {
                color.a = GameData.Instance.TileOpacity;
                spriteRenderer.color = color;
            }
        }
    
        public void AddNeighbour(Tile tile)
        {
            if(tile == null) return;
            if(neighbours.Contains(tile)) return;
        
            neighbours.Add(tile);
        }

        public void RemoveConnectedBubbles(List<Bubble> bubbles, out bool wasBubbleRemoved)
        {
            wasBubbleRemoved = false;

            if (_bubble == null) return;

            if (!bubbles.Contains(_bubble))
            {
                wasBubbleRemoved = true;
                return;
            }
            bubbles.Remove(_bubble);
            
            foreach (var neighbour in neighbours)
            {
                if(neighbour.Bubble == null) continue;
                if (!bubbles.Contains(neighbour.Bubble))
                {
                    wasBubbleRemoved = true;
                    continue;
                }
                neighbour.RemoveConnectedBubbles(bubbles, out wasBubbleRemoved);
                bubbles.Remove(neighbour.Bubble);
            }
        }
        #endregion

        #region Private Methods

        // Disable tile collider if doesn't have any neighbour with bubble. Enable otherwise
        private void CheckAndSetCollider()
        {
            var hasAnyNeighbourWithBubble = false;
            
            foreach (var neighbour in neighbours)
            {
                if (neighbour.Bubble != null)
                {
                    hasAnyNeighbourWithBubble = true;
                    break;
                }
            }
                        
            Debug.Log(name + " hasAnyNeighbourWithBubble " + hasAnyNeighbourWithBubble);
            SetColliderActive(hasAnyNeighbourWithBubble);
        } 
        
        private void SetColliderActive(bool isActive)
        {
            Debug.Log(gameObject.name + "SetColliderActive: " + isActive);
            circleCollider.enabled = isActive;
            spriteRenderer_2.gameObject.SetActive(isActive);
        }

        #endregion
    }
}