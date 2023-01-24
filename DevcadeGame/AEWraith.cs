using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        // Telekinesis
        TimeSpan _TKCooldown = new TimeSpan(0,0,15);
        TimeSpan _TKTick = TimeSpan.Zero;
        private int _TKRange = 20;
        private AEObject _TKCandidate;

        public AEWraith(
            string spritePath, float spriteOffset, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteOffset, spriteScale, bodySize, body, playerType
        )
        {
        }

        public AEObject getTkCandidate() => _TKCandidate;

        public void Update(GameTime gameTime, List<AEObject> throwables)
        {
            base.Update(gameTime);
            _blastAttackTick -= gameTime.ElapsedGameTime;
            _planeShiftTick -= gameTime.ElapsedGameTime;
            _TKTick -= gameTime.ElapsedGameTime;
            TKSearch(throwables);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public void DrawExtas(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw an outline of the nearest throwable on the Wraith's screen.
            if (_TKTick <= TimeSpan.Zero && _TKCandidate != null)
                _TKCandidate.DrawOutline(spriteBatch);
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
                        ((float)_planeShiftTick.TotalMilliseconds / (float)_planeShiftCooldown.TotalMilliseconds)),
                    5
                ),
                Color.Orange
            );


            // TKBlast Cooldown
            Color tkBlastTextColor = Color.White;
            if (_TKTick > TimeSpan.Zero || _TKCandidate == null)
                tkBlastTextColor = Color.Gray;

            // Planeshift cooldown bar
            string textTkBlast = "BLAST";
            Vector2 textTkBlastSize = font.MeasureString(textTkBlast);
            spriteBatch.DrawString(
                font,
                textTkBlast,
                new Vector2(
                    20,
                    HUDHeight + 70
                ),
                tkBlastTextColor
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    20,
                    HUDHeight + 90,
                    defaultViewport.Width / 5,
                    5
                ),
                Color.Black
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    20,
                    HUDHeight + 90,
                    (int)(((float)defaultViewport.Width / 5.0f) *
                        ((float)_TKTick.TotalMilliseconds / (float)_TKCooldown.TotalMilliseconds)),
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
            _TKTick = TimeSpan.Zero;
        }

        public AEDamageBox Attack(World world)
        {
            if (_blastAttackTick < TimeSpan.Zero)
            {
                _blastAttackTick = _blastAttackCooldown;
                Vector2 attackSize = new Vector2(5.5f, 5.5f);
                return new AEDamageBox(
                    _spritePath,
                    _spriteOffset,
                    _spriteOffset,
                    attackSize,
                    world.CreateRectangle(
                        attackSize.X,
                        attackSize.Y,
                        1,
                        new Vector2(
                            _body.Position.X + (facing == Direction.RIGHT ? attackSize.X / 2 + .5f : -1 * (attackSize.Y / 2 + .5f)),
                            _body.Position.Y - attackSize.Y / 4
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 4, new Vector2(15, -15)),
                    new TimeSpan(0, 0, 0, 0, 500),
                    true,
                    Color.Orange,
                    new Vector2(facing == Direction.RIGHT ? 10 : -10, 0)
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
        public void TKSearch(List<AEObject> furniture)
        {
            _TKCandidate = furniture[0];
            foreach (AEObject furn in furniture)
            {
                // How far away this furn is
                float furnDistX = Math.Abs(furn.Position().X - this._body.Position.X);
                float furnDistY = Math.Abs(furn.Position().Y - this._body.Position.Y);
                float furnPythag = (float)Math.Sqrt(furnDistX * furnDistX + furnDistY * furnDistY);

                // How far away the closest furn is
                float closestDistX = Math.Abs(_TKCandidate.Position().X - this._body.Position.X);
                float closestDistY = Math.Abs(_TKCandidate.Position().Y - this._body.Position.Y);
                float closestPythag = (float)Math.Sqrt(closestDistX * closestDistX + closestDistY * closestDistY);

                if (
                    furnPythag < _TKRange &&
                    furnPythag < closestPythag
                )
                {
                    _TKCandidate = furn;
                }
            }
            float distX = Math.Abs(_TKCandidate.Position().X - this._body.Position.X);
            float distY = Math.Abs(_TKCandidate.Position().Y - this._body.Position.Y);
            float pythag = (float)Math.Sqrt(distX * distX + distY * distY);
            if (pythag > _TKRange)
                _TKCandidate = null;
        }
        public AEDamageBox TKBlast(World world)
        {
            if (_TKCandidate != null && _TKTick <= TimeSpan.Zero)
            {
                int dirMod = 1;
                if (facing == Direction.LEFT)
                    dirMod = -1;

                _TKCandidate.setPosition(new Vector2(_TKCandidate.Position().X + 0.5f * dirMod, _TKCandidate.Position().Y - 0.5f));

                _TKCandidate.setVelocity(new Vector2(_TKCandidate.Velocity().X + 30f * dirMod, _TKCandidate.Velocity().Y - 30f));
                _TKTick = _TKCooldown;

                Vector2 attackSize = new Vector2(5.5f, 5.5f);
                return new AEDamageBox(
                    _spritePath,
                    _spriteOffset,
                    _spriteOffset,
                    attackSize,
                    world.CreateRectangle(
                        attackSize.X,
                        attackSize.Y,
                        1,
                        new Vector2(
                            _body.Position.X + (facing == Direction.RIGHT ? attackSize.X / 2 + .5f : -1 * (attackSize.Y / 2 + .5f)),
                            _body.Position.Y - attackSize.Y / 4
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 4, new Vector2(15, -15)),
                    new TimeSpan(0, 0, 0, 0, 500),
                    true,
                    Color.Orange,
                    new Vector2(facing == Direction.RIGHT ? 10 : -10, 0)
                    );
            }
            return null;
        }
    }
}
