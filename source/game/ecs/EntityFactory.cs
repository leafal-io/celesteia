using Celesteia.Resources;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Celesteia.Resources.Sprites;
using Celesteia.Game.Components.Player;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using Celesteia.Game.Input;
using Celesteia.Game.Components;
using Microsoft.Xna.Framework.Input;
using Celesteia.Game.Components.Physics;
using Celesteia.Game.Components.Items;
using Celesteia.Game.Components.Skybox;
using Celesteia.Resources.Types;
using Celesteia.Game.Input.Keyboard.Definitions;

namespace Celesteia.Game.ECS {
    /*
        Contains various commonly used prefabrications for entities.
        Many of the functions were moved to EntityTypes.
    */

    public class EntityFactory {
        private readonly World World;

        public EntityFactory(World world) {
            World = world;
        }

        public Entity CreateSkyboxPortion(string name, Color color, float rotation, float depth)
        {
            return new EntityBuilder(World)
                .AddComponent(new Transform2(Vector2.Zero, 0F, new Vector2(3f, 3f)))
                .AddComponent(new SkyboxRotateZ(rotation))
                .AddComponent(ResourceManager.Skybox.GetAsset(name).Frames.Clone().SetColor(color).SetDepth(depth))
                .Build();
        }

        public Entity CreateEntity(NamespacedKey key) => CreateEntity(ResourceManager.Entities.GetResource(key) as EntityType);

        public Entity CreateEntity(EntityType type)
        {
            Entity entity = World.CreateEntity();
            type.Instantiate(World, entity);
            
            return entity;
        }

        public static void BuildPlayer(Entity entity, Texture2D sprites) {
            entity.Attach(new Transform2());

            entity.Attach(new TargetPosition());

            entity.Attach(new EntityFrames(
                TextureAtlas.Create("player", sprites, 24, 24),
                0, 2,
                ResourceManager.SPRITE_SCALING
            ));

            entity.Attach(new EntityInventory(36, 
                new ItemStack(NamespacedKey.Base("iron_pickaxe"), 1),
                new ItemStack(NamespacedKey.Base("wooden_torch"), 10)
            ));

            entity.Attach(new PhysicsEntity(1f, true));

            entity.Attach(new CollisionBox(1.5f, 3f));

            entity.Attach(new PlayerInput() {
                Horizontal = {
                    new TrinaryKeyboardDefinition() { Negative = Keys.A, Positive = Keys.D, PollType = KeyPollType.Held }
                },
                Run = {
                    new BinaryKeyboardDefinition() { Keys = Keys.LeftShift, PollType = KeyPollType.Held }
                },
                Jump = {
                    new BinaryKeyboardDefinition() { Keys = Keys.Space, PollType = KeyPollType.Held }
                },
                Inventory = {
                    new BinaryKeyboardDefinition() { Keys = Keys.B, PollType = KeyPollType.Pressed }
                },
                Crafting = {
                    new BinaryKeyboardDefinition() { Keys = Keys.C, PollType = KeyPollType.Pressed }
                },
                Pause = {
                    new BinaryKeyboardDefinition() { Keys = Keys.Escape, PollType = KeyPollType.Pressed }
                }
            });

            entity.Attach(new LocalPlayer());

            entity.Attach(new CameraFollow());

            entity.Attach(new EntityAttributes(new EntityAttributes.EntityAttributeMap()
                .Set(EntityAttribute.MovementSpeed, 5f)
                .Set(EntityAttribute.JumpFuel, .5f)
                .Set(EntityAttribute.JumpForce, 10f)
                .Set(EntityAttribute.BlockRange, 7f)
            ));
        }
    }
}