using Microsoft.Xna.Framework;

namespace Celesteia.Game.Components.Physics {
    public class PhysicsEntity {
        public float Mass;

        public bool Gravity;

        public bool CollidingUp;
        public bool CollidingLeft;
        public bool CollidingRight;
        public bool CollidingDown;

        public PhysicsEntity(float mass, bool affectedByGravity) {
            Mass = mass;
            Gravity = affectedByGravity;
        }

        private Vector2 _velocity;
        public Vector2 Velocity => _velocity;

        public void SetVelocity(Vector2 vector) {
            _velocity = vector;
        }

        public void SetVelocity(float x, float y) {
            SetVelocity(new Vector2(x, y));
        }

        public void AddVelocity(Vector2 vector) {
            _velocity += vector;
        }

        public void AddVelocity(float x, float y) {
            AddVelocity(new Vector2(x, y));
        }
    }
}