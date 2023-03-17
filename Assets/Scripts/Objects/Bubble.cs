using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Objects
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private int _number;
        private Tile _tile;
    
        private void OnValidate()
        {
            textMeshPro = GetComponentInChildren<TextMeshPro>(true);
        }

        public void InjectData(int number, Tile tile = null)
        {
            _number = number;
            textMeshPro.text = number.ToString();

            if (tile != null)
            {
                _tile = tile;
                transform.position = _tile.transform.position;
            }

            spriteRenderer.color = GameData.Instance.BubbleData.Colors[number];
        }

        public void MoveToShotPosition(Transform target, bool isFirstSlot = false)
        {
            transform.position = target.transform.position;
            transform.rotation = Quaternion.identity;
            if (!isFirstSlot)
            {
                
            }
        }
    }
}