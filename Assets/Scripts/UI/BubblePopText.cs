using System;
using DG.Tweening;
using Lean.Pool;
using ScriptableObjects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class BubblePopText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        private int _completedAnimationCount;

        private void OnValidate()
        {
            text = GetComponent<TextMeshPro>();
        }

        public void InjectData(string str)
        {
            text.text = str;

            // Scale up
            transform.localScale = Vector3.one;
            transform.DOScale(1.2f, GameData.Instance.BubblePopTextDuration);
            
            // Make relative movement.
            transform.DOMoveY(GameData.Instance.BubblePopTextMovement, GameData.Instance.BubblePopTextDuration).SetRelative();

            // Get color and make sure alpha is 1
            var color = text.color;
            color.a = 1f;

            // Reset color
            text.color = color;
            
            // Set target color alpha
            color.a = 0f;
            
            // Fade text.
            text.DOColor(color, GameData.Instance.BubblePopTextDuration / 2).SetDelay(GameData.Instance.BubblePopTextDuration / 2);
            
            // Despawn after delay.
            LeanPool.Despawn(this, GameData.Instance.BubblePopTextDuration);
        }
    }
}