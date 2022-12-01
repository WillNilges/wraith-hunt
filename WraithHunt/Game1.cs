using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

using TiledSharp;

// For camera functionality
using Comora;

// For fiziks
using tainicom.Aether.Physics2D.Dynamics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Data.Common;

namespace WraithHunt
{

    public enum GameState
    {
        PLAYING,
        MEDIUM_WON,
        DEMON_WON,
        START,
        MENU,
        PAUSE
    }

	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

        private World world; // Our world, which will keep track of all physics

        // These guys are on THIN fuckin' ice.
        private List<DamageBox> _dmgBoxes; // For stuff that hurts
        private DamageBox killPlane; // For the thing that kills you


        // Sprites n Textures
        private Texture2D _redButton; // Appears on the game over screen

        // Player 1 and Player 2
        private AEPlayer medium;
        private Demon demon;

        private KeyboardState _lastState;

        private Camera camera;
        private Camera demonCamera;

        // Stuff for viewport
        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

        // Fonts
        private SpriteFont _HUDFont;
        private SpriteFont _titleFont;

        // Game State
        public GameState state = GameState.START;

        // New Map Stuff
        TmxMap map;
        Texture2D tileset;
        private List<Body> mapBodies;

        int tileWidth;
        int tileHeight;
        int tilesetTilesWide;
        int tilesetTilesHigh;

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
            world = new World();
            world.Gravity = new Vector2(0, 100f);

            camera = new Camera(_graphics.GraphicsDevice);
            demonCamera = new Camera(_graphics.GraphicsDevice);

            camera.Zoom = 2;

            medium = new AEPlayer(
               "medium_placeholder_01_white",
               new Vector2(24, 24),
               world.CreateRectangle(15f, 15f, 1, new Vector2(1500, 1000), 0, BodyType.Dynamic)
            );

             mapBodies = new List<Body>();

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

            // New Map Stuff
            map = new TmxMap("Content/apartment_block.tmx");
            tileset = Content.Load<Texture2D>("chom_map_2");//map.Tilesets[0].Name.ToString());

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;

            mapHitboxes();

            // Attack System
            _dmgBoxes = new List<DamageBox>(); 

            // Kill Plane
            killPlane = new DamageBox(-1000, 3000, Direction.RIGHT, 10000, 10000, Color.Red, -1, 1000000, false, null);
            _dmgBoxes.Add(killPlane);

            // Players
            medium.LoadContent(Content);

            // Font for HUD
            _HUDFont = Content.Load<SpriteFont>("hudfont"); 
            _titleFont = Content.Load<SpriteFont>("titlefont"); 

            // For the continue screen...
            _redButton = Content.Load<Texture2D>("red_button");

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

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            this.camera.Position = medium.Position();
            this.camera.Update(gameTime);

            // Controls
            if (_lastState == null)
                _lastState = Keyboard.GetState();
            KeyboardState myState = Keyboard.GetState();

            // TODO: Re-add game mode stuff

            // Medium Keys
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

            _lastState = Keyboard.GetState();
			base.Update(gameTime);
		}
        private void mapHitboxes()
        {
            for (var layer = 1; layer < map.Layers.Count; layer++)
            {
                for (var i = 0; i < map.Layers[layer].Tiles.Count; i++)
                {
                    int gid = map.Layers[layer].Tiles[i].Gid;

                    // Empty tile, do nothing
                    if (gid == 0)
                    {

                    }
                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                        // Eeach tile is going to be 2x2 metres

                        float x = (i % map.Width) * tileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * tileHeight;

                        var mapBody = world.CreateRectangle(
                            tileHeight,
                            tileWidth,
                            100,
                            new Vector2(x, y),
                            0,
                            BodyType.Static
                        );

                        mapBody.SetFriction(100.75f);
                        mapBody.SetRestitution(0);

                        mapBodies.Add(mapBody);
                    }
                }
            }
        }

        private void drawNewMap()
        {
            for (var layer = 0; layer < map.Layers.Count; layer++)
            {
                for (var i = 0; i < map.Layers[layer].Tiles.Count; i++)
                {
                    int gid = map.Layers[layer].Tiles[i].Gid;

                    // Empty tile, do nothing
                    if (gid == 0)
                    {

                    }
                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                        _spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                    }
                }
            }
        }

		/// <summary>
		/// Your main draw loop. This runs once every frame, over and over.
		/// </summary>
		/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
		protected override void Draw(GameTime gameTime)
		{
            // Draw the background and the separation bar.
            GraphicsDevice.Viewport = defaultViewport;
			GraphicsDevice.Clear(Color.DarkSlateGray);

            GraphicsDevice.Viewport = leftViewport;
            _spriteBatch.Begin(camera);

            // TODO: Add your drawing code here
            // Draw the world map background
            drawNewMap();
            medium.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            GraphicsDevice.Viewport = defaultViewport;
            _spriteBatch.Begin();
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
            _spriteBatch.End();

            GraphicsDevice.Viewport = rightViewport;

            base.Draw(gameTime);
		}
	}
}
