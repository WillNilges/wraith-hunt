using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WraithHunt
{
    public class AEPlayer : AEObject
    {
        // Health
        public int healthMax = 10;
        public int health = 10;
        protected AETag playerType;

        // Movement
        protected Vector2 _startPosition;
        protected float _walkAccel = 2.0f;
        protected float _maxWalkSpeed = 20.0f;
        protected bool _hasJumped = true;
        protected float _jumpPower = 30.0f;
        protected float floorTileCludge = 0.001f;
        protected Direction facing;

        // Planar nonsense
        public WHPlane currentPlane;

        public AEPlayer(
            string spritePath, float spriteOffset, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteOffset, spriteScale, bodySize, body
        )
        {
            this._spritePath = spritePath;
            this._spriteOffset = spriteOffset;
            this._spriteScale = spriteScale;
            this.BodySize = bodySize;
            this._body = body;
            _body.Mass = 1;

            this._body.OnCollision -= base.CollisionHandler;
            this._body.OnCollision += PlayerCollisionHandler;
            this._body.OnSeparation += PlayerSeparationHandler;
            this._body.FixedRotation = true;
            this.playerType = playerType;
            this._startPosition = this._body.Position;
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


        public void Update(GameTime gameTime, World world)
        {
            base.Update(gameTime);
            if (health <= 0)
            {
                world.Remove(_body);
            }
        }

        public new void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (facing == Direction.RIGHT)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Rectangle dimensions = GetCameraCoords();

            dimensions.X -= dimensions.Width / 2; // Properly center the sprite within the hitbox
            dimensions.Width *= 2; // Players are twice as tall as they are wide. 

            spriteBatch.Draw(
                _sprite,
                dimensions,
                null,
                Color.White,
                0,
                new Vector2(0, 0),
                effects,
                0
            );

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
                Color.LimeGreen
            );
        }

        /**** FUN STUFF ****/

        public void Walk(Direction direction)
        {
            Walk(direction, 1.0f);
        }

        public void Walk(Direction direction, float walkMultiplier)
        {
            float walkMax = _maxWalkSpeed * walkMultiplier;
            facing = direction;
            if (Math.Abs(_body.LinearVelocity.X) > walkMax)
                return;
            switch (direction)
            {
                case Direction.LEFT:
                    // REALLY cludgy hack to prevent the player from getting caught on floor tile corners.
                    if (_body.LinearVelocity.X == 0)
                        _body.Position = new Vector2(_body.Position.X - floorTileCludge, _body.Position.Y);
                    _body.ApplyLinearImpulse(new Vector2(-1 * _walkAccel, 0f));
                    if (_body.LinearVelocity.X < -1 * walkMax)
                        _body.LinearVelocity = new Vector2(-1 * walkMax, _body.LinearVelocity.Y);

                    break;
                case Direction.RIGHT:
                    if (_body.LinearVelocity.X == 0)
                        _body.Position = new Vector2(_body.Position.X + floorTileCludge, _body.Position.Y);
                    _body.ApplyLinearImpulse(new Vector2(_walkAccel, 0f));
                    if (_body.LinearVelocity.X > walkMax)
                        _body.LinearVelocity = new Vector2(walkMax, _body.LinearVelocity.Y);
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

            // Handle getting hit by an attack
            if (other.Tag is DamageFrom && ((DamageFrom)other.Tag).player != this)
            {
                health -= ((DamageFrom)other.Tag).damage;
                Vector2 kb = ((DamageFrom)other.Tag).knockback;
                _body.ApplyLinearImpulse(
                    new Vector2(other.Body.LinearVelocity.X > 0 ? kb.X : -1 * kb.X, kb.Y) //FIXME: Is this where the unidirectional knockback bug is happening?
                );
                if (((DamageFrom)other.Tag).player != null) // Don't yeet the killplane
                    other.Tag = 0;
            }
            return false;
        }

        protected void PlayerSeparationHandler(Fixture sender, Fixture other, Contact contact)
        {
            //if (other.Tag is AETag && (AETag)other.Tag == AETag.WORLD)
            //    _hasJumped = true;
        }
    }
}
