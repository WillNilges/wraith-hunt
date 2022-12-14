using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using WraithHunt;
using tainicom.Aether.Physics2D.Dynamics;
using Comora;
using System.Collections.Generic;
using System;

namespace DevcadeGame
{
    public enum GameState
    {
        PLAYING,
        MEDIUM_WON,
        WRAITH_WON,
        START,
        MENU,
        PAUSE
    }

    public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private float _spriteScale = 100f;

        public SpriteFont _HUDFont;
        public SpriteFont _titleFont;
        private Texture2D _redButton;

        private World world;

		private AEMedium medium;
		private AEWraith wraith;

		private List<AEDamageBox> damageBoxes;

		private AEDamageBox killPlane;

		private Map map;

        private Camera camera;
        private Camera wraithCamera;

        public GameState state = GameState.START;

        // Here be dragons
        // Stuff for viewport
        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

		/// <summary>
		/// Game constructor
		/// </summary>
		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = false;
		}

		/// <summary>
		/// Does any setup prior to the first frame that doesn't need loaded content.
		/// </summary>
		protected override void Initialize()
		{
			Input.Initialize(); // Sets up the input library

			// Set window size if running debug (in release it will be fullscreen)
			#region
#if DEBUG
			_graphics.PreferredBackBufferWidth = 420;
			_graphics.PreferredBackBufferHeight = 980;
			_graphics.ApplyChanges();
#else
			_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
			_graphics.ApplyChanges();
#endif
			#endregion

			// TODO: Add your initialization logic here

			camera = new Camera(_graphics.GraphicsDevice);
			wraithCamera = new Camera(_graphics.GraphicsDevice);
			camera.Zoom = 0.1f;
			wraithCamera.Zoom = 0.1f;

			world = new World();
			world.Gravity = new Vector2(0, 40f);

			map = new Map("Content/apartment_block.tmx", "chom_map_2", _spriteScale);

			medium = new AEMedium(
				"medium_placeholder",
				_spriteScale,
				new Vector2(1.5f, 1.5f),
				world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(10f, 150f), 0, BodyType.Dynamic),
				AETag.WRAITH
			);

            wraith = new AEWraith(
				"wraith_placeholder",
				_spriteScale,
				new Vector2(1.5f, 1.5f),
				world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(12f, 150f), 0, BodyType.Dynamic),
                AETag.MEDIUM
            );

			damageBoxes = new List<AEDamageBox>();

            Vector2 killPlaneSize = new Vector2(1000f, 10f);

            killPlane = new AEDamageBox(
                null,
                _spriteScale,
                killPlaneSize,
                world.CreateRectangle(
                    killPlaneSize.X,
                    killPlaneSize.Y,
                    1,
                    new Vector2(
                        -100f, 200f
                    ),
                    0,
                    BodyType.Static
                ),
                new DamageFrom(null, 100, new Vector2(15, -15)),
                new TimeSpan(1, 0, 0, 0, 0),
                false,
                Color.DarkGray,
                new Vector2(0,0)
            );

            base.Initialize();
		}

		/// <summary>
		/// Does any setup prior to the first frame that needs loaded content.
		/// </summary>
		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // ex.
            // texture = Content.Load<Texture2D>("fileNameWithoutExtention");
            
            // Split Screen
            defaultViewport = GraphicsDevice.Viewport;
            leftViewport = defaultViewport;
            rightViewport = defaultViewport;
            leftViewport.Height = leftViewport.Height / 2;
            rightViewport.Height = rightViewport.Height / 2;
            rightViewport.Y = leftViewport.Height;

            // Projection Matrix
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,1.0f, 4.0f / 3.0f, 10000f);
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 1.0f, 2.0f / 3.0f, 10000f);

            _HUDFont = Content.Load<SpriteFont>("hudfont");
            _titleFont = Content.Load<SpriteFont>("titlefont");
            _redButton = Content.Load<Texture2D>("red_button");

            medium.LoadContent(Content);
            wraith.LoadContent(Content);

			map.LoadContent(Content, world);
        }

		/// <summary>
		/// Your main update loop. This runs once every frame, over and over.
		/// </summary>
		/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
		protected override void Update(GameTime gameTime)
		{
			Input.Update(); // Updates the state of the input library

			// Exit when both menu buttons are pressed (or escape for keyboard debuging)
			// You can change this but it is suggested to keep the keybind of both menu
			// buttons at once for gracefull exit.
			if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
				(Input.GetButton(1, Input.ArcadeButtons.Menu) &&
				Input.GetButton(2, Input.ArcadeButtons.Menu)))
			{
				Exit();
			}

            // TODO: Add your update logic here
            KeyboardState myState = Keyboard.GetState();
            switch (state)
            {
                case GameState.START:
                    medium.Reset();
                    wraith.Reset();
                    state = GameState.PLAYING;
                    break;
                case GameState.MEDIUM_WON:
                case GameState.WRAITH_WON:
                    if (myState.IsKeyDown(Keys.Enter) || Input.GetButtonDown(1, Input.ArcadeButtons.A1))
                    {
                        state = GameState.START;
                    }
                    break;
                case GameState.PLAYING:
                    world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (wraith.health <= 0)
                    {
                        state = GameState.MEDIUM_WON;
                        break;
                    }
                    if (medium.health <= 0)
                    {
                        state = GameState.WRAITH_WON;
                        break;
                    }
                    map.Update(gameTime);

                    medium.Update(gameTime);
                    wraith.Update(gameTime);

                    foreach (AEDamageBox box in damageBoxes)
                    {
                        box.Update(gameTime, world);
                    }

                    // Delete damage boxes whose timers have expired
                    damageBoxes.RemoveAll(x => x.tick < TimeSpan.Zero);

                    camera.Position =
                        new Vector2(medium.Position().X * _spriteScale, medium.Position().Y * _spriteScale + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 2);
                    camera.Update(gameTime);

                    wraithCamera.Position =
                        new Vector2(wraith.Position().X * _spriteScale, wraith.Position().Y * _spriteScale + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 2);
                    wraithCamera.Update(gameTime);

                    /** Player 1 **/
                    if (myState.IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.A1))
                    {
                        medium.Jump();
                    }

                    if (myState.IsKeyDown(Keys.A) || Input.GetButtonHeld(1, Input.ArcadeButtons.StickLeft))
                    {
                        medium.Walk(Direction.LEFT);
                    }

                    if (myState.IsKeyDown(Keys.D) || Input.GetButtonHeld(1, Input.ArcadeButtons.StickRight))
                    {
                        medium.Walk(Direction.RIGHT);
                    }

                    if (myState.IsKeyDown(Keys.E) || Input.GetButtonHeld(1, Input.ArcadeButtons.A2))
                    {
                        AEDamageBox box = medium.Attack(world);
                        if (box != null)
                            damageBoxes.Add(box);
                    }

                    /** Player 2 **/
                    if (myState.IsKeyDown(Keys.I) || Input.GetButtonDown(2, Input.ArcadeButtons.A1))
                    {
                        wraith.Jump();
                    }

                    if (myState.IsKeyDown(Keys.J) || Input.GetButtonHeld(2, Input.ArcadeButtons.StickLeft))
                    {
                        wraith.Walk(Direction.LEFT);
                    }

                    if (myState.IsKeyDown(Keys.L) || Input.GetButtonHeld(2, Input.ArcadeButtons.StickRight))
                    {
                        wraith.Walk(Direction.RIGHT);
                    }

                    if (myState.IsKeyDown(Keys.O) || Input.GetButtonHeld(2, Input.ArcadeButtons.A2))
                    {
                        AEDamageBox box = wraith.Attack(world);
                        if (box != null)
                            damageBoxes.Add(box);
                    }
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
		}

        /// <summary>
        /// Your main draw loop. This runs once every frame, over and over.
        /// </summary>
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            void drawThings()
            {
                // TODO: Add your drawing code here
                map.Draw(gameTime, _spriteBatch);
                medium.Draw(gameTime, _spriteBatch);
                wraith.Draw(gameTime, _spriteBatch);

                foreach (AEDamageBox box in damageBoxes)
                {
                    box.DrawBox(gameTime, _spriteBatch);
                }
                //killPlane.DrawBox(gameTime, _spriteBatch); // Don't draw the killplane; We don't need it.
            }
            switch (state)
            {
                case GameState.PLAYING:
                    GraphicsDevice.Viewport = leftViewport;
                    _spriteBatch.Begin(camera);
                    drawThings();
                    _spriteBatch.End();

                    GraphicsDevice.Viewport = rightViewport;
                    _spriteBatch.Begin(wraithCamera);
                    drawThings();
                    _spriteBatch.End();

                    // Draw HUDs and other important stuff
                    GraphicsDevice.Viewport = defaultViewport;
                    _spriteBatch.Begin();
                    medium.DebugDraw(gameTime, _spriteBatch, _HUDFont);
                    // Draw black dividing bar
                    RectangleSprite.FillRectangle(
                        _spriteBatch,
                        new Rectangle(
                            0,
                            defaultViewport.Height / 2,
                            defaultViewport.Width,
                            5
                        ),
                        Color.Black
                    );
                    medium.drawHUD(_spriteBatch, defaultViewport, _HUDFont, false);
                    wraith.drawHUD(_spriteBatch, defaultViewport, _HUDFont, true);
                    _spriteBatch.End();
                    break;
                case GameState.WRAITH_WON:
                case GameState.MEDIUM_WON:
                    _spriteBatch.Begin();
                    WHScreens.drawWinner(state, GraphicsDevice, defaultViewport, _spriteBatch, _HUDFont, _titleFont, _redButton);
                    _spriteBatch.End();
                    break;
                default:
                    break;
            }

			base.Draw(gameTime);
		}
	}
}
