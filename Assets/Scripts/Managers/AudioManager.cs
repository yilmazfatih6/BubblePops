using System;
using DG.Tweening;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        [SerializeField] private AudioSource audioSource;

        private void Start()
        {
            PlayClip(GameData.Instance.GameStartAudio, 1f);
        }

        public void PlayClip(AudioClip clip, float delay = 0)
        {
            DOVirtual.DelayedCall(delay, () => audioSource.PlayOneShot(clip));
        }
        
    }
}