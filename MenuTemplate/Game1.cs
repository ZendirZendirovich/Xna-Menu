using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media; // Для музыки

namespace MenuTemplate
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private MenuScene _menuScene;
        private SettingsScene _settingsScene;

        private Texture2D _pixel;
        private SpriteFont _font;
        private Song _bgMusic;

        private enum GameState { Menu, Settings, Game }
        private GameState _currentState = GameState.Menu;

        private bool _waitForInputRelease = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var res = SaveSystem.Load();
            _graphics.PreferredBackBufferWidth = res.w;
            _graphics.PreferredBackBufferHeight = res.h;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _font = Content.Load<SpriteFont>("font"); // Убедись, что шрифт называется "font"

            _menuScene = new MenuScene(GraphicsDevice, _pixel);
            _menuScene.LoadContent(Content, _font);

            _settingsScene = new SettingsScene(GraphicsDevice, _pixel);
            _settingsScene.LoadContent(Content, _font);

            _bgMusic = Content.Load<Song>("MainMenu");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(_bgMusic);
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            GamePadState gs = GamePad.GetState(PlayerIndex.One);

            if (_waitForInputRelease)
            {
                if (ms.LeftButton == ButtonState.Released && gs.Buttons.A == ButtonState.Released)
                {
                    _waitForInputRelease = false;
                }
                else
                {
                    base.Update(gameTime);
                    return;
                }
            }

            switch (_currentState)
            {
                case GameState.Menu:
                    bool startGame, exitGame, openSettings;
                    _menuScene.Update(gameTime, out startGame, out exitGame, out openSettings);

                    if (exitGame)
                        Exit();

                    if (startGame)
                    {
                        _currentState = GameState.Game;
                        _waitForInputRelease = true;
                    }

                    if (openSettings)
                    {
                        _currentState = GameState.Settings;
                        _settingsScene.Reset();
                        _waitForInputRelease = true;
                    }
                    break;

                case GameState.Settings:
                    bool backToMenu;
                    Vector2? newRes;
                    _settingsScene.Update(gameTime, out backToMenu, out newRes);

                    if (newRes.HasValue)
                    {
                        _graphics.PreferredBackBufferWidth = (int)newRes.Value.X;
                        _graphics.PreferredBackBufferHeight = (int)newRes.Value.Y;
                        _graphics.ApplyChanges();
                    }

                    if (backToMenu)
                    {
                        _currentState = GameState.Menu;
                        _waitForInputRelease = true;
                    }
                    break;

                case GameState.Game:

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        _currentState = GameState.Menu;
                        _waitForInputRelease = true;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_currentState == GameState.Menu)
            {
                _menuScene.Draw(_spriteBatch);
            }
            else if (_currentState == GameState.Settings)
            {
                _settingsScene.Draw(_spriteBatch);
            }
            else if (_currentState == GameState.Game)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, "Всё, дальше делаем игру сами,", new Vector2(400, 320), Color.White);
                _spriteBatch.DrawString(_font, "\n если уж будете использовать, пожалуйста отмечайте как автора меню.", new Vector2(100, 320), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}