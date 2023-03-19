using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 0)]
    public class GameData : SingletonScriptableObject<GameData>
    {
        [SerializeField, InlineEditor()] private BubbleData bubbleData;
        [SerializeField] private float tileOpacity = .25f;
        public BubbleData BubbleData => bubbleData;
        public float TileOpacity => tileOpacity;
    }
}