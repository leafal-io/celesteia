using System.Diagnostics;
using Celestia.GameInput;
using Celestia.UI;
using Celestia.GUIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celestia.Screens {
    public class MainMenuScreen : IScreen
    {
        private Game gameRef;
        private GUI mainMenu;
        private Texture2D logo;
        private SpriteFont arialBold;

        public MainMenuScreen(Game gameRef) {
            this.gameRef = gameRef;

            this.mainMenu = new GUI();
        }

        public void Load(ContentManager contentManager)
        {
            logo = contentManager.Load<Texture2D>("celestia/logo");
            arialBold = contentManager.Load<SpriteFont>("ArialBold");

            float logoRatio = logo.Height / (float) logo.Width;
            
            // Todo: Make custom MainMenu GUI.
            this.mainMenu.elements.Add(new Image(new Rect(
                new ScreenSpaceUnit(.3f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(.1f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical),
                new ScreenSpaceUnit(.4f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(.4f * logoRatio, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal)
            ), logo, Color.White, 1f, ImageFitMode.Contain));

            this.mainMenu.elements.Add(new Button(new Rect(
                new ScreenSpaceUnit(.3f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(.4f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical),
                new ScreenSpaceUnit(.4f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(.1f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical)
            ),
            (position) => { gameRef.LoadScreen(new GameScreen(gameRef)); }, null, "Start Game", TextAlignment.Center, arialBold));

            this.mainMenu.elements.Add(new Button(new Rect(
                new ScreenSpaceUnit(.3f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(.55f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical),
                new ScreenSpaceUnit(.4f, ScreenSpaceUnit.ScreenSpaceOrientation.Horizontal),
                new ScreenSpaceUnit(.1f, ScreenSpaceUnit.ScreenSpaceOrientation.Vertical)
            ),
            (position) => { gameRef.Exit(); }, null, "Quit Game", TextAlignment.Center, arialBold));
            return;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.mainMenu.Draw(spriteBatch);
            return;
        }

        public void Update(float deltaTime)
        {
            if (Input.Mouse.GetMouseUp(MouseButtons.Left)) {
                this.mainMenu.ResolveMouseClick(Input.Mouse.GetPosition(), MouseButtons.Left);
            }
            return;
        }

        public void Dispose() {
            Debug.WriteLine("Disposing MainMenuScreen...");
        }

        public SamplerState GetSamplerState()
        {
            return SamplerState.PointClamp;
        }
    }
}