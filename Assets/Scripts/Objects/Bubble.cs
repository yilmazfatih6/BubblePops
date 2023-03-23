using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using Managers;
using ScriptableObjects;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    public class Bubble : MonoBehaviour
    {
        #region Events
        
        public static event Action<Bubble> OnExploded;
        public static event Action<Bubble> OnMergeNotFound;

        #endregion
        
        #region Data

        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer spriteRendererShadow;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private ParticleSystem explosionVFX;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private BubblePopText bubblePopText;
        
        private int _number;
        private Tile _tile;
        private Vector3 _defaultScale;
        private Tween _movementTween;
        
        #endregion

        #region Accessors

        public Color Color => GameData.Instance.BubbleData.Colors[_number];

        #endregion

        #region Init

        private void OnValidate()
        {
            textMeshPro = GetComponentInChildren<TextMeshPro>(true);
        }

        private void Awake()
        {
            _defaultScale = Vector3.one;
        }

        private void OnEnable()
        {
            spriteRenderer.enabled = true;
            spriteRendererShadow.enabled = true;
            textMeshPro.enabled = true;
            trailRenderer.emitting = false;
        }

        #endregion

        #region Public Methods

        public void InjectData(int number, Tile tile = null, bool isColliderActive = true, bool playScaleAnimation = false)
        {
            _number = number;
            textMeshPro.text = Mathf.Pow(2, _number).ToString(CultureInfo.InvariantCulture);

            if (tile != null)
            {
                _tile = tile;
                transform.position = _tile.transform.position;
                _tile.SetBubble(this);
                transform.SetParent(_tile.transform);
            }

            var color = GameData.Instance.BubbleData.Colors[number];
            spriteRenderer.color = color;
            trailRenderer.startColor = color;
            color.a = 0;
            trailRenderer.endColor = color;

            SetColliderActive(isColliderActive);

            if (playScaleAnimation)
            {
                ScaleAnimation();
            }
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

        public Tween Move(Vector3 position, float speed, AnimationCurve ease = null)
        {
            _movementTween?.Kill();
            // Debug.Log(gameObject.name + "Kill Movement Tween", gameObject);
            var distance = Vector3.Distance(transform.position, position);
            _movementTween = transform.DOMove(position, distance / speed);
            if (ease != null) _movementTween.SetEase(ease);
            return _movementTween;
        }
        
        public void ResetScale()
        {
            transform.DOScale(_defaultScale, .5f);
        }

        public void SetTrailActive(bool isActive)
        {
            trailRenderer.emitting = isActive;
        }

        public void SetTile(Tile tile)
        {
            _tile = tile;
            _tile.SetBubble(this);
            SetColliderActive(true);
            transform.SetParent(_tile.transform);
        }
        
        public void CheckForMerge()
        {
            // Get merge bubble and store in a list.
            var bubbles = new List<Bubble> { this };  
            GetMergeBubbles(bubbles, _number);
            
            // If count is 1 which means there is no merge, return.
            if (bubbles.Count <= 1)
            {
                OnMergeNotFound?.Invoke(this);
                BubbleShooter.Instance.CheckTiles();
                return;
            }
            
            // Calculate new number to assign after merge.
            var newNumber = Mathf.Clamp(_number + bubbles.Count - 1, 
                GameData.Instance.BubbleData.Colors.First().Key,
                GameData.Instance.BubbleData.Colors.Last().Key);
            
            // Debug.Log("GameData.Instance.BubbleData.Colors.First().Key: " + GameData.Instance.BubbleData.Colors.First().Key);
            // Debug.Log("GameData.Instance.BubbleData.Colors.Last().Key: " + GameData.Instance.BubbleData.Colors.Last().Key);

            // Decide on which position to merge at.
            var positionToMergeAt = Vector3.zero;
            int maxCount = 1;
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
                var tween = bubbles[i].MergeMove(bubbles[bubbleIndex].transform.position, i);

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
                        bubbles[index].Explode(index);
                    });
                }
            }
        }

        public void Fall()
        {
            var targetPosition = transform.position;
            targetPosition.y = BubbleShooter.Instance.Barrel.transform.position.y;

            if (_tile)
            {
                _tile.ResetBubble();
                _tile = null;
            }

            Move(targetPosition, GameData.Instance.BubbleData.FallSpeed, GameData.Instance.BubbleData.FallEase);
            _movementTween.onComplete += () => Explode();
            _movementTween.onComplete += () => PlayExplosionVFX();
        }

        public void Despawn()
        {
            if (!gameObject.activeSelf) return;
            _tile = null;
            spriteRenderer.enabled = false;
            spriteRendererShadow.enabled = false;
            textMeshPro.enabled = false;
            LeanPool.Despawn(this);
        }
        
        #endregion

        #region Private Methods

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

        private void BumpUp(int newNumber)
        {
            _number = newNumber;
            textMeshPro.text = Mathf.Pow(2, _number).ToString(CultureInfo.InvariantCulture);
            spriteRenderer.color = GameData.Instance.BubbleData.Colors[_number];

            var spawned = LeanPool.Spawn(bubblePopText, transform.position, transform.rotation);
            spawned.InjectData(textMeshPro.text);

            NotificationText.Instance.DisplayBumpUp();
            
            // On max number is reached explode neighbours and self
            if (_number == GameData.Instance.BubbleData.Colors.Last().Key)
            {
                for (var index = 0; index < _tile.Neighbours.Count; index++)
                {
                    var neighbour = _tile.Neighbours[index];
                    if (neighbour.Bubble)
                    {
                        neighbour.Bubble.Explode(index);
                    }
                }
                Explode();

                BubbleShooter.Instance.CheckTiles();
            }
            else
            {
                // Else check for merge
                CheckForMerge();
            }
        }
        
        private Tween MergeMove(Vector3 position, int index)
        {
            PlayExplosionVFX();
            
            _movementTween?.Kill();
            _movementTween = transform.DOMove(position, GameData.Instance.BubbleData.MergeDuration);
            return _movementTween;
        }

        private void PlayExplosionVFX(int index = 0)
        {
            var particleSystemMain = explosionVFX.main;
            particleSystemMain.startColor = spriteRenderer.color;
            particleSystemMain.startDelay = index * GameData.Instance.BubbleData.ExplosionFXDelay;
            explosionVFX.Play();
        }
        
        private void SetColliderActive(bool isActive)
        {
            circleCollider.enabled = isActive;
        }

        private void Explode(int index = 0)
        {
            if (!gameObject.activeSelf) return;

            // Debug.Log(gameObject + " Bubble -> Explode", gameObject);

            if (_tile)
            {
             
                _tile.ResetBubble();
                _tile = null;
            }

            spriteRenderer.enabled = false;
            spriteRendererShadow.enabled = false;
            textMeshPro.enabled = false;
            LeanPool.Despawn(this, GameData.Instance.BubbleData.DespawnDelay);
            AudioManager.Instance.PlayClip(GameData.Instance.BubblePopAudio, index * GameData.Instance.BubbleData.ExplosionFXDelay);
            OnExploded?.Invoke(this);
        }

        // Called when a bubble is placed to neighbour tile.
        private void Wiggle(Bubble bubble)
        {
            _movementTween?.Kill();
            // Debug.Log(gameObject.name + "Kill Movement Tween", gameObject);
            var distance = (transform.position - bubble.transform.position) * GameData.Instance.BubbleData.WiggleDistanceMultiplier;
            _movementTween = transform.DOLocalMove(duration: GameData.Instance.BubbleData.WiggleDuration, endValue: distance)
                .SetRelative()
                .SetLoops(2, LoopType.Yoyo);
        }

        public void WiggleNeighbours()
        {
            foreach (var neighbour in _tile.Neighbours)
            {
                // Debug.Log("neighbour: " + neighbour.name);
                if (neighbour.Bubble != null)
                {
                    // Debug.Log("neighbour Bubble: " + neighbour.Bubble.name);
                    neighbour.Bubble.Wiggle(this);
                }
            }
        }

        private void ScaleAnimation()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, GameData.Instance.BubbleData.ScaleUpDuration);
        }
        #endregion
        
        #region Debug
        
        [Button]
        private void Debug_Bump()
        {
            BumpUp(_number + 1);
        }
        
        #endregion
    }
}