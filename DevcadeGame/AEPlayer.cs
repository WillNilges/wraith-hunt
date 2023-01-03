using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Numerics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WraithHunt
{
    public enum WHPlane
    {
        MATERIAL,
        ETHEREAL
    }

    public class AEPlayer : AEObject
    {
        // Health
        public int healthMax = 10;
        public int health = 10;
        protected AETag playerType;

        // Movement
        protected Vector2 _startPosition;
        protected float _walkAccel = 1.0f;
        protected float _maxWalkSpeed = 20.0f;
        protected bool _hasJumped = true;
        protected float _jumpPower = 30.0f;
        protected float floorTileCludge = 0.001f;

        // Planar nonsense
        public WHPlane currentPlane;

        /// <summary>
        /// A boolean indicating if this ball is colliding with another
        /// </summary>
        public bool Colliding { get; protected set; }

        public AEPlayer(
            string spritePath, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteScale, bodySize, body
        )
        {
            this._spritePath = spritePath;
            this._spriteScale = spriteScale;
            this.BodySize = bodySize;
            this._body = body;
            _body.Mass = 1;

            //this._body.OnCollision += CollisionHandler;
            this._body.FixedRotation = true;
            //this._body.FixtureList[0].Tag = &this;
            this.playerType = playerType;
            this._startPosition = this._body.Position;
        }

        /**** FUN STUFF ****/
        public void Walk(Direction direction)
        {
            if (Math.Abs(_body.LinearVelocity.X) > _maxWalkSpeed)
                return;
            switch (direction)
            {
                case Direction.LEFT:
                    // REALLY cludgy hack to prevent the player from getting caught on floor tile corners.
                    if (_body.LinearVelocity.X == 0)
                        _body.Position = new Vector2(_body.Position.X - floorTileCludge, _body.Position.Y);
                    _body.ApplyLinearImpulse(new Vector2(-1 * _walkAccel, 0f));
                    break;
                case Direction.RIGHT:
                    if (_body.LinearVelocity.X == 0)
                        _body.Position = new Vector2(_body.Position.X + floorTileCludge, _body.Position.Y);
                    _body.ApplyLinearImpulse(new Vector2(_walkAccel, 0f));
                    break;
                default:
                    break;
            }
        }

        public void Jump()
        {
            if (!_hasJumped)
            {
                _body.ApplyLinearImpulse(new Vector2(0, -1 * _jumpPower));
                _hasJumped = true;
            }
        }

        /**** DATA ****/
        protected bool PlayerCollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Colliding = true;

            if (other.Tag == null)
                return true;

            // Only refresh jump if you're standing ON TOP of something.
            if (other.Tag is AETag && (AETag)other.Tag == AETag.WORLD)
            {
                if (_body.Position.Y - BodySize.Y < other.Body.Position.Y
                    && _body.LinearVelocity.Y >= 0)
                    _hasJumped = false;
                return true;
            }

            if (other.Tag is DamageFrom && ((DamageFrom)other.Tag).player != this)
            {
                health -= ((DamageFrom)other.Tag).damage;
                Vector2 kb = ((DamageFrom)other.Tag).knockback;
                _body.ApplyLinearImpulse(
                    new Vector2(other.Body.LinearVelocity.X > 0 ? kb.X : -1 * kb.X, kb.Y)
                );
                if (((DamageFrom)other.Tag).player != null) // Don't yeet the killplane
                    other.Tag = 0;
            }

            return false;
        }

        /**** MONOGAME PLUMBING ****/

        public void Reset()
        {
            // Put wraith on the Ethereal Plane.
            if (this is AEWraith)
            {
                this.currentPlane = WHPlane.ETHEREAL;
            }
            else
            {
                this.currentPlane = WHPlane.MATERIAL;
            }

            this._body.Position = _startPosition;
            this._body.LinearVelocity = Vector2.Zero;
            health = healthMax;
        }

        public void drawHUD(SpriteBatch spriteBatch, Viewport defaultViewport, SpriteFont font, bool drawOnBottom)
        {
            int HUDHeight = defaultViewport.Height / 2 - 50;
            if (drawOnBottom)
            {
                HUDHeight = defaultViewport.Height / 2 + 50;
            }

            // Life Bar
            string textHP = $"LIFE: {health}";
            Vector2 HUDSize = font.MeasureString(textHP);
            spriteBatch.DrawString(
                font,
                textHP,
                new Vector2(
                    defaultViewport.Width / 2 - HUDSize.X / 2,
                    HUDHeight + (drawOnBottom ? -10 : 0)
                    ),
                Color.White
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    0,
                    HUDHeight + (drawOnBottom ? -20 : 20),
                    defaultViewport.Width,
                    10
                ),
                Color.Red
            );

            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    0,
                    HUDHeight + (drawOnBottom ? -20 : 20),
                    (int)((float)defaultViewport.Width * ((float)health / (float)healthMax)),
                    10
                ),
                Color.Green
            );
        }
    }
}
