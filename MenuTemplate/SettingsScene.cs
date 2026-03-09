using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MenuTemplate
{
    public class SettingsScene
    {
        private Texture2D _pattern, _border, _btnNormal, _btnHover, _depthTex;
        private SpriteFont _font;
        private GraphicsDevice _graphicsDevice;

        private Vector2 _patternOffset;
        private float _borderXOffset;
        private int _selectedIndex = -1;

        private string[] _actionNames = { "ВВЕРХ", "ВНИЗ", "ВЛЕВО", "ВПРАВО", "ПРЫЖОК", "БЕГ", "что там ещё для\n       игр надо?"};
        private bool _isWaitingForKey = false;
        private int _bindingIndex = -1;

        private Vector2[] _resolutions = { new Vector2(1280, 720), new Vector2(1920, 1080) };
        private int _resIndex = 0;

        private GamePadState _lastGs;
        private MouseState _lastMs;
        private KeyboardState _lastKs;


        public SettingsScene(GraphicsDevice gd, Texture2D pixel) { _graphicsDevice = gd; }

        public void Reset()
        {
            _selectedIndex = -1;
            _isWaitingForKey = false;
        }

        public void LoadContent(ContentManager content, SpriteFont font)
        {
            _font = font;
            _pattern = content.Load<Texture2D>("pattern");
            _border = content.Load<Texture2D>("pattern2");
            _btnNormal = content.Load<Texture2D>("button1");
            _btnHover = content.Load<Texture2D>("button2");
            _depthTex = content.Load<Texture2D>("depth");
        }

        public void Update(GameTime gameTime, out bool backToMenu, out Vector2? newRes)
        {
            backToMenu = false;
            newRes = null;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _patternOffset += new Vector2(20, -20) * dt;
            _borderXOffset += 25f * dt;

            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            GamePadState gs = GamePad.GetState(PlayerIndex.One);

            if (_isWaitingForKey)
            {
                Keys[] pressed = ks.GetPressedKeys();
                if (pressed.Length > 0 && _lastKs.GetPressedKeys().Length == 0)
                {
                    if (_bindingIndex < SaveSystem.Keys.Length)
                        SaveSystem.Keys[_bindingIndex] = pressed[0];
                    _isWaitingForKey = false;
                }
                _lastKs = ks; return;
            }

            float mx = ms.X * (1280f / _graphicsDevice.Viewport.Width);
            float my = ms.Y * (720f / _graphicsDevice.Viewport.Height);
            Point mousePt = new Point((int)mx, (int)my);

            _selectedIndex = -1;
            for (int i = 0; i < 7; i++)
                if (new Rectangle(550, 40 + i * 60, 390, 50).Contains(mousePt)) _selectedIndex = i;
            if (new Rectangle(490, 480, 450, 55).Contains(mousePt)) _selectedIndex = 7;
            if (new Rectangle(400, 580, 200, 60).Contains(mousePt)) _selectedIndex = 8;
            if (new Rectangle(680, 580, 200, 60).Contains(mousePt)) _selectedIndex = 9;

            bool clicked = (ms.LeftButton == ButtonState.Pressed && _lastMs.LeftButton == ButtonState.Released) ||
                           (gs.Buttons.A == ButtonState.Pressed && _lastGs.Buttons.A == ButtonState.Released);

            if (clicked && _selectedIndex != -1)
            {
                if (_selectedIndex < 7) { _isWaitingForKey = true; _bindingIndex = _selectedIndex; }
                else if (_selectedIndex == 7) { _resIndex = (_resIndex + 1) % _resolutions.Length; }
                else if (_selectedIndex == 8) { newRes = _resolutions[_resIndex]; SaveSystem.Save((int)newRes.Value.X, (int)newRes.Value.Y); backToMenu = true; }
                else if (_selectedIndex == 9) { backToMenu = true; }
            }
            _lastGs = gs; _lastMs = ms; _lastKs = ks;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float sX = (float)_graphicsDevice.Viewport.Width / 1280f;
            float sY = (float)_graphicsDevice.Viewport.Height / 720f;
            Matrix matrix = Matrix.CreateScale(sX, sY, 1.0f);

            spriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(_pattern, new Rectangle(0, 0, 1280, 720), new Rectangle((int)-_patternOffset.X, (int)-_patternOffset.Y, 1280, 720), Color.White);
            spriteBatch.Draw(_border, new Rectangle(0, 0, 1280, 64), new Rectangle((int)_borderXOffset, 0, 1280, 64), Color.White);
            spriteBatch.Draw(_border, new Rectangle(0, 720 - 64, 1280, 64), new Rectangle((int)-_borderXOffset, 0, 1280, 64), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);
            for (int i = 0; i < 7; i++)
            {
                int y = 40 + i * 60;
                Rectangle dRect = new Rectangle(340, y, 200, 50);
                spriteBatch.Draw(_depthTex, dRect, Color.White);
                DrawText(spriteBatch, _actionNames[i], dRect, Color.White);

                Rectangle bRect = new Rectangle(550, y, 390, 50);
                DrawNineSlice(spriteBatch, _selectedIndex == i ? _btnHover : _btnNormal, bRect);

                string keyTxt = "NONE";
                if (i < SaveSystem.Keys.Length)
                    keyTxt = (_isWaitingForKey && _bindingIndex == i) ? "???" : SaveSystem.Keys[i].ToString();

                DrawText(spriteBatch, keyTxt, bRect, Color.Black);
            }

            Rectangle resR = new Rectangle(490, 480, 450, 55);
            DrawNineSlice(spriteBatch, _selectedIndex == 7 ? _btnHover : _btnNormal, resR);
            DrawText(spriteBatch, $"РАЗРЕШЕНИЕ: {(int)_resolutions[_resIndex].X}x{(int)_resolutions[_resIndex].Y}", resR, Color.Black);

            Rectangle accR = new Rectangle(400, 580, 200, 60);
            DrawNineSlice(spriteBatch, _selectedIndex == 8 ? _btnHover : _btnNormal, accR);
            DrawText(spriteBatch, "ПРИНЯТЬ", accR, Color.Black);

            Rectangle canR = new Rectangle(680, 580, 200, 60);
            DrawNineSlice(spriteBatch, _selectedIndex == 9 ? _btnHover : _btnNormal, canR);
            DrawText(spriteBatch, "ОТМЕНА", canR, Color.Black);
            spriteBatch.End();
        }

        private void DrawText(SpriteBatch sb, string txt, Rectangle r, Color c)
        {
            Vector2 sz = _font.MeasureString(txt);
            sb.DrawString(_font, txt, new Vector2(r.Center.X, r.Center.Y), c, 0f, sz / 2, 0.7f, SpriteEffects.None, 0f);
        }

        private void DrawNineSlice(SpriteBatch sb, Texture2D tex, Rectangle dest)
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