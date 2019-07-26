using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.IMEHelper;

namespace SkiaSharp_MonoGame_Example
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        IMEHandler imeHandler;
        string inputContent;

        TextView textViewTitle;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;

            Content.RootDirectory = "Content";
        }

        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = 1024;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = 800;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            imeHandler = IMEHandler.Create(this, true);
            imeHandler.TextInput += ImeHandler_TextInput;

            IsMouseVisible = true;

            textViewTitle = new TextView("Press F1 to toggle IME.", new Point(250, 80), 20) { TextColor = Color.Orange };

            base.Initialize();
        }

        private void ImeHandler_TextInput(object sender, MonoGame.IMEHelper.TextInputEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Back:
                    if (inputContent.Length > 0)
                        inputContent = inputContent.Remove(inputContent.Length - 1, 1);
                    break;
                case Keys.Enter:
                case Keys.Escape:
                    inputContent = "";
                    break;
                default:
                    inputContent += e.Character;
                    break;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if (imeHandler.Enabled)
                    imeHandler.StopTextComposition();
                else
                    imeHandler.StartTextComposition();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            textViewTitle.Draw(spriteBatch, new Point(10,10), Color.White);

            base.Draw(gameTime);
        }
    }
}
