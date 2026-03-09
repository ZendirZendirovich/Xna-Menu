using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MenuTemplate
{
    public class MenuScene
    {
        private Texture2D _pattern, _border, _pixel, _btnNormal, _btnHover, _logoTexture;
        private SpriteFont _font;
        private GraphicsDevice _graphicsDevice;
        private Vector2 _patternOffset;
        private float _borderXOffset;

        private int _selectedIndex = -1;
        private string[] _buttonLabels = { "ИГРАТЬ", "НАСТРОЙКИ", "ВЫЙТИ" };

        private GamePadState _lastGs;
        private float _fadeAlpha = 0f;
        private bool _isStarting = false, _isExiting = false, _isSettings = false;
        private int _buttonWidth = 400, _buttonHeight = 60, _buttonSpacing = 20;

        private float _animationTimer;
        private Vector2 _basePosition = new Vector2(512, 84);

        string versionText = "*Версия игры*";

        public MenuScene(GraphicsDevice gd, Texture2D pixel)
        {
            _graphicsDevice = gd;
            _pixel = pixel;
        }

        public void LoadContent(ContentManager content, SpriteFont font)
        {
            _font = font;
            _pattern = content.Load<Texture2D>("pattern");
            _border = content.Load<Texture2D>("pattern2");
            _btnNormal = content.Load<Texture2D>("button1");
            _btnHover = content.Load<Texture2D>("button2");
            _logoTexture = content.Load<Texture2D>("logo");
        }

        public void Update(GameTime gameTime, out bool startGame, out bool exitGame, out bool openSettings)
        {
            startGame = false; exitGame = false; openSettings = false;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isStarting || _isExiting || _isSettings)
            {
                _fadeAlpha += dt * 2.5f;
                if (_fadeAlpha >= 1.0f)
                {
                    _fadeAlpha = 1.0f;
                    if (_isStarting) { startGame = true; _isStarting = false; }
                    if (_isSettings) { openSettings = true; _isSettings = false; }
                    if (_isExiting) exitGame = true;
                }
                return;
            }

            if (_fadeAlpha > 0) _fadeAlpha -= dt * 2.5f;

            _patternOffset += new Vector2(20, -20) * dt;
            _borderXOffset += 25f * dt;
            _animationTimer += dt;

            GamePadState gs = GamePad.GetState(PlayerIndex.One);
            MouseState ms = Mouse.GetState();

            float mx = ms.X * (1280f / _graphicsDevice.Viewport.Width);
            float my = ms.Y * (720f / _graphicsDevice.Viewport.Height);
            Point mousePt = new Point((int)mx, (int)my);

            _selectedIndex = -1;
            for (int i = 0; i < _buttonLabels.Length; i++)
            {
                Rectangle btnRect = new Rectangle(640 - _buttonWidth / 2, 390 + i * (_buttonHeight + _buttonSpacing), _buttonWidth, _buttonHeight);
                if (btnRect.Contains(mousePt))
                {
                    _selectedIndex = i;
                    bool clicked = (ms.LeftButton == ButtonState.Pressed);
                    bool padClicked = (gs.Buttons.A == ButtonState.Pressed && _lastGs.Buttons.A == ButtonState.Released);

                    if (clicked || padClicked)
                    {
                        if (i == 0) _isStarting = true;
                        if (i == 1) openSettings = true;
                        if (i == 2) exitGame = true;
                    }
                }

            }
            _lastGs = gs;
        }

        private void TriggerButton(int idx)
        {
            if (idx == 0) _isStarting = true;
            if (idx == 1) _isSettings = true;
            if (idx == 2) _isExiting = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scaleX = (float)_graphicsDevice.Viewport.Width / 1280f;
            float scaleY = (float)_graphicsDevice.Viewport.Height / 720f;
            Matrix matrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);

            spriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(_pattern, new Rectangle(0, 0, 1280, 720), new Rectangle((int)-_patternOffset.X, (int)-_patternOffset.Y, 1280, 720), Color.White);
            spriteBatch.Draw(_border, new Rectangle(0, 0, 1280, 64), new Rectangle((int)_borderXOffset, 0, 1280, 64), Color.White);
            spriteBatch.Draw(_border, new Rectangle(0, 720 - 64, 1280, 64), new Rectangle((int)-_borderXOffset, 0, 1280, 64), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);

            Vector2 versionSize = _font.MeasureString(versionText);
            Vector2 versionPosition = new Vector2(20, 720 - versionSize.Y - 20);

            spriteBatch.DrawString(_font, versionText, versionPosition, Color.White);

            float amplitude = 15f;
            float speed = 2f;
            float offset = (float)Math.Sin(_animationTimer * speed) * amplitude;
            float shadowOffset = (float)Math.Sin((_animationTimer - 0.2f) * speed) * amplitude;
            Vector2 logoOrigin = new Vector2(128, 85);

            spriteBatch.Draw(_logoTexture, _basePosition + new Vector2(0, shadowOffset), null, Color.Black * 0.5f, 0f, logoOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_logoTexture, _basePosition + new Vector2(0, offset), null, Color.White, 0f, logoOrigin, 1f, SpriteEffects.None, 0f);

            for (int i = 0; i < _buttonLabels.Length; i++)
            {
                Rectangle dest = new Rectangle(640 - _buttonWidth / 2, 390 + i * (_buttonHeight + _buttonSpacing), _buttonWidth, _buttonHeight);

                Texture2D currentTex = (_selectedIndex == i) ? _btnHover : _btnNormal;

                DrawNineSliceButton(spriteBatch, currentTex, dest);

                Vector2 sz = _font.MeasureString(_buttonLabels[i]);
                Vector2 textPos = new Vector2((int)(dest.Center.X - sz.X / 2), (int)(dest.Center.Y - sz.Y / 2));
                spriteBatch.DrawString(_font, _buttonLabels[i], textPos, Color.Black);
            }

            spriteBatch.End();
        }

        private void DrawNineSliceButton(SpriteBatch sb, Texture2D tex, Rectangle dest)
        {
            int p = 32;
            sb.Draw(tex, new Rectangle(dest.X + p, dest.Y + p, dest.Width - p * 2, dest.Height - p * 2), new Rectangle(p, p, tex.Width - p * 2, tex.Height - p * 2), Color.White);
            sb.Draw(tex, new Rectangle(dest.X + p, dest.Y, dest.Width - p * 2, p), new Rectangle(p, 0, tex.Width - p * 2, p), Color.White);
            sb.Draw(tex, new Rectangle(dest.X + p, dest.Bottom - p, dest.Width - p * 2, p), new Rectangle(p, tex.Height - p, tex.Width - p * 2, p), Color.White);
            sb.Draw(tex, new Rectangle(dest.X, dest.Y + p, p, dest.Height - p * 2), new Rectangle(0, p, p, tex.Height - p * 2), Color.White);
            sb.Draw(tex, new Rectangle(dest.Right - p, dest.Y + p, p, dest.Height - p * 2), new Rectangle(tex.Width - p, p, p, tex.Height - p * 2), Color.White);
            sb.Draw(tex, new Rectangle(dest.X, dest.Y, p, p), new Rectangle(0, 0, p, p), Color.White);
            sb.Draw(tex, new Rectangle(dest.Right - p, dest.Y, p, p), new Rectangle(tex.Width - p, 0, p, p), Color.White);
            sb.Draw(tex, new Rectangle(dest.X, dest.Bottom - p, p, p), new Rectangle(0, tex.Height - p, p, p), Color.White);
            sb.Draw(tex, new Rectangle(dest.Right - p, dest.Bottom - p, p, p), new Rectangle(tex.Width - p, tex.Height - p, p, p), Color.White);
        }
    }
}