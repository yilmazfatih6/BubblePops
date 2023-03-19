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
        [SerializeField] private float mergeDuration = .5f;
        public IntColorPairs Colors => colors;
        public float MagazineScale => magazineScale;
        public float MovementSpeed => movementSpeed;
        public float MergeDuration => mergeDuration;
    }
}