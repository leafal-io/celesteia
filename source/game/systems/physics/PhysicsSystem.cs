using Celesteia.Game.Components;
using Celesteia.Game.Components.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Celesteia.Game.Systems.Physics {
    public class PhysicsSystem : EntityUpdateSystem {
        public const float GRAVITY_CONSTANT = 9.7f;

        public PhysicsSystem() : base(Aspect.All(typeof(PhysicsEntity), typeof(TargetPosition))) {}

        private ComponentMapper<TargetPosition> targetPositionMapper;
        private ComponentMapper<PhysicsEntity> physicsEntityMapper;

        public override void Initialize(IComponentMapperService mapperService)
        {
            targetPositionMapper = mapperService.GetMapper<TargetPosition>();
            physicsEntityMapper = mapperService.GetMapper<PhysicsEntity>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities) {
                TargetPosition targetPosition = targetPositionMapper.Get(entityId);
                PhysicsEntity physicsEntity = physicsEntityMapper.Get(entityId);

                // Apply gravity if applicable
                if (physicsEntity.Gravity) {
                    if (physicsEntity.CollidingDown && physicsEntity.Velocity.Y > 0f) {
                        physicsEntity.SetVelocity(physicsEntity.Velocity.X, 0.1f);
                    }
                    
                    physicsEntity.AddVelocity(0f, physicsEntity.Mass * PhysicsSystem.GRAVITY_CONSTANT * gameTime.GetElapsedSeconds());
                }

                targetPosition.Target += physicsEntity.Velocity * gameTime.GetElapsedSeconds();
            }
        }
    }
}