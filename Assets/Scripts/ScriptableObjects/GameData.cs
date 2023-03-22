using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 0)]
    public class GameData : SingletonScriptableObject<GameData>
    {
        [SerializeField, InlineEditor()] private BubbleData bubbleData;
        public BubbleData BubbleData => bubbleData;

        #region Tile Data

        [TitleGroup("Tile Data")]
        [SerializeField] private float tileOpacity = .25f;
        [SerializeField] private float tileColorAnimationDuration = .25f;
        public float TileOpacity => tileOpacity;
        public float TileColorAnimationDuration => tileColorAnimationDuration;

        #endregion

        #region Bubble Spawn

        [TitleGroup("Bubble Spawn")]
        [SerializeField] private int initialRowCount = 2;
        [SerializeField] private int maxBubbleExponent = 5;
        public int InitialRowCount => initialRowCount;
        public int MaxBubbleExponent => maxBubbleExponent;

        #endregion

        #region UI

        [TitleGroup("UI")] 
        [SerializeField] private string poolClearMessage = "Perfect!";
        [SerializeField] private float bubblePopTextDuration = .5f;
        [SerializeField] private float bubblePopTextMovement = 1f;
        public string PoolClearMessage => poolClearMessage;
        public float BubblePopTextDuration => bubblePopTextDuration;
        public float BubblePopTextMovement => bubblePopTextMovement;

        #endregion

        #region Audio

        [TitleGroup("Audio")]
        [SerializeField] private AudioClip gameStartAudio;
        [SerializeField] private AudioClip bubblePlacementAudio;
        [SerializeField] private AudioClip bubblePopAudio;

        public AudioClip GameStartAudio => gameStartAudio;
        public AudioClip BubblePlacementAudio => bubblePlacementAudio;
        public AudioClip BubblePopAudio => bubblePopAudio;

        #endregion

    }
}