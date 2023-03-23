using System.Collections.Generic;
using DG.Tweening;
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
        private bool _isTopTile;

        #endregion

        #region Accessors

        public Bubble Bubble => _bubble;
        public List<Tile> Neighbours => neighbours;
        public TextMeshPro TextMeshPro => textMeshPro;

        #endregion

        #region Init

        private void OnEnable()
        {
            SetColliderActive(false);
            spriteRenderer.enabled = false;
            spriteRenderer.gameObject.SetActive(true);
        }

        #endregion

        #region Public Methods

        public void SetBubble(Bubble bubble)
        {
            if (bubble == null) return;
            
            _bubble = bubble;

            SetColliderActive(false);
            SetNeighbourColliders();
        }

        public void ResetBubble()
        {
            if (_bubble == null) return;
            
            _bubble = null;
            CheckAndSetCollider();

            foreach (var neighbour in neighbours)
            {
                neighbour.CheckAndSetCollider();
            }
        }

        public void SetText(string text)
        {
            textMeshPro.text = text;
            gameObject.name = "Tile_" + text;
        }

        public void SetTopTile(bool isTopTile)
        {
            _isTopTile = isTopTile;
        }
        
        public void SetSpriteRendererActive(bool isActive, Color color = default)
        {
            spriteRenderer.enabled = isActive;
            if (isActive)
            {
                color.a = GameData.Instance.TileOpacity;
                spriteRenderer.color = color;
                spriteRenderer.transform.localScale = Vector3.zero;
                spriteRenderer.transform.DOScale(Vector3.one, GameData.Instance.TileColorAnimationDuration);
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
        
        // Disable tile collider if doesn't have any neighbour with bubble. Enable otherwise
        public void CheckAndSetCollider()
        {
            if (_bubble != null)
            {
                SetColliderActive(false);
                return;
            }
            
            var hasAnyNeighbourWithBubble = false;
            
            foreach (var neighbour in neighbours)
            {
                if (neighbour.Bubble != null)
                {
                    hasAnyNeighbourWithBubble = true;
                    break;
                }
            }
            
            SetColliderActive(hasAnyNeighbourWithBubble);
        } 
        
        public void SetNeighbourColliders()
        {
            foreach (var neighbour in neighbours)
                neighbour.SetColliderActive(neighbour.Bubble == null);
        }
        #endregion

        #region Private Methods

        private void SetColliderActive(bool isActive)
        {
            // Debug.Log(gameObject.name + "SetColliderActive: " + isActive);
            if (_isTopTile && _bubble == null) isActive = true;
            circleCollider.enabled = isActive;
            spriteRenderer_2.gameObject.SetActive(isActive);
        }

        #endregion
    }
}