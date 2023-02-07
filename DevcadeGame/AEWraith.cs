using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace WraithHunt
{
    public class AEWraith : AEPlayer
    {
        TimeSpan _blastAttackCooldown = new TimeSpan(0, 0, 0, 0, 500);
        TimeSpan _blastAttackTick = TimeSpan.Zero;

        TimeSpan _planeShiftCooldown = new TimeSpan(0, 0, 10);
        TimeSpan _planeShiftTick = TimeSpan.Zero;

        // Telekinesis
        TimeSpan _TKCooldown = new TimeSpan(0,0,2); // DEBUG: should be 15 seconds.
        TimeSpan _TKTick = TimeSpan.Zero;
        private int _TKRange = 20;
        private AEObject _TKCandidate;
        public RopeJoint TKWeld = null;
        private float _TKLength = 4f;
        private float _TKBoost = -0.1f;

        private Vector2 _TKBlastForce = new Vector2(100f, 20f);

        public Color WraithColor = Color.Orange;

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
            if (TKWeld != null)
            {
                _TKCandidate._body.ApplyLinearImpulse(new Vector2(0, _TKBoost));
            }
        }

        public void DrawExtas(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw an outline of the nearest throwable on the Wraith's screen.
            if (_TKTick <= TimeSpan.Zero && _TKCandidate != null && TKWeld == null)
                _TKCandidate.DrawOutline(spriteBatch, Color.Yellow);
            else if (TKWeld != null)
                _TKCandidate.DrawOutline(spriteBatch, WraithColor);
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
                WraithColor
            );

            // TKBlast Cooldown
            Color tkBlastTextColor = Color.White;
            if (_TKTick > TimeSpan.Zero || _TKCandidate == null)
                tkBlastTextColor = Color.Gray;
            else if (TKWeld != null)
                tkBlastTextColor = WraithColor;

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
                WraithColor
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
                    WraithColor,
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

        public void TKAttach(World world)
        {
            if (_TKCandidate != null && _TKTick <= TimeSpan.Zero)
            {
                TKWeld = new RopeJoint(
                    _body,
                    _TKCandidate._body,
                    new Vector2(0f, 0f),
                    new Vector2(0f, 0f),
                    false
                );
                TKWeld.MaxLength = _TKLength;
                world.Add(TKWeld);
                _TKCandidate._body.Mass = 0.1f;
                _TKTick = new TimeSpan(0, 0, 0, 0, 500);
            }
        }
        
        public void TKRelease(World world)
        {
            world.Remove(TKWeld);
            _TKCandidate._body.Mass = 1f;
            TKWeld = null;
        }

        public AEDamageBox TKBlast(World world, Direction direction)
        {
            if (_TKCandidate != null && _TKTick <= TimeSpan.Zero)
            {
                TKRelease(world);
                int dirMod = 1;
                if (facing == Direction.LEFT)
                    dirMod = -1;

                float lift = 0f;
                switch (direction)
                {
                    case Direction.UP:
                        lift = _TKBlastForce.Y * -1f;
                        break;
                    case Direction.DOWN:
                        lift = _TKBlastForce.Y;
                        break;
                    default:
                        break;
                }

                _TKCandidate.setPosition(new Vector2(_TKCandidate.Position().X + 0.5f * dirMod, _TKCandidate.Position().Y - 0.5f));

                _TKCandidate.setVelocity(new Vector2(_TKBlastForce.X * dirMod, lift));
                _TKTick = _TKCooldown;

                Vector2 attackSize = new Vector2(5.5f, 5.5f);
                _TKCandidate._body.FixtureList[0].Tag = new DamageFrom(this, 5, new Vector2(15, -15));
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
                    new DamageFrom(this, 2, new Vector2(15, -15)),
                    new TimeSpan(0, 0, 0, 0, 500),
                    true,
                    WraithColor,
                    new Vector2(facing == Direction.RIGHT ? 10 : -10, 0)
                    );
            }
            return null;
        }
    }
}
