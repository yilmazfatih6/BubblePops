using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public void ReloadLevel()
        {
            DOTween.KillAll();
            var index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }
    }
}