using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Security.Cryptography;
using tainicom.Aether.Physics2D.Dynamics;
using static System.Net.Mime.MediaTypeNames;
using Transform;
using Color = Microsoft.Xna.Framework.Color;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using DevcadeGame;

namespace WraithHunt
{
    public class AEMedium : AEPlayer
    {
        TimeSpan _beamAttackCooldown = new TimeSpan(0, 0, 0, 0, 500);
        TimeSpan _beamAttackTick;

        TimeSpan _blinkCooldown = new TimeSpan(0, 0, 12);
        TimeSpan _blinkTick = TimeSpan.Zero;
        float _blinkRange = 10f;

        Color mediumColor = new Color(198, 107, 255);

        public AEMedium(
            string spritePath, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteScale, bodySize, body, playerType
        )
        {
            this._body.OnCollision -= CollisionHandler;
            this._body.OnCollision += base.PlayerCollisionHandler;
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _beamAttackTick -= gameTime.ElapsedGameTime;
            _blinkTick -= gameTime.ElapsedGameTime;
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
                    _spriteScale,
                    attackSize,
                    world.CreateRectangle(
                        15f,
                        .5f,
                        1,
                        new Vector2(
                            _body.LinearVelocity.X > 0 ? _body.Position.X + attackSize.X / 2 + .5f : _body.Position.X - attackSize.X / 2 - .5f,
                            _body.Position.Y
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 2, new Vector2(10, -10)),
                    new TimeSpan(0, 0, 0, 1, 500),
                    true,
                    new Color(198, 107, 255),
                    new Vector2(_body.LinearVelocity.X > 0 ? 100 : -100, 0)
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
                Vector2 attackSize = new Vector2(1.5f, _blinkRange);
                Vector2 origin = new Vector2(
                            _body.Position.X + attackSize.X / 2,
                            _body.Position.Y - BodySize.Y
                        );
                currentPlane = WHPlane.ETHEREAL;
                //_collide = false;
                switch (dir)
                {
                    case Direction.UP:
                        _body.ApplyLinearImpulse(new Vector2(0, -20));
                        _body.Position = new Vector2(_body.Position.X, _body.Position.Y - _blinkRange);
                        break;
                    case Direction.DOWN:
                        _body.Position = new Vector2(_body.Position.X, _body.Position.Y + _blinkRange);
                        break;
                    case Direction.LEFT:
                        _body.Position = new Vector2(_body.Position.X - _blinkRange, _body.Position.Y);
                        break;
                    case Direction.RIGHT:
                        _body.Position = new Vector2(_body.Position.X + _blinkRange, _body.Position.Y);
                        break;
                }
                currentPlane = WHPlane.MATERIAL;
                _blinkTick = _blinkCooldown;
                // TODO: I need a particle system or something.
                return new AEDamageBox(
                    _spritePath,
                    _spriteScale,
                    attackSize,
                    world.CreateRectangle(
                        attackSize.X,
                        attackSize.Y,
                        1,
                        origin,
                        0,
                        BodyType.Dynamic
                    ),
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
