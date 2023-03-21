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

        [TitleGroup("Bubble Spawn")]
        [SerializeField] private int initialRowCount = 2;
        [SerializeField] private int maxBubbleExponent = 5;
        
        public BubbleData BubbleData => bubbleData;
        public float TileOpacity => tileOpacity;
        public int InitialRowCount => initialRowCount;
        public int MaxBubbleExponent => maxBubbleExponent;
    }
}