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
        
        [TitleGroup("Movement")] 
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float fallSpeed = 1f;
        [SerializeField] private AnimationCurve pathMovementEase;
        [SerializeField] private AnimationCurve fallEase;
        
        [TitleGroup("Animation & FX")] 
        [SerializeField] private float wiggleDuration = 1f;
        [SerializeField] private float wiggleDistanceMultiplier = 1f;
        [SerializeField] private float mergeDuration = .5f;
        [FormerlySerializedAs("explosionVFXDelay")] [SerializeField] private float explosionFXDelay = .1f;

        [TitleGroup("Audio")] 
        [SerializeField] private AudioClip popSound;
        [SerializeField] private AudioClip placementSound;

        public AudioClip PopSound => popSound;
        public AudioClip PlacementSound => placementSound;
        public float DespawnDelay => despawnDelay;

        public IntColorPairs Colors => colors;
        public float MagazineScale => magazineScale;
        public float MovementSpeed => movementSpeed;
        public float FallSpeed => fallSpeed;
        public AnimationCurve PathMovementEase => pathMovementEase;
        public AnimationCurve FallEase => fallEase;
        public float WiggleDuration => wiggleDuration;
        public float WiggleDistanceMultiplier => wiggleDistanceMultiplier;
        public float MergeDuration => mergeDuration;
        public float ExplosionFXDelay => explosionFXDelay;
    }
}