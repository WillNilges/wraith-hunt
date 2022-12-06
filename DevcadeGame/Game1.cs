using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using WraithHunt;
using tainicom.Aether.Physics2D.Dynamics;
using Comora;
using System.Collections.Generic;

namespace DevcadeGame
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private float _spriteScale = 100f;

		private World world;

		private AEPlayer medium;
		private List<AEObject> platforms;

		private Map map;

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
			camera.Zoom = 0.01f;

			world = new World();
			world.Gravity = new Vector2(0, 40f);

			map = new Map("Content/apartment_block.tmx", "chom_map_2", _spriteScale);

			medium = new AEPlayer(
			   "medium_placeholder",
			   _spriteScale,
			   new Vector2(1.5f, 1.5f),
			   world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(0f, -50f), 0, BodyType.Dynamic)
			);

			platforms = new List<AEObject>();

            platforms.Add(new AEObject(
               "ground_placeholder",
               _spriteScale,
               new Vector2(5f, 5f),
               world.CreateRectangle(5f, 5f, 1, new Vector2(0f, -30f), 0, BodyType.Static)
            ));

            platforms.Add(new AEObject(
               "ground_placeholder",
               _spriteScale,
               new Vector2(5f, 5f),
               world.CreateRectangle(5f, 5f, 1, new Vector2(5f, -25f), 0, BodyType.Static)
            ));

            platforms.Add(new AEObject(
               "ground_placeholder",
               _spriteScale,
               new Vector2(20f, 5f),
               world.CreateRectangle(20f, 5f, 1, new Vector2(20f, -25f), 0, BodyType.Static)
            ));


            platforms.Add(new AEObject(
               "ground_placeholder",
               _spriteScale,
               new Vector2(200f, 5f),
               world.CreateRectangle(200f, 5f, 1, new Vector2(-10f, -0f), 0, BodyType.Static)
            ));

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

			foreach (AEObject AEObj in platforms)
				AEObj.LoadContent(Content);

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

			map.Update(gameTime);

			medium.Update(gameTime);
			foreach(AEObject AEObj in platforms) AEObj.Update(gameTime);

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

			camera.Position = new Vector2(medium.Position().X * _spriteScale, medium.Position().Y * _spriteScale);
			camera.Update(gameTime);

            KeyboardState myState = Keyboard.GetState();
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
			foreach (AEObject AEObj in platforms)
			{
                AEObj.Draw(gameTime, _spriteBatch);
            }
            medium.Draw(gameTime, _spriteBatch);

			map.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
