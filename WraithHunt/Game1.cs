﻿using System.Collections.Generic;

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

        private List<WorldObject> _platforms; // For stuff you can stand on
        private List<DamageBox> _dmgBoxes; // For stuff that hurts

        private Medium medium;
        private Demon demon;

        private KeyboardState _lastState;

        private Camera camera;
        private Camera demonCamera;

        // Here be dragons
        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

        // Fonts
        private SpriteFont _HUDFont;

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
            this.demonCamera = new Camera(this._graphics.GraphicsDevice);

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

            // Platforms
            _platforms = new List<WorldObject>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j ++)
                {
                    WorldObject skz = new WorldObject(200 * j,100 + i*100,100,10,Color.DarkGray);
                    _platforms.Add(skz);
                }
            }

            // Attack System
            _dmgBoxes = new List<DamageBox>(); 
            
            // Players
            medium = new Medium(70, 350, 10, 10, Color.White);
            medium.sprite = Content.Load<Texture2D>("medium_placeholder_01_white");
            medium.spriteParams = new Rectangle(
                medium.space.X-15,
                medium.space.Y-15,
                30,
                30
            );
            medium.spriteOffsetLeft = 15;
            medium.spriteOffsetRight = 0;

            demon = new Demon(600, 350, 10, 10, Color.Red);
            demon.sprite = Content.Load<Texture2D>("demon_placeholder_01");
            demon.spriteParams = new Rectangle(
                demon.space.X-15,
                demon.space.Y-15,
                30,
                30
            );
            demon.spriteOffsetLeft = 10;
            demon.spriteOffsetRight = 10;

            // Font for HUD
            _HUDFont = Content.Load<SpriteFont>("hudfont"); 

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

            // Medium's Camera
            this.camera.Update(gameTime);
            Vector2 mediumPos = new Vector2(medium.space.X, medium.space.Y);
            this.camera.Position = mediumPos;

            // Demon's Camera
            this.demonCamera.Update(gameTime);
            Vector2 demonPos = new Vector2(demon.space.X, demon.space.Y);
            this.demonCamera.Position = demonPos;

            // DamageBoxes
            
            // This is a felony
            int boxes = _dmgBoxes.Count;
            int i = 0;
            while (i < boxes)
            {
                // Decay timer
                _dmgBoxes[i].update();

                if (_dmgBoxes[i].timer == 0)
                {
                    _dmgBoxes.RemoveAt(i);
                    boxes = _dmgBoxes.Count;
                    continue;
                }
                // TODO: Do the rest of the collision logic for damage boxes.
                // if a character is inside of one, they die, or whatever.
                // Also you need to draw damage boxes.
                _dmgBoxes[i].checkCollision(medium);
                _dmgBoxes[i].checkCollision(demon);
                i++;
            }

            // Controls
            KeyboardState myState = Keyboard.GetState();
            if (_lastState == null)
                _lastState = Keyboard.GetState();

            // Medium Keys
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

            if (myState.IsKeyDown(Keys.Q))
            {
                _dmgBoxes.Add(medium.BeamAttack());
            }

            // Demon Keys
            if (myState.IsKeyDown(Keys.I))
            {
                demon.Jump();
            }

            if (myState.IsKeyDown(Keys.J))
            {
                demon.Walk(Direction.LEFT);
            }

            if (myState.IsKeyDown(Keys.L))
            {
                demon.Walk(Direction.RIGHT);
            }

            if (myState.IsKeyDown(Keys.U))
            {
                _dmgBoxes.Add(demon.BlastAttack());
            }

            if (myState.IsKeyDown(Keys.K))
            {
                demon.SwitchPlanes();
            }

            medium.UpdatePhysics(_platforms);
            medium.abilitiesTick();
            demon.UpdatePhysics(_platforms);
            demon.abilitiesTick();

            _lastState = Keyboard.GetState();
			base.Update(gameTime);
		}

        private void viewportSprites(Viewport port, Camera cam, Player player)
        {
            GraphicsDevice.Viewport = port;
            _spriteBatch.Begin(cam);
            if (player.currentPlane == Plane.ETHEREAL)
            {
                RectangleSprite.FillRectangle(
                    _spriteBatch,
                    new Rectangle(
                        (int)cam.Position.X-port.Width/2,
                        (int)cam.Position.Y-port.Height/2,
                        port.Width,
                        port.Height
                    ),
                    new Color(0, 20, 50)
                );
                //GraphicsDevice.Clear(new Color(0, 20, 50));
            }
            // TODO: Add your drawing code here
            foreach(WorldObject obj in _platforms)
            {
                obj.DrawBox(_spriteBatch);
            }
            foreach(DamageBox obj in _dmgBoxes)
            {
                obj.DrawBox(_spriteBatch);
            }
            if (player.currentPlane == medium.currentPlane)
            {
                medium.DrawBox(_spriteBatch);
                medium.Draw(_spriteBatch);
            }
            if (player.currentPlane == demon.currentPlane)
            {
                demon.DrawBox(_spriteBatch);
                demon.Draw(_spriteBatch);
            }
            // Draw the hud of a given player
            _spriteBatch.End();
        }

        private void drawHUD(Player player, bool drawOnBottom)
        {
            int HUDHeight = 50;
            if (drawOnBottom)
            {
                HUDHeight = defaultViewport.Height/2 - 50;
            }
            _spriteBatch.Begin();

            // Life Bar
            string textHP = $"LIFE: {player.health}";
            Vector2 HUDSize = _HUDFont.MeasureString(textHP);
            _spriteBatch.DrawString(
                _HUDFont, 
                textHP, 
                new Vector2(
                    defaultViewport.Width / 2 - HUDSize.X / 2,
                    HUDHeight
                    ), 
                Color.White
            );

            RectangleSprite.FillRectangle(
                _spriteBatch,
                new Rectangle(
                    0,
                    HUDHeight + 20,
                    defaultViewport.Width,
                    5
                ),
                Color.Red
            );

            RectangleSprite.FillRectangle(
                _spriteBatch,
                new Rectangle(
                    0,
                    HUDHeight + 20,
                    (int)((float)defaultViewport.Width * ((float)player.health/(float)player.healthMax)),
                    5
                ),
                Color.Green
            );

            // Demon HUD
            if (player == demon)
            {
                // PlaneShift Cooldown
                Color planeshiftTextColor = Color.White;
                if (demon.planeSwitchTick > 0)
                    planeshiftTextColor = Color.Gray;

                string textPlaneshift = "PLANESHIFT";
                Vector2 textPlaneshiftSize = _HUDFont.MeasureString(textPlaneshift);
                _spriteBatch.DrawString(
                    _HUDFont, 
                    textPlaneshift, 
                    new Vector2(
                        20,
                        HUDHeight + 30
                    ), 
                    planeshiftTextColor
                );

                RectangleSprite.FillRectangle(
                    _spriteBatch,
                    new Rectangle(
                        20,
                        HUDHeight + 50,
                        defaultViewport.Width/5,
                        5
                    ),
                    Color.Black
                );

                RectangleSprite.FillRectangle(
                    _spriteBatch,
                    new Rectangle(
                        20,
                        HUDHeight + 50,
                        (int)(((float)defaultViewport.Width/5.0f) * 
                            ((float) demon.planeSwitchTick / (float) demon.planeSwitchCooldown)),
                        5
                    ),
                    Color.Orange
                );

            }
            _spriteBatch.End();
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

            viewportSprites(leftViewport, camera, medium);
            drawHUD(medium, true);
            viewportSprites(rightViewport, demonCamera, demon);
            drawHUD(demon, false);

            GraphicsDevice.Viewport = defaultViewport;
			_spriteBatch.Begin();
            // Draw black dividing bar
            RectangleSprite.FillRectangle(
                _spriteBatch,
                new Rectangle(
                    0,
                    defaultViewport.Height/2,
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
