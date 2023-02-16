using System;
using System.Diagnostics;
using Celestia.GameInput;
using Celestia.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Celestia.Screens {
    public class SplashScreen : GameScreen {
        private new Game Game => (Game) base.Game;

        Texture2D leafalLogo;
        SoundEffect splashSound;
        
        Image backgroundImage;
        Image logoElement;
        Rect logoRect;

        private float logoRatio;

        public SplashScreen(Game game) : base(game) {}

        public override void LoadContent()
        {
            base.LoadContent();

            leafalLogo = Game.Content.Load<Texture2D>("branding/leafal/leafal_text_logo");
            splashSound = Game.Content.Load<SoundEffect>("branding/leafal/splash");

            logoRatio = leafalLogo.Height / (float) leafalLogo.Width;

            backgroundImage = new Image(new Rect(
                new ScreenSpaceUnit(0f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(0f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical),
                new ScreenSpaceUnit(1f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(1f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical)
            ), null, Color.Black, 0f);

            logoRect = new Rect(
                new ScreenSpaceUnit(0.25f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(0.5f - (logoRatio / 2f), ScreenSpaceUnit.ScreenSpaceOrientation.Vertical),
                new ScreenSpaceUnit(0.5f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(logoRatio * 0.5f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal)
            );
            logoElement = new Image(logoRect, leafalLogo, Color.White, 1f);

            splashSound.Play(0.5f, 0f, 0f);
        }
        
        private float timeElapsed = 0f;
        private float fadeInTime = 0.25f;
        private float fadeOutTime = 0.75f;
        private float duration = 3f;
        private float endTimeout = 1f;
        private float progress = 0f;
        private Color color = Color.White;

        public override void Update(GameTime gameTime) {
            if (progress >= 1f || Input.GetAny()) {
                Game.LoadScreen(new MainMenuScreen(Game));
                return;
            }

            timeElapsed += (float) (gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);
            float alpha = 1f;
            if (timeElapsed <= fadeInTime) alpha = Math.Min(timeElapsed / fadeInTime, 1f);
            if (duration - fadeOutTime <= timeElapsed) alpha = Math.Max((duration - timeElapsed) / fadeOutTime, 0f);

            color.A = (byte) ((int) (alpha * 255));

            progress = timeElapsed / (duration + endTimeout);
            UpdateLogoRect(progress);
        }

        private float growFactor = 0f;
        private void UpdateLogoRect(float progress) {
            Rect r = logoElement.GetRect();

            r.X.SetValue(0.25f - (progress * (growFactor / 2f)));
            r.Y.SetValue(0.5f - (logoRatio / 2f) - ((logoRatio / 2f) * progress * (growFactor / 2f)));
            r.Width.SetValue(0.5f + (progress * growFactor));
            r.Height.SetValue(0.5f * logoRatio + ((progress * growFactor * logoRatio)));

            logoElement.SetRect(r);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicWrap);

            backgroundImage.Draw(Game.SpriteBatch);
            
            logoElement.color = color;
            logoElement.Draw(Game.SpriteBatch);

            Game.SpriteBatch.End();
        }

        public override void Dispose()
        {
            Debug.WriteLine("Unloading SplashScreen content...");
            base.UnloadContent();
            Debug.WriteLine("Disposing SplashScreen...");
            base.Dispose();
        }
    }
}