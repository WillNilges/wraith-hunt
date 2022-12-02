using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using WraithHunt;
using tainicom.Aether.Physics2D.Dynamics;
using Comora;

namespace DevcadeGame
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private World world;

		private AEPlayer medium;
		private AEPlayer platform;

		private Camera camera;

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
			camera.Zoom = 0.5f;

			world = new World();
			world.Gravity = new Vector2(0, 20f);

			medium = new AEPlayer(
			   "medium_placeholder",
			   100.0f,
				new Vector2(1.5f, 1.5f),
			   world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(0f, -50f), 0, BodyType.Dynamic)
			);

            platform = new AEPlayer(
               "ground_placeholder",
               100.0f,
                new Vector2(20f, 1f),
               world.CreateRectangle(20f, 1f, 1, new Vector2(0f, -30f), 0, BodyType.Static)
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

            medium.LoadContent(Content);
            platform.LoadContent(Content);
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

			medium.Update(gameTime);
			platform.Update(gameTime);

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

			camera.Position = new Vector2(medium.Position().X * 100f, medium.Position().Y * 100f);
			camera.Update(gameTime);

            KeyboardState myState = Keyboard.GetState();
            if (myState.IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.A1))
            {
                medium.Jump();
            }

            base.Update(gameTime);
		}

		/// <summary>
		/// Your main draw loop. This runs once every frame, over and over.
		/// </summary>
		/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			//_spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(0, 0, 10)));
            _spriteBatch.Begin(camera);
			//_spriteBatch.Begin(transformMatrix: Matrix.CreateScale(10));
            // TODO: Add your drawing code here
            platform.Draw(gameTime, _spriteBatch);
            medium.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}