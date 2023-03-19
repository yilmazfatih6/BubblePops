using System;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Lean.Pool;
using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Objects
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CircleCollider2D circleCollider;
        private int _number;
        private Tile _tile;
        private Vector3 _defaultScale;

        public Color Color => GameData.Instance.BubbleData.Colors[_number];

        #region Init

        private void OnValidate()
        {
            textMeshPro = GetComponentInChildren<TextMeshPro>(true);
        }

        private void Awake()
        {
            _defaultScale = Vector3.one;
        }

        #endregion

        #region Public Methods

        public void InjectData(int number, Tile tile = null, bool isColliderActive = true)
        {
            _number = number;
            textMeshPro.text = Mathf.Pow(2, _number).ToString(CultureInfo.InvariantCulture);

            if (tile != null)
            {
                _tile = tile;
                transform.position = _tile.transform.position;
                _tile.SetBubble(this);
            }

            spriteRenderer.color = GameData.Instance.BubbleData.Colors[number];

            SetColliderActive(isColliderActive);
        }
        
        public void SetTransform(Transform target, bool isFirstSlot = false)
        {
            transform.position = target.transform.position;
            transform.rotation = Quaternion.identity;
            if (!isFirstSlot)
            {
                transform.localScale = GameData.Instance.BubbleData.MagazineScale * Vector3.one;
            }
        }

        public Tween Move(Vector3 position)
        {
            var distance = Vector3.Distance(transform.position, position);
            return transform.DOMove(position, distance / GameData.Instance.BubbleData.MovementSpeed);
        }
        
        public Tween MergeMove(Vector3 position)
        {
            var distance = Vector3.Distance(transform.position, position);
            return transform.DOMove(position, GameData.Instance.BubbleData.MergeDuration);
        }

        public void ResetScale()
        {
            transform.DOScale(_defaultScale, .5f);
        }

        public void SetTile(Tile tile, Vector3[] pathPositions = null)
        {
            _tile = tile;
            _tile.SetBubble(this);
            SetColliderActive(true);
        }

        public void CheckForMerge()
        {
            var bubbles = new List<Bubble> { this };
            foreach (var neighbourTile in _tile.Neighbours)
            {
                if(neighbourTile.Bubble == null) continue;
                if(_number != neighbourTile.Bubble._number) continue;
                if(bubbles.Contains(neighbourTile.Bubble)) continue;
                
                bubbles.Add(neighbourTile.Bubble);
                neighbourTile.Bubble.GetMergeBubbles(bubbles);
                break;
            }

            if (bubbles.Count == 1) return;
            
            for (int i = 0; i < bubbles.Count; i++)
            {
                var tween = bubbles[i].MergeMove(bubbles[^1].transform.position);

                if (i == bubbles.Count - 1)
                {
                    var index = i;
                    tween.OnComplete(() =>
                    {
                        bubbles[index].BumpUp();
                    });
                }
                else
                {
                    var index = i;
                    tween.OnComplete(() =>
                    {
                        bubbles[index].Explode();
                    });
                }
            }
        }

        private void GetMergeBubbles(List<Bubble> matchedBubbles)
        {
            foreach (var neighbourTile in _tile.Neighbours)
            {
                if(neighbourTile.Bubble == null) continue;
                if(_number != neighbourTile.Bubble._number) continue;
                if(matchedBubbles.Contains(neighbourTile.Bubble)) continue;
                
                matchedBubbles.Add(neighbourTile.Bubble);
                neighbourTile.Bubble.GetMergeBubbles(matchedBubbles);
                break;
            }
            
            // isMatched = _number == _number;
            // if (isMatched)
            // {
            //     bubble = neighbourTile.Bubble;
            // }
            // if (isMatched)
            // {
            //     // Merge
            //     neighbourTile.Bubble.CheckForMerge();
            //     Move(neighbourTile.transform.position).OnComplete(() =>
            //     {
            //         Explode();
            //         neighbourTile.Bubble.BumpUp();
            //     });
            //
            //     return;
            // }
        }

        public void BumpUp()
        {
            _number += 1;
            textMeshPro.text = Mathf.Pow(2, _number).ToString(CultureInfo.InvariantCulture);
            spriteRenderer.color = GameData.Instance.BubbleData.Colors[_number];
            CheckForMerge();
        } 
        #endregion

        #region Private Methods
        
        private void SetColliderActive(bool isActive)
        {
            circleCollider.enabled = isActive;
        }

        private void Explode()
        {
            _tile.ResetBubble();
            LeanPool.Despawn(this);
        }
        #endregion
    }
}