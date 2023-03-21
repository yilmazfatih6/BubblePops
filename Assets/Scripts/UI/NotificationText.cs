using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    public class NotificationText : SingletonMonoBehaviour<NotificationText>
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private DOTweenAnimation[] doTweenAnimations;

        public void Display(string str)
        {
            Debug.Log("NotificationText -> Display");
            text.text = str;
            PlayAnimations();
        }

        [Button]
        private void PlayAnimations()
        {
            foreach (var doTweenAnimation in doTweenAnimations)
            {
                doTweenAnimation.DORestart();
            }
        }
    }
}