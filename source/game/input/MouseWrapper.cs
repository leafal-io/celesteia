using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;

namespace Celesteia.Game.Input {
    public class MouseWrapper {
        private static MouseStateExtended _prev;
        private static MouseStateExtended _curr;

        public static void Update() {
            _prev = _curr;
            _curr = MouseExtended.GetState();
        }

        public static bool GetMouseDown(MouseButton button) {
            return _prev.IsButtonUp(button) && _curr.IsButtonDown(button);
        }

        public static bool GetMouseUp(MouseButton button) {
            return _prev.IsButtonDown(button) && _curr.IsButtonUp(button);
        }

        public static bool GetMouseHeld(MouseButton button) {
            return _curr.IsButtonDown(button);
        }

        public static Point GetPosition() {
            return _curr.Position;
        }

        public static int GetScrollDelta() {
            int delta = _curr.ScrollWheelValue - _prev.ScrollWheelValue;
            return delta == 0 ? 0 : (delta < 0 ? -1 : 1);
        }
    }
}