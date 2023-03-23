using System.Collections.Generic;
using DG.Tweening;
using ScriptableObjects;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    public class NotificationText : SingletonMonoBehaviour<NotificationText>
    {
        [SerializeField] private TextMeshPro text;
        private List<Tween> _tweens = new List<Tween>();
        private bool _isTweenPlaying;
        private int _bumpUpAmount;


        public void Display(string str)
        {
            text.text = str;
            PlayAnimations();
        }

        public void DisplayBumpUp()
        {
            if (!_isTweenPlaying)
            {
                _bumpUpAmount = 2;
            }
            else
            {
                _bumpUpAmount++;
            }
            
            Display(_bumpUpAmount + "X");
        }
        
        private void PlayAnimations()
        {
            foreach (var tween in _tweens)
                tween?.Kill();
            
            _tweens.Clear();

            _isTweenPlaying = true;
            Scale();
            Fade();
        }

        private void Scale()
        {
            // Scale up
            transform.localScale = Vector3.zero;
            var tween = transform.DOScale(1, GameData.Instance.BubblePopTextDuration).SetRelative();
            _tweens.Add(tween);
        }
        
        private void Fade()
        {
            // Get color and make sure alpha is 1
            var color = text.color;
            color.a = 1f;

            // Reset color
            text.color = color;
            
            // Set target color alpha
            color.a = 0f;
            
            // Fade text.
            var tween = text.DOColor(color, GameData.Instance.BubblePopTextDuration / 2).SetDelay(GameData.Instance.BubblePopTextDuration / 2);
            tween.onComplete += () => _isTweenPlaying = false;
            _tweens.Add(tween);
        }
    }
}