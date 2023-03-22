using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        [SerializeField] private AudioSource audioSource;

        public void PlayClip(AudioClip clip, float delay = 0)
        {
            DOVirtual.DelayedCall(delay, () => audioSource.PlayOneShot(clip));
        }
        
    }
}