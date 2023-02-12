using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using Color = Microsoft.Xna.Framework.Color;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Devcade;

namespace WraithHunt
{
    public class AEMedium : AEPlayer
    {
        TimeSpan _beamAttackCooldown = new TimeSpan(0, 0, 0, 0, 500);
        TimeSpan _beamAttackTick;

        // Blink
        TimeSpan _blinkCooldown = new TimeSpan(0, 0, 2); // DEBUG: Should be 12 seconds
        TimeSpan _blinkTick = TimeSpan.Zero;
        float _blinkRange = 10f;
        public bool BlinkButtonHeld = false;

        Color mediumColor = new Color(198, 107, 255);

        public AEMedium(
            string spritePath, float spriteOffset, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteOffset, spriteScale, bodySize, body, playerType
        )
        {
        }

        public void handleHeldInputs(KeyboardState myState)
        {
            if (myState.IsKeyDown(Keys.Q) || Input.GetButtonDown(1, Input.ArcadeButtons.A3))
                BlinkButtonHeld = true;
        }

        public void HandleInput(KeyboardState myState, World world, List<AEDamageBox> damageBoxes)
        {
            if (myState.IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.A1))
            {
                if (!BlinkButtonHeld)
                    Jump();
            }

            if (myState.IsKeyDown(Keys.A) || Input.GetButtonHeld(1, Input.ArcadeButtons.StickLeft))
            {
                Walk(Direction.LEFT);
            }

            if (myState.IsKeyDown(Keys.D) || Input.GetButtonHeld(1, Input.ArcadeButtons.StickRight))
            {
                Walk(Direction.RIGHT);
            }

            if (myState.IsKeyDown(Keys.E) || Input.GetButtonHeld(1, Input.ArcadeButtons.A2))
            {
                AEDamageBox box = Attack(world);
                if (box != null)
                    damageBoxes.Add(box);
            }

            if (myState.IsKeyDown(Keys.Q) || Input.GetButtonDown(1, Input.ArcadeButtons.A3))
            {
                BlinkButtonHeld = true;
                AEDamageBox box = null;
                if (myState.IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                {
                    box = Blink(Direction.UP, world);
                }
                else if (myState.IsKeyDown(Keys.S) || Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                {
                    box = Blink(Direction.DOWN, world);
                }
                else if (myState.IsKeyDown(Keys.A) || Input.GetButtonDown(1, Input.ArcadeButtons.StickLeft))
                {
                    box = Blink(Direction.LEFT, world);
                }
                else if (myState.IsKeyDown(Keys.D) || Input.GetButtonDown(1, Input.ArcadeButtons.StickRight))
                {
                    box = Blink(Direction.RIGHT, world);
                }
                if (box != null)
                    damageBoxes.Add(box);
            }
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _beamAttackTick -= gameTime.ElapsedGameTime;
            _blinkTick -= gameTime.ElapsedGameTime;
            BlinkButtonHeld = false;
        }

        public void drawHUD(SpriteBatch spriteBatch, Viewport defaultViewport, SpriteFont font, bool drawOnBottom)
        {
            base.drawHUD(spriteBatch, defaultViewport, font, drawOnBottom);

            int HUDHeight = defaultViewport.Height / 2 - 50;
            if (drawOnBottom)
            {
                HUDHeight = defaultViewport.Height / 2 + 50;
            }

            // Blink Cooldown
            Color blinkTextColor = Color.White;
            if (_blinkTick > TimeSpan.Zero)
                blinkTextColor = Color.Gray;
            else if (BlinkButtonHeld)
                blinkTextColor = mediumColor;

            string textPlaneshift = "BLINK";
            Vector2 textPlaneshiftSize = font.MeasureString(textPlaneshift);
            spriteBatch.DrawString(
                font,
                textPlaneshift,
                new Vector2(
                    20,
                    HUDHeight - 20
                ),
                blinkTextColor
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    20,
                    HUDHeight - 25,
                    defaultViewport.Width / 5,
                    5
                ),
                Color.Black
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    20,
                    HUDHeight - 25,
                    (int)(((float)defaultViewport.Width / 5.0f) *
                        ((float)_blinkTick.TotalMilliseconds / (float)_blinkCooldown.TotalMilliseconds)),
                    5
                ),
                mediumColor
            );
        }

        public void Reset()
        {
            base.Reset();
            _blinkTick = TimeSpan.Zero;
        }

        public AEDamageBox Attack(World world)
        {
            if (_beamAttackTick < TimeSpan.Zero)
            {
                _beamAttackTick = _beamAttackCooldown;
                Vector2 attackSize = new Vector2(15f, .5f);
                return new AEDamageBox(
                    _spritePath,
                    _spriteOffset,
                    _spriteOffset,
                    attackSize,
                    world.CreateRectangle(
                        15f,
                        .5f,
                        1,
                        new Vector2(
                            facing == Direction.RIGHT ? _body.Position.X + attackSize.X / 2 + .5f : _body.Position.X - attackSize.X / 2 - .5f,
                            _body.Position.Y
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 2, new Vector2(10, -10)),
                    new TimeSpan(0, 0, 0, 1, 500),
                    true,
                    new Color(198, 107, 255),
                    new Vector2(facing == Direction.RIGHT ? 100 : -100, 0)
                    );


                //true, 1, new TimeSpan(0, 0, 0, 0, 500), Color.Red, playerType, new Vector2(_body.LinearVelocity.X > 0 ? 30 : -30, 0));
            }
            return null;
        }

        // Dash abilitiy, enter the ethereal plane, pop out like 4 tiles in a direction
        public AEDamageBox Blink(Direction dir, World world)
        {
            if (currentPlane == WHPlane.MATERIAL && _blinkTick <= TimeSpan.Zero)
            {
                Vector2 attackSize;
                attackSize = new Vector2(1.5f, _blinkRange);
                Vector2 origin = new Vector2(
                    _body.Position.X,
                    _body.Position.Y
                );
                currentPlane = WHPlane.ETHEREAL;
                //_collide = false;
                switch (dir)
                {
                    case Direction.UP:
                        origin = new Vector2(
                            _body.Position.X,
                            _body.Position.Y - BodySize.Y
                        );
                        _body.LinearVelocity = new Vector2(_body.LinearVelocity.X, -20); // 'Lil boost
                        _body.Position = new Vector2(_body.Position.X, _body.Position.Y - _blinkRange);
                        attackSize = new Vector2(1.5f, _blinkRange);
                        break;
                    case Direction.DOWN:
                        origin = new Vector2(
                            _body.Position.X,
                            _body.Position.Y + BodySize.Y
                        );
                        _body.LinearVelocity = new Vector2(_body.LinearVelocity.X, 0); // No boost down (Not really necessary?)
                        _body.Position = new Vector2(_body.Position.X, _body.Position.Y + _blinkRange);
                        attackSize = new Vector2(1.5f, _blinkRange);
                        break;
                    case Direction.LEFT:
                        origin = new Vector2(
                            _body.Position.X,
                            _body.Position.Y + BodySize.Y / 2
                        );
                        _body.LinearVelocity = new Vector2(_body.LinearVelocity.X - 5, _body.LinearVelocity.Y / 2); // 'Lil boost, cancel some vertical momentum.
                        _body.Position = new Vector2(_body.Position.X - _blinkRange, _body.Position.Y);
                        attackSize = new Vector2(_blinkRange, 1.5f);
                        break;
                    case Direction.RIGHT:
                        origin = new Vector2(
                            _body.Position.X + BodySize.X * 2,
                            _body.Position.Y + BodySize.Y / 2
                        );
                        _body.LinearVelocity = new Vector2(_body.LinearVelocity.X + 5, _body.LinearVelocity.Y / 2); // 'Lil boost, cancel some vertical momentum.
                        _body.Position = new Vector2(_body.Position.X + _blinkRange, _body.Position.Y);
                        attackSize = new Vector2(_blinkRange, 1.5f);
                        break;
                }
                currentPlane = WHPlane.MATERIAL;
                _blinkTick = _blinkCooldown;
                // TODO: I need a particle system or something.
                _hasJumped = true; // Don't let the player jump after a blink. 
                return new AEDamageBox(
                    _spritePath,
                    _spriteOffset,
                    _spriteOffset,
                    attackSize,
                    world.CreateRectangle(attackSize.X, attackSize.Y, 1, origin, 0, BodyType.Dynamic),
                    new DamageFrom(this, 0, new Vector2(10, -10)),
                    new TimeSpan(0, 0, 0, 2, 500),
                    true,
                    mediumColor,
                    new Vector2(0, 0)
                    );

                //return new AEDamageBox(space.X + 30, space.Y, Direction.LEFT, 30, _blinkRange, Color.BlueViolet, _beamDuration, 0, _beamDecays, this);
            }
            return null;
        }
    }
}
