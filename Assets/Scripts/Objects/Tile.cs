using System;
using System.Collections.Generic;
using ScriptableObjects;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Objects
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer spriteRenderer_2;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField, ReadOnly] private List<Tile> neighbours = new List<Tile>();
        private Bubble _bubble;

        public Bubble Bubble => _bubble;
        public List<Tile> Neighbours => neighbours;

        private void OnEnable()
        {
            SetColliderActive(false);
        }

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

            SetColliderActive(true);
                
            foreach (var neighbour in neighbours)
            {
                if (neighbour.Bubble == null)
                {
                    var hasAnyNeighbourWithBubble = false;
                    foreach (var neighbourOfNeighbour in neighbour.Neighbours)
                    {
                        if (neighbourOfNeighbour.Bubble != null) hasAnyNeighbourWithBubble = true;
                    }
                        
                    if(!hasAnyNeighbourWithBubble) neighbour.SetColliderActive(false);
                }
            }
        }
        
        public void SetText(string text)
        {
            textMeshPro.text = text;
            gameObject.name = "Tile_" + text;
        }

        public void SetColliderActive(bool isActive)
        {
            circleCollider.enabled = isActive;
            spriteRenderer_2.gameObject.SetActive(isActive);
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
    }
}