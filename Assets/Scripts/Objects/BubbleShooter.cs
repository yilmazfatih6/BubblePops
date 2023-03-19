using System;
using DG.Tweening;
using Managers;
using ScriptableObjects;
using UI;
using UnityEngine;
using Utilities;

namespace Objects
{
    public class BubbleShooter : SingletonMonoBehaviour<BubbleShooter>
    {
        [SerializeField] private Transform[] magazines;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform barrel;
        [SerializeField] private float maxAngle = 150f;
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask includeLayers;
        [SerializeField] private LayerMask tileLayer;
        private Vector2 _direction = Vector2.zero;
        private Vector3 _barrelRotation;
        private Tile _hitTile;
        private Bubble _currentBubble;
        private bool _isInputDown;
        private bool _canShoot = true;
        RaycastHit2D[] _results = new RaycastHit2D[10];

        public Transform[] Magazines => magazines;

        #region Init

        private void Awake()
        {
            lineRenderer.SetPosition(0, magazines[0].position);
            // _direction.y = directionY;
        }
        
        #endregion
        
        #region Unity Loop

        private void Update()
        {
            if (!_canShoot) return;
            var lerpTime = (UIManager.Instance.HorizontalDirection + 1) / 2f;
            _barrelRotation = barrel.rotation.eulerAngles;
            _barrelRotation.z = Mathf.Lerp(maxAngle, -maxAngle, lerpTime);
            barrel.rotation = Quaternion.Euler(_barrelRotation);

            if (Input.GetMouseButtonDown(0))
            {
                _isInputDown = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isInputDown = false;
                Shoot();
            }
            
        }
        
        private void FixedUpdate()
        {
            RayCast();
        }

        #endregion
        
        #region Public Methods

        #endregion

        #region Private Methods

        // ReSharper disable Unity.PerformanceAnalysis
        private void RayCast()
        {
            if (!_canShoot) return;
            if (!_isInputDown) return;
            if(_hitTile) _hitTile.SetSpriteRendererActive(false);
            
            var index = 1;
            Vector2 dir = barrel.up;
            Vector2 origin = magazines[0].position;
            while (true)
            {
                // Cast a ray straight down.
                var hit = Physics2D.Raycast(origin, dir, Mathf.Infinity, includeLayers);
                
                if (hit.collider != null)
                {
                    // Set line renderer position
                    lineRenderer.positionCount = index + 1;
                    lineRenderer.SetPosition(index, hit.point);

                    // Loop to bounce ray if wall is hit.
                    if (((1 << hit.collider.gameObject.layer) & wallLayerMask) != 0)
                    {
                        index = index + 1;

                        if (index >= 10) break;
                        
                        // Set direction and origin
                        dir = (hit.point - origin).normalized;
                        dir.x *= -1f;
                        origin = hit.point;
                        if (origin.x > 0) origin.x -= .01f;
                        else origin.x += .01f;
                        
                        continue;
                    }
                }
                
                break;
            }
            
            // Get furthest tile index.
            int hits = Physics2D.RaycastNonAlloc(origin, dir, _results, Mathf.Infinity, tileLayer);
            Debug.Log("Hits: " + hits);
            float furthestDistance = -1f;
            int furthestTileIndex = 0;
            for (int i = 0; i < hits; i++)
            {
                var distance = Vector3.Distance(_results[i].transform.position, origin);
                if (distance >= furthestDistance)
                {
                    furthestDistance = distance;
                    furthestTileIndex = i;
                }
            }
            
            // Get furthest tile.
            var tileHit = _results[furthestTileIndex];
            if (tileHit.collider != null)
            {
                _hitTile = tileHit.collider.gameObject.GetComponent<Tile>();
                _hitTile.SetSpriteRendererActive(true, BubbleSpawner.Instance.FirstShotBubble.Color);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void Shoot()
        {
            if (!_canShoot) return;

            if (!_hitTile)
            {
                Debug.LogWarning("[BubbleShooter.cs] Shoot(), No tile is hit, returning!");
                return;
            }

            _canShoot = false;
            
            // Set tile. 
            BubbleSpawner.Instance.FirstShotBubble.SetTile(_hitTile);
            
            // Move bubble along path
            MoveBubbleAlongPath(0);
        }

        private void MoveBubbleAlongPath(int index)
        {
            var position = lineRenderer.GetPosition(index);
            if (index == lineRenderer.positionCount - 1)
                position = _hitTile.transform.position;
            
            BubbleSpawner.Instance.FirstShotBubble.Move(position).OnComplete(() =>
            {
                index += 1;
                if (index >= lineRenderer.positionCount)
                {
                    BubbleSpawner.Instance.FirstShotBubble.CheckForMerge();
                    BubbleSpawner.Instance.SpawnNewShotBubble();
                    _canShoot = true;
                    return;
                }
                MoveBubbleAlongPath(index);
            });
        }
        
        #endregion
    }
}