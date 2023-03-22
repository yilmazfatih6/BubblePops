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
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float wiggleDuration = 1f;
        [SerializeField] private float wiggleDistanceMultiplier = 1f;
        [SerializeField] private float mergeDuration = .5f;
        public IntColorPairs Colors => colors;
        public float MagazineScale => magazineScale;
        public float MovementSpeed => movementSpeed;
        public float WiggleDuration => wiggleDuration;
        public float WiggleDistanceMultiplier => wiggleDistanceMultiplier;
        public float MergeDuration => mergeDuration;
    }
}