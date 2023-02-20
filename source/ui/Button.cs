using System;
using Celesteia.Resources;
using Celesteia.Resources.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Celesteia.UI {
    public class Button : IClickableElement {

        // Interface implementations.
        public Rect GetRect() => _rect;
        public void SetRect(Rect rect) => _rect = rect;
        public void OnMouseIn() => _mouseOver = true;
        public void OnMouseOut() => _mouseOver = false;
        private IContainer _parent;
        public IContainer GetParent() => _parent;
        public void SetParent(IContainer parent) => _parent = parent;
        private Vector2 _pivot;
        public Vector2 GetPivot() => _pivot;
        public void SetPivot(Vector2 pivot) => _pivot = pivot;

        public Rectangle GetRectangle() {
            Rectangle r = GetRect().ResolveRectangle();

            if (GetParent() != null) {
                r.X += GetParent().GetRectangle().X;
                r.Y += GetParent().GetRectangle().Y;
            }

            r.X -= (int)Math.Round(_pivot.X * r.Width);
            r.Y -= (int)Math.Round(_pivot.Y * r.Height);
            
            return r;
        }
        
        private Rect _rect = Rect.AbsoluteZero;

        private ButtonColorGroup _colorGroup = new ButtonColorGroup(Color.White);
        private Color ButtonColor = Color.White;

        public delegate void ClickEvent(Point position);
        private ClickEvent _onClick = null;

        private bool _isEnabled = true;

        private bool _mouseOver;

        private Texture2D _texture;

        private string _text = "";
        private TextAlignment _textAlignment = TextAlignment.Left;
        private FontType _font;
        private float _fontSize;

        private TextureAtlas _patches;
        private int _patchSize;

        public Button(Rect rect) {
            _rect = rect;
        }

        public Button SetPivotPoint(Vector2 pivot) {
            _pivot = pivot;
            return this;
        }

        public Button SetOnClick(ClickEvent onClick) {
            _onClick = onClick;
            return this;
        }

        public Button SetTexture(Texture2D texture) {
            _texture = texture;
            return this;
        }

        public Button MakePatches(int size) {
            if (_texture != null) {
                _patchSize = size;
                _patches = TextureAtlas.Create("buttonPatches", _texture, _patchSize, _patchSize);
            }
            return this;
        }

        public Button SetFont(FontType font) {
            _font = font;
            return this;
        }

        public Button SetText(string text) {
            _text = text;
            return this;
        }
        
        public Button SetFontSize(float fontSize) {
            _fontSize = fontSize;
            return this;
        }

        public Button SetTextAlignment(TextAlignment textAlignment) {
            _textAlignment = textAlignment;
            return this;
        }

        public Button SetColorGroup(ButtonColorGroup ButtonColorGroup) {
            _colorGroup = ButtonColorGroup;
            return this;
        }

        public void OnClick(Point position) {
            _onClick?.Invoke(position);
        }

        // https://gamedev.stackexchange.com/a/118255
        private float _colorAmount = 0.0f;
        private bool _prevMouseOver = false;
        public void Update(GameTime gameTime) {
            if (_prevMouseOver != _mouseOver) _colorAmount = 0.0f;

            _colorAmount += (float)gameTime.ElapsedGameTime.TotalSeconds / 0.5f;

            if (_colorAmount > 1.0f)
                _colorAmount = 0.0f;

            ButtonColor = Color.Lerp(ButtonColor, GetTargetColor(), _colorAmount);

            _prevMouseOver = _mouseOver;
        }

        Rectangle r;
        public void Draw(SpriteBatch spriteBatch)
        {
            r = GetRectangle();

            // Draw the button's texture.
            if (_patches != null) DrawPatched(spriteBatch, r);
            else spriteBatch.Draw(GetTexture(spriteBatch), r, null, ButtonColor);

            TextUtilities.DrawAlignedText(spriteBatch, _font, _text, _textAlignment, r, 24f);
        }

        private int _scaledPatchSize => _patchSize * UIReferences.Scaling;
        private void DrawPatched(SpriteBatch spriteBatch, Rectangle r) {
            int y;

            // Top
            y = r.Y;
            {
                // Top left
                spriteBatch.Draw(_patches.GetRegion(0), new Rectangle(r.X, y, _scaledPatchSize, _scaledPatchSize), ButtonColor);

                // Top center
                spriteBatch.Draw(_patches.GetRegion(1), new Rectangle(r.X + _scaledPatchSize, y, r.Width - (2 * _scaledPatchSize), _scaledPatchSize), ButtonColor);

                // Top right
                spriteBatch.Draw(_patches.GetRegion(2), new Rectangle(r.X + r.Width - _scaledPatchSize, y, _scaledPatchSize, _scaledPatchSize), ButtonColor);
            }

            // Center
            y = r.Y + _scaledPatchSize;
            {
                // Left center
                spriteBatch.Draw(_patches.GetRegion(3), new Rectangle(r.X, y, _scaledPatchSize, r.Height - (2 * _scaledPatchSize)), ButtonColor);

                // Center
                spriteBatch.Draw(_patches.GetRegion(4), new Rectangle(r.X + _scaledPatchSize, y, r.Width - (2 * _scaledPatchSize), r.Height - (2 * _scaledPatchSize)), ButtonColor);

                // Right center
                spriteBatch.Draw(_patches.GetRegion(5), new Rectangle(r.X + r.Width - _scaledPatchSize, y, _scaledPatchSize, r.Height - (2 * _scaledPatchSize)), ButtonColor);
            }

            // Bottom
            y = r.Y + r.Height - _scaledPatchSize;
            {
                // Bottom left
                spriteBatch.Draw(_patches.GetRegion(6), new Rectangle(r.X, y, _scaledPatchSize, _scaledPatchSize), ButtonColor);

                // Bottom center
                spriteBatch.Draw(_patches.GetRegion(7), new Rectangle(r.X + _scaledPatchSize, y, r.Width - (2 * _scaledPatchSize), _scaledPatchSize), ButtonColor);

                // Bottom right
                spriteBatch.Draw(_patches.GetRegion(8), new Rectangle(r.X + r.Width - _scaledPatchSize, y, _scaledPatchSize, _scaledPatchSize), ButtonColor);
            }
        }

        public Texture2D GetTexture(SpriteBatch spriteBatch) {
            if (_texture == null) {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _texture.SetData(new[] { Color.Gray });
            }

            return _texture;
        }

        public Color GetTargetColor() {
            return _isEnabled ? (_mouseOver ? _colorGroup.Hover : _colorGroup.Normal) : _colorGroup.Disabled;
        }
    }

    public struct ButtonColorGroup {
        public Color Normal;
        public Color Disabled;
        public Color Hover;
        public Color Active;

        public ButtonColorGroup(Color normal, Color disabled, Color hover, Color active) {
            Normal = normal;
            Disabled = disabled;
            Hover = hover;
            Active = active;
        }

        public ButtonColorGroup(Color normal, Color disabled, Color hover) : this (normal, disabled, hover, normal) {}

        public ButtonColorGroup(Color normal, Color disabled) : this (normal, disabled, normal, normal) {}
        public ButtonColorGroup(Color normal) : this (normal, normal, normal, normal) {}        
    }
}