// Vstup funguje v obou nastaveních Unity (starý Input Manager i nový Input System).
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace Pivovar
{
    public static class Inp
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        public static Vector2 MousePos() { return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero; }
        public static Vector2 MouseDelta() { return Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero; }
        public static float Scroll() { return Mouse.current != null ? Mouse.current.scroll.ReadValue().y / 120f : 0f; }
        public static bool Left() { return Mouse.current != null && Mouse.current.leftButton.isPressed; }
        public static bool Right() { return Mouse.current != null && Mouse.current.rightButton.isPressed; }
        public static bool Middle() { return Mouse.current != null && Mouse.current.middleButton.isPressed; }
        static Key Map(string k)
        {
            switch (k)
            {
                case "W": return Key.W; case "A": return Key.A; case "S": return Key.S; case "D": return Key.D;
                case "Q": return Key.Q; case "V": return Key.V; case "E": return Key.E; case "F": return Key.F; case "X": return Key.X;
                case "Shift": return Key.LeftShift; default: return Key.None;
            }
        }
        public static bool Held(string k) { var kb = Keyboard.current; return kb != null && kb[Map(k)].isPressed; }
        public static bool Pressed(string k) { var kb = Keyboard.current; return kb != null && kb[Map(k)].wasPressedThisFrame; }
#else
        public static Vector2 MousePos() { return Input.mousePosition; }
        public static Vector2 MouseDelta() { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f; }
        public static float Scroll() { return Input.mouseScrollDelta.y; }
        public static bool Left() { return Input.GetMouseButton(0); }
        public static bool Right() { return Input.GetMouseButton(1); }
        public static bool Middle() { return Input.GetMouseButton(2); }
        static KeyCode Map(string k)
        {
            switch (k)
            {
                case "W": return KeyCode.W; case "A": return KeyCode.A; case "S": return KeyCode.S; case "D": return KeyCode.D;
                case "Q": return KeyCode.Q; case "V": return KeyCode.V; case "E": return KeyCode.E; case "F": return KeyCode.F; case "X": return KeyCode.X;
                case "Shift": return KeyCode.LeftShift; default: return KeyCode.None;
            }
        }
        public static bool Held(string k) { return Input.GetKey(Map(k)); }
        public static bool Pressed(string k) { return Input.GetKeyDown(Map(k)); }
#endif
    }
}
