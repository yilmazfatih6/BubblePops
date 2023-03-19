using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using ScriptableObjects;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
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
            // Get merge bubble and store in a list.
            var bubbles = new List<Bubble> { this };  
            GetMergeBubbles(bubbles, _number);
            
            // If count is 1 which means there is no merge, return.
            if (bubbles.Count == 1) return;
            
            // Calculate new number to assign after merge.
            var newNumber = Mathf.Clamp(_number + bubbles.Count - 1, 
                GameData.Instance.BubbleData.Colors.First().Key,
                GameData.Instance.BubbleData.Colors.Last().Key);
            
            // Decide on which position to merge at.
            var positionToMergeAt = Vector3.zero;
            int maxCount = 0;
            int bubbleIndex = bubbles.Count - 1;
            for (int i = 0; i < bubbles.Count; i++)
            {
                var mergeBubbles = new List<Bubble> { this };
                bubbles[i].GetMergeBubbles(mergeBubbles, newNumber);
                if (maxCount < mergeBubbles.Count)
                {
                    maxCount = mergeBubbles.Count;
                    bubbleIndex = i;
                }
            }
            
            // Move each bubble, after movement bump at bubble at merge position and explode others.
            for (int i = 0; i < bubbles.Count; i++)
            {
                var tween = bubbles[i].MergeMove(bubbles[bubbleIndex].transform.position);

                if (i == bubbleIndex)
                {
                    var index = i;
                    tween.OnComplete(() =>
                    {
                        bubbles[index].BumpUp(newNumber);
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

        private void GetMergeBubbles(List<Bubble> matchedBubbles, int number)
        {
            foreach (var neighbourTile in _tile.Neighbours)
            {
                if(neighbourTile.Bubble == null) continue;
                if(number != neighbourTile.Bubble._number) continue;
                if(matchedBubbles.Contains(neighbourTile.Bubble)) continue;
                
                matchedBubbles.Add(neighbourTile.Bubble);
                neighbourTile.Bubble.GetMergeBubbles(matchedBubbles, number);
            }
        }
        #endregion

        #region Private Methods

        // private List<Bubble> GetMergeBubbles()
        // {
        //     var bubbles = new List<Bubble> { this };
        //     foreach (var neighbourTile in _tile.Neighbours)
        //     {
        //         if(neighbourTile.Bubble == null) continue;
        //         if(_number != neighbourTile.Bubble._number) continue;
        //         if(bubbles.Contains(neighbourTile.Bubble)) continue;
        //         
        //         bubbles.Add(neighbourTile.Bubble);
        //         neighbourTile.Bubble.GetMergeBubbles(bubbles);
        //     }
        //     return bubbles;
        // }

        private void BumpUp(int newNumber)
        {
            _number = newNumber;
            textMeshPro.text = Mathf.Pow(2, _number).ToString(CultureInfo.InvariantCulture);
            spriteRenderer.color = GameData.Instance.BubbleData.Colors[_number];
            
            // On max number is reached explode neighbours and self
            if (_number == GameData.Instance.BubbleData.Colors.Last().Key)
            {
                Explode();
                foreach (var neighbour in _tile.Neighbours)
                {
                    if (neighbour.Bubble)
                    {
                        neighbour.Bubble.Explode();
                    }
                }
            }
            else
            {
                // Else check for merge
                CheckForMerge();
            }
        }

        [Button]
        private void Debug_Bump()
        {
            BumpUp(_number + 1);
        }

        private Tween MergeMove(Vector3 position)
        {
            var distance = Vector3.Distance(transform.position, position);
            return transform.DOMove(position, GameData.Instance.BubbleData.MergeDuration);
        }
        
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