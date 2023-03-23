using Celesteia.Game.Components;
using Celesteia.Game.Worlds;
using Celesteia.Resources;
using Celesteia.Resources.Types;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Celesteia.Game {
    public class TileEntityItemActions : ItemActions {
        private NamespacedKey _entityKey;
        private TileEntityType _tileEntity = null;

        public TileEntityItemActions(NamespacedKey entityKey) {
            UseTime = 0.2;
            _entityKey = entityKey;
        }
        
        private void TryQualify() {
            //_tileEntity = ResourceManager.TileEntities.GetResource(_entityKey) as TileEntityType;
        }
        
        public override bool OnLeftClick(GameTime gameTime, GameWorld world, Vector2 cursor, Entity user) {
            TryQualify();
            return Assert(gameTime, world, cursor, user, false) && Place(world, cursor, false);
        }
        
        public override bool OnRightClick(GameTime gameTime, GameWorld world, Vector2 cursor, Entity user) {
            return false;
        }

        public virtual bool Assert(GameTime gameTime, GameWorld world, Vector2 cursor, Entity user, bool forWall) {
            if (_tileEntity != null) return false;
            if (!CheckUseTime(gameTime)) return false;

            if (!user.Has<Transform2>() || !user.Has<EntityAttributes>()) return false;

            Transform2 entityTransform = user.Get<Transform2>();
            EntityAttributes.EntityAttributeMap attributes = user.Get<EntityAttributes>().Attributes;

            if (Vector2.Distance(entityTransform.Position, cursor) > attributes.Get(EntityAttribute.BlockRange)) return false;

            for (int i = 0; i < _tileEntity.Bounds.X; i++) {
                for (int j = 0; j < _tileEntity.Bounds.Y; j++) {
                    if (world.GetBlock(cursor.ToPoint().X - _tileEntity.Origin.X + i, cursor.ToPoint().Y - _tileEntity.Origin.Y + j) != 0) return false;
                }
            }

            /*if (!world.GetAnyBlock(cursor + new Vector2(-1, 0), true) && 
                !world.GetAnyBlock(cursor + new Vector2(1, 0), true) &&
                !world.GetAnyBlock(cursor + new Vector2(0, -1), true) && 
                !world.GetAnyBlock(cursor + new Vector2(0, 1), true)) {
                if (!forWall && world.GetWallBlock(cursor) == 0) return false;
                else if (forWall && world.GetBlock(cursor) == 0) return false;
            }*/
            
            UpdateLastUse(gameTime);
            return true;
        }

        public bool Place(GameWorld world, Vector2 cursor, bool wall) {
            return true;
        }
    }
}