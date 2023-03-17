// using Sirenix.OdinInspector;
// using UnityEngine;
//
// [ExecuteAlways]
// public class CameraAdjuster : MonoBehaviour
// {
//     [SerializeField] private GridGenerator gridGenerator;
//     [SerializeField] new Camera camera;
//     // [SerializeField] Vector3 offset;
//
//     private void OnValidate()
//     {
//         gridGenerator = FindObjectOfType<GridGenerator>();
//         camera = GetComponent<Camera>();
//             
//         if (gridGenerator)
//         {
//             gridGenerator.OnGridGenerated += OnGridGenerated;
//         }
//     }
//
//     private void OnDisable()
//     {
//         if (gridGenerator)
//         {
//             gridGenerator.OnGridGenerated -= OnGridGenerated;
//         }
//     }
//
//     private void LateUpdate()
//     {
//         AdjustCamera();
//     }
//
//     private void OnGridGenerated()
//     {
//         AdjustCamera();
//     }
//
//     [Button]
//     private void AdjustCamera()
//     {
//         if (!camera) return;
//         if (!gridGenerator) return;
//             
//         // transform.position = new Vector3(0, 0, gridGenerator.transform.position.z) + offset;
//         camera.orthographicSize = CalculateOrthographicSize();
//     }
//     
//
//     private float CalculateOrthographicSize()
//     {
//         float orthographicSize = camera.orthographicSize;
//         Vector3 topRight = new Vector3(gridGenerator.transform.position.x + gridGenerator.GridSizeX / 2, gridGenerator.transform.position.y + gridGenerator.GridSizeY / 2,  0);
//         Vector3 topRightAsViewport = camera.WorldToViewportPoint(topRight);
//         if (topRightAsViewport.x >= topRightAsViewport.y)
//             orthographicSize = Mathf.Abs(gridGenerator.GridSizeX) / camera.aspect / 2f;
//         else
//             orthographicSize = Mathf.Abs(gridGenerator.GridSizeX) / 2f;
//         return orthographicSize;
//     }
// }