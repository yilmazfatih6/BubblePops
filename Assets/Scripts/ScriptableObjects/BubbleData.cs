using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BubbleData", menuName = "ScriptableObjects/BubbleData", order = 0)]
    public class BubbleData : ScriptableObject
    {
        [SerializeField] private IntColorPairs colors;
        [SerializeField] private float magazineScale = .8f;
        [SerializeField] private float despawnDelay = 2f;
        
        #region Accessors

        public IntColorPairs Colors => colors;
        public float MagazineScale => magazineScale;
        public float DespawnDelay => despawnDelay;

        #endregion
        
        [TitleGroup("Movement")] 
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float fallSpeed = 1f;
        [SerializeField] private AnimationCurve pathMovementEase;
        [SerializeField] private AnimationCurve fallEase;
        
        #region Accessors
        
        public float MovementSpeed => movementSpeed;
        public float FallSpeed => fallSpeed;
        public AnimationCurve PathMovementEase => pathMovementEase;
        public AnimationCurve FallEase => fallEase;
        
        #endregion

        [TitleGroup("Animation & FX")] 
        [SerializeField] private float wiggleDuration = 1f;
        [SerializeField] private float wiggleDistanceMultiplier = 1f;
        [SerializeField] private float mergeDuration = .5f;
        [SerializeField] private float scaleUpDuration = .25f;
        [SerializeField] private float explosionFXDelay = .1f;

        #region Accessors
        
        public float WiggleDuration => wiggleDuration;
        public float WiggleDistanceMultiplier => wiggleDistanceMultiplier;
        public float MergeDuration => mergeDuration;
        public float ScaleUpDuration => scaleUpDuration;
        public float ExplosionFXDelay => explosionFXDelay;
       
        #endregion

    }
}