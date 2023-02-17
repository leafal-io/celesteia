using System.Diagnostics;
using Celestia.Graphics;
using Celestia.Resources.Sprites;
using Celestia.Screens.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Celestia.Screens.Systems.MainMenu {
    public class MainMenuRenderSystem : EntityDrawSystem
    {
        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<SkyboxPortionFrames> framesMapper;

        private Camera2D _camera;
        private SpriteBatch _spriteBatch;

        public MainMenuRenderSystem(Camera2D camera, SpriteBatch spriteBatch) : base(Aspect.All(typeof(Transform2), typeof(SkyboxPortionFrames))) {
            _camera = camera;
            _spriteBatch = spriteBatch;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            framesMapper = mapperService.GetMapper<SkyboxPortionFrames>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _camera.GetViewMatrix());
            
            foreach (int entityId in ActiveEntities) {
                SkyboxPortionFrames frames = framesMapper.Get(entityId);
                Transform2 transform = transformMapper.Get(entityId);

                frames.Draw(0, _spriteBatch, transform.Position, transform.Rotation, transform.Scale);
            }

            _spriteBatch.End();
        }
    }
}