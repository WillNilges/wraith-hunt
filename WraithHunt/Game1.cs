using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

// For camera functionality
using Comora;

namespace WraithHunt
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

        private List<WorldObject> _platforms;

        private Player medium;
        private Player demon;

        private KeyboardState _lastState;

        private Camera camera;

        // Here be dragons
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
            this.camera = new Camera(this._graphics.GraphicsDevice);

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
            leftViewport.Width = leftViewport.Width / 2;
            rightViewport.Width = rightViewport.Width / 2;
            rightViewport.X = leftViewport.Width;

            // Projection Matrix
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 4.0f / 3.0f, 1.0f, 10000f);
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);

            // Platforms
            _platforms = new List<WorldObject>();
            for (int i = 0; i < 10; i++)
            {
                WorldObject chom = new WorldObject(100,200 + i*100,100,10,Color.Brown);
                WorldObject skz = new WorldObject(300,100 + i*100,100,10,Color.Brown);
                _platforms.Add(chom);
                _platforms.Add(skz);
            }
            
            // Players
            medium = new Player(120, 350, 10, 10, Color.White);

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
            this.camera.Update(gameTime);
            Vector2 mediumPos = new Vector2(medium.space.X, medium.space.Y);
            this.camera.Position = mediumPos;
            
            KeyboardState myState = Keyboard.GetState();
            if (_lastState == null)
                _lastState = Keyboard.GetState();

            if (myState.IsKeyDown(Keys.W))
            {
                medium.Jump();
            }

            if (myState.IsKeyDown(Keys.A))
            {
                medium.Walk(Direction.LEFT);
            }

            if (myState.IsKeyDown(Keys.D))
            {
                medium.Walk(Direction.RIGHT);
            }

            medium.UpdatePhysics(_platforms);

            _lastState = Keyboard.GetState();
			base.Update(gameTime);
		}

		/// <summary>
		/// Your main draw loop. This runs once every frame, over and over.
		/// </summary>
		/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
		protected override void Draw(GameTime gameTime)
		{
            GraphicsDevice.Viewport = defaultViewport;
			GraphicsDevice.Clear(Color.Navy);


            GraphicsDevice.Viewport = leftViewport;
			_spriteBatch.Begin(this.camera);
			// TODO: Add your drawing code here
            foreach(WorldObject obj in _platforms)
            {
                obj.DrawBox(_spriteBatch);
            }
            medium.DrawBox(_spriteBatch);
			_spriteBatch.End();

            GraphicsDevice.Viewport = rightViewport;
			_spriteBatch.Begin(this.camera);
			// TODO: Add your drawing code here
            foreach(WorldObject obj in _platforms)
            {
                obj.DrawBox(_spriteBatch);
            }
            medium.DrawBox(_spriteBatch);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
