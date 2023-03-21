using System;
using UnityEngine;
using Utilities;

namespace UI
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        [SerializeField] private Joystick joystick;
        
        public float HorizontalDirection => joystick.Horizontal;
        public Vector2 Direction => joystick.Direction;
    }
}