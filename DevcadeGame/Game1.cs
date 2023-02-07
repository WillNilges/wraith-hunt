using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using WraithHunt;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using Comora;
using System.Collections.Generic;
using System;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Content;

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
        private float _spriteOffset = 10f; // An offset multiplier for all sprites
        private float _spriteScale = 10f; // A scale multiplier for all sprites.

        Vector2 characterSize = new Vector2(1.5f, 3.0f);
        Vector2 mediumStartingPosition = new Vector2(140f, 170f); // new Vector2(40f, 60f);
        Vector2 wraithStartingPosition = new Vector2(150f, 170f);

        public SpriteFont _HUDFont;
        public SpriteFont _titleFont;
        private Texture2D _redButton;

        private World world;

        private AEMedium medium;
        private AEWraith wraith;

        private List<AEObject> tkThrowables; // List of shit the wraith can chuck at people

        private List<AEPlayer> npcs;

        private List<AEDamageBox> damageBoxes;

        private AEDamageBox killPlane;

        private Map map;


        private Camera mediumCamera;
        private Camera wraithCamera;

        public GameState state = GameState.START;

        // Here be dragons
        // Stuff for viewport
        Viewport defaultViewport;
        Viewport mediumViewport;
        Viewport wraithViewport;
        //Matrix projectionMatrix;
        //Matrix halfprojectionMatrix;

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
            mediumCamera = new Camera(_graphics.GraphicsDevice);
            wraithCamera = new Camera(_graphics.GraphicsDevice);
            mediumCamera.Zoom = 1f;
            wraithCamera.Zoom = 1f;

            world = new World();
            world.Gravity = new Vector2(0, 40f);

            map = new Map("Content/apartment_block.tmx", "chom_map_2", _spriteScale, _spriteOffset);


            medium = new AEMedium(
                "medium_placeholder_02/medium_placeholder_red_hood_128x128",
                _spriteOffset,
                _spriteScale,
                characterSize,
                world.CreateRectangle(
                    characterSize.X, characterSize.Y, 1, mediumStartingPosition, 0, BodyType.Dynamic
                ), // Player hitboxes are twice as tall as they are wide.
                AETag.WRAITH
            );

            wraith = new AEWraith(
                "demon_placeholder_01",
                _spriteOffset,
                _spriteScale,
                characterSize,
                world.CreateRectangle(
                    characterSize.X, characterSize.Y, 1, wraithStartingPosition, 0, BodyType.Dynamic
                ),
                AETag.MEDIUM
            );

            tkThrowables = new List<AEObject>();

            npcs = new List<AEPlayer>();

            damageBoxes = new List<AEDamageBox>();

            Vector2 killPlaneSize = new Vector2(1000f, 10f);

            killPlane = new AEDamageBox(
                null,
                _spriteScale,
                _spriteScale,
                killPlaneSize,
                world.CreateRectangle(
                    killPlaneSize.X,
                    killPlaneSize.Y,
                    1,
                    new Vector2(
                        -100f, 500f
                    ),
                    0,
                    BodyType.Static
                ),
                new DamageFrom(null, 100, new Vector2(15, -15)),
                new TimeSpan(1, 0, 0, 0, 0),
                false,
                Color.DarkGray,
                new Vector2(0, 0)
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
            mediumViewport = defaultViewport;
            wraithViewport = defaultViewport;
            mediumViewport.Height /= 2;
            wraithViewport.Height /= 2;
            wraithViewport.Y = wraithViewport.Height;

            _HUDFont = Content.Load<SpriteFont>("hudfont");
            _titleFont = Content.Load<SpriteFont>("titlefont");
            _redButton = Content.Load<Texture2D>("red_button");

            medium.LoadContent(Content);
            wraith.LoadContent(Content);

            for (int i = 0; i < 30; i++)
            {
                AEObject throwable = new AEObject(
                    "wraith_placeholder",
                    _spriteScale,
                    _spriteScale,
                    new Vector2(1.5f, 1.5f),
                    world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(wraithStartingPosition.X - 50f + (float)(i * 12f), wraithStartingPosition.Y - 100f), 0, BodyType.Dynamic)
                );

                throwable._body.FixedRotation = false;
                throwable.setTag(AETag.NONE);

                throwable.LoadContent(Content);

                tkThrowables.Add(throwable);
            }

            for (int i = 0; i < 10; i++)
            {
                AEPlayer npc = new AEPlayer(
                    "npc_placeholder_01",
                    _spriteOffset,
                    _spriteScale,
                    characterSize,
                    world.CreateRectangle(
                        characterSize.X, characterSize.Y, 1, new Vector2(wraithStartingPosition.X + i * 4, wraithStartingPosition.Y), 0, BodyType.Dynamic
                    ),
                    AETag.MEDIUM
                ); 

                npc.LoadContent(Content);
                npcs.Add(npc);
            }

            AEObject throwableDebug = new AEObject(
                    "wraith_placeholder",
                    _spriteScale,
                    _spriteScale,
                    new Vector2(1.5f, 1.5f),
                    world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(wraithStartingPosition.X + 2f, wraithStartingPosition.Y - 2f), 0, BodyType.Dynamic)
                );

            throwableDebug._body.FixedRotation = false;
            throwableDebug.setTag(AETag.NONE);

            throwableDebug.LoadContent(Content);

            tkThrowables.Add(throwableDebug);


            map.LoadContent(Content, world);
        }

        /// <summary>
        /// Your main update loop. This runs once every frame, over and over.
        /// </summary>
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        protected override void Update(GameTime gameTime)
        {
            // If a button might be held continuously, check for it here so that the rest of the input loop can continue
            // accordingly
            void handleHeldInputs(KeyboardState myState)
            {
                if (myState.IsKeyDown(Keys.Q) || Input.GetButtonDown(1, Input.ArcadeButtons.A3))
                    medium.BlinkButtonHeld = true;
            }

            void handlePlayingInput(KeyboardState myState)
            {
                handleHeldInputs(myState);

                /** Player 1 **/
                if (myState.IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.A1))
                {
                    if (!medium.BlinkButtonHeld)
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

                if (myState.IsKeyDown(Keys.Q) || Input.GetButtonDown(1, Input.ArcadeButtons.A3))
                {
                    medium.BlinkButtonHeld = true;
                    AEDamageBox box = null;
                    if (myState.IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        box = medium.Blink(Direction.UP, world);
                    }
                    else if (myState.IsKeyDown(Keys.S) || Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        box = medium.Blink(Direction.DOWN, world);
                    }
                    else if (myState.IsKeyDown(Keys.A) || Input.GetButtonDown(1, Input.ArcadeButtons.StickLeft))
                    {
                        box = medium.Blink(Direction.LEFT, world);
                    }
                    else if (myState.IsKeyDown(Keys.D) || Input.GetButtonDown(1, Input.ArcadeButtons.StickRight))
                    {
                        box = medium.Blink(Direction.RIGHT, world);

                    }
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

                if (myState.IsKeyDown(Keys.K) || Input.GetButtonDown(2, Input.ArcadeButtons.A3))
                {
                    wraith.SwitchPlanes();
                }

                if (myState.IsKeyDown(Keys.U) || Input.GetButtonHeld(2, Input.ArcadeButtons.A4))
                {
                    if (wraith.TKWeld == null)
                        wraith.TKAttach(world);
                    else
                    {
                        if (myState.IsKeyDown(Keys.I) || Input.GetButtonHeld(2, Input.ArcadeButtons.StickUp))
                        {
                            AEDamageBox box = wraith.TKBlast(world, Direction.UP);
                            if (box != null)
                                damageBoxes.Add(box);
                        } else if (myState.IsKeyDown(Keys.K) || Input.GetButtonHeld(2, Input.ArcadeButtons.StickDown))
                        {
                            AEDamageBox box = wraith.TKBlast(world, Direction.DOWN);
                            if (box != null)
                                damageBoxes.Add(box);
                        } else
                        {
                            AEDamageBox box = wraith.TKBlast(world, Direction.NONE);
                            if (box != null)
                                damageBoxes.Add(box);
                        }
                    }
                }
            }

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
                    foreach (AEPlayer npc in npcs)
                    {
                        npc.Reset();
                    }
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
                    handlePlayingInput(myState);

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
                    wraith.Update(gameTime, tkThrowables);

                    foreach (AEObject throwable in tkThrowables)
                    {
                        throwable.Update(gameTime);
                    }

                    foreach (AEPlayer npc in npcs)
                    {
                        npc.Update(gameTime);
                    }

                    foreach (AEDamageBox box in damageBoxes)
                    {
                        box.Update(gameTime, world);
                    }

                    // Delete damage boxes whose timers have expired
                    damageBoxes.RemoveAll(x => x.tick < TimeSpan.Zero);

                    mediumCamera.Position = new Vector2(medium.GetCameraCoords().X, medium.GetCameraCoords().Y + (mediumViewport.Height / 2) / mediumCamera.Zoom);
                    wraithCamera.Position = new Vector2(wraith.GetCameraCoords().X, wraith.GetCameraCoords().Y + (wraithViewport.Height / 2) / wraithCamera.Zoom);

                    // We are probably still holding inputs, and we need to deal with that 
                    handleHeldInputs(myState);

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

            void drawThings(Camera cam, AEPlayer player)
            {
                if (player.currentPlane == WHPlane.ETHEREAL)
                {
                    Color etherealBlue = new Color(0, 20, 50);
                    // Wraith Kludge
                    // FIXME: Why Width/2? Could be due to how I'm setting the camera to the position of the character.
                    RectangleSprite.FillRectangle(
                        _spriteBatch,
                        new Rectangle(
                            (int) (cam.Position.X - (GraphicsDevice.Viewport.Width/2) / cam.Zoom),
                            (int) (cam.Position.Y - (GraphicsDevice.Viewport.Height) / cam.Zoom),
                            (int) (GraphicsDevice.Viewport.Width/cam.Zoom),
                            (int) (GraphicsDevice.Viewport.Height/cam.Zoom)
                        ),
                        etherealBlue
                    );
                    //GraphicsDevice.Clear(new Color(0, 20, 50));
                }

                map.Draw(gameTime, _spriteBatch);

                foreach (AEObject throwable in tkThrowables)
                {
                    throwable.Draw(gameTime, _spriteBatch);
                }

                if (player.currentPlane == medium.currentPlane)
                    medium.Draw(gameTime, _spriteBatch);
                if (player.currentPlane == wraith.currentPlane)
                    wraith.Draw(gameTime, _spriteBatch);

                foreach (AEPlayer npc in npcs)
                {
                    npc.Draw(gameTime, _spriteBatch);
                }

                foreach (AEDamageBox box in damageBoxes)
                {
                    box.DrawBox(gameTime, _spriteBatch);
                }

                /*
                if (player == medium)
                {
                    
                }
                */

                if (player == wraith)
                {
                    wraith.DrawExtas(gameTime, _spriteBatch);
                }
                //killPlane.DrawBox(gameTime, _spriteBatch); // Don't draw the killplane; We don't need it.
            }

            switch (state)
            {
                case GameState.PLAYING:
                    GraphicsDevice.Viewport = mediumViewport;
                    _spriteBatch.Begin(mediumCamera);
                    drawThings(mediumCamera, medium);
                    _spriteBatch.End();

                    GraphicsDevice.Viewport = wraithViewport;
                    _spriteBatch.Begin(wraithCamera);
                    drawThings(wraithCamera, wraith);
                    _spriteBatch.End();

                    // Draw HUDs and other important stuff
                    GraphicsDevice.Viewport = defaultViewport;
                    _spriteBatch.Begin();

                    // DEBUG STATEMENTS
                    medium.DebugDraw(_spriteBatch, _HUDFont, $"Position: {medium.Position().X}, {medium.Position().Y}", new Vector2(1, 1));
                    medium.DebugDraw(_spriteBatch, _HUDFont, $"Cam_Position: {mediumCamera.Position.X}, {mediumCamera.Position.Y}", new Vector2(1, 20));
                    medium.DebugDraw(_spriteBatch, _HUDFont, $"Draw_Position: {medium.GetCameraCoords().X}, {medium.GetCameraCoords().Y}", new Vector2(1, 40));

                    wraith.DebugDraw(_spriteBatch, _HUDFont, $"Position: {wraith.Position().X}, {wraith.Position().Y}", new Vector2(150, 550));
                    wraith.DebugDraw(_spriteBatch, _HUDFont, $"Cam_Position: {wraithCamera.Position.X}, {wraithCamera.Position.Y}", new Vector2(150, 570));
                    wraith.DebugDraw(_spriteBatch, _HUDFont, $"Draw_Position: {wraith.GetCameraCoords().X}, {wraith.GetCameraCoords().Y}", new Vector2(150, 590));

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

                    // Draw HUDs
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
