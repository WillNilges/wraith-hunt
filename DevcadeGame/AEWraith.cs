using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEWraith : AEPlayer
    {
        TimeSpan _blastAttackCooldown = new TimeSpan(0, 0, 0, 0, 500);
        TimeSpan _blastAttackTick = TimeSpan.Zero;

        TimeSpan _planeShiftCooldown = new TimeSpan(0, 0, 10);
        TimeSpan _planeShiftTick = TimeSpan.Zero;

        public AEWraith(
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
            _blastAttackTick -= gameTime.ElapsedGameTime;
            _planeShiftTick -= gameTime.ElapsedGameTime;
        }


        public void drawHUD(SpriteBatch spriteBatch, Viewport defaultViewport, SpriteFont font, bool drawOnBottom)
        {
            base.drawHUD(spriteBatch, defaultViewport, font, drawOnBottom);
            
            int HUDHeight = defaultViewport.Height / 2 - 50;
            if (drawOnBottom)
            {
                HUDHeight = defaultViewport.Height / 2 + 50;
            }

            // PlaneShift Cooldown
            Color planeshiftTextColor = Color.White;
            if (_planeShiftTick > TimeSpan.Zero)
                planeshiftTextColor = Color.Gray;

            // Planeshift cooldown bar
            string textPlaneshift = "PLANESHIFT";
            Vector2 textPlaneshiftSize = font.MeasureString(textPlaneshift);
            spriteBatch.DrawString(
                font,
                textPlaneshift,
                new Vector2(
                    20,
                    HUDHeight + 30
                ),
                planeshiftTextColor
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    20,
                    HUDHeight + 50,
                    defaultViewport.Width / 5,
                    5
                ),
                Color.Black
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    20,
                    HUDHeight + 50,
                    (int)(((float)defaultViewport.Width / 5.0f) *
                        ((float) _planeShiftTick.TotalMilliseconds / (float)_planeShiftCooldown.TotalMilliseconds)),
                    5
                ),
                Color.Orange
            );
        }

        public void Reset()
        {
            base.Reset();
            _blastAttackTick = TimeSpan.Zero;
            _planeShiftTick = TimeSpan.Zero;
        }

        public AEDamageBox Attack(World world)
        {
            if (_blastAttackTick < TimeSpan.Zero)
            {
                _blastAttackTick = _blastAttackCooldown;
                Vector2 attackSize = new Vector2(5.5f, 5.5f);
                return new AEDamageBox(
                    _spritePath,
                    _spriteScale,
                    attackSize,
                    world.CreateRectangle(
                        attackSize.X,
                        attackSize.Y,
                        1,
                        new Vector2(
                            _body.Position.X + (_body.LinearVelocity.X > 0 ? attackSize.X/2 + .5f : -1 * ( attackSize.Y/2 + .5f)),
                            _body.Position.Y - attackSize.Y/4
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 4, new Vector2(15, -15)),
                    new TimeSpan(0, 0, 0, 0, 500),
                    true,
                    Color.Orange,
                    new Vector2(_body.LinearVelocity.X > 0 ? 10 : -10, 0)
                    );

                //true, 1, new TimeSpan(0, 0, 0, 0, 500), Color.Red, playerType, new Vector2(_body.LinearVelocity.X > 0 ? 30 : -30, 0));
            }
            return null;
        }

        public void SwitchPlanes()
        {
            if (_planeShiftTick <= TimeSpan.Zero)
            {
                System.Diagnostics.Debug.WriteLine("Switching Planes...");
                if (currentPlane == WHPlane.MATERIAL)
                {
                    currentPlane = WHPlane.ETHEREAL;
                }
                else if (currentPlane == WHPlane.ETHEREAL)
                {
                    currentPlane = WHPlane.MATERIAL;
                }
                _planeShiftTick = _planeShiftCooldown;
            }
        }
    }
}
