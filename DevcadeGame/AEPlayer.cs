using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEPlayer : AEObject
    {
        // Health
        public int healthMax = 10;
        public int health = 10;

        // Movement
        private float _walkAccel = 1.0f;
        private float _maxWalkSpeed = 20.0f;
        private bool _hasJumped = true;
        private float _jumpPower = 30.0f;

        // Planar nonsense
        public Plane currentPlane;

        /// <summary>
        /// A boolean indicating if this ball is colliding with another
        /// </summary>
        public bool Colliding { get; protected set; }

        public AEPlayer(string spritePath, float spriteScale, Vector2 bodySize, Body body) : base(spritePath, spriteScale, bodySize, body)
        {
            this._spritePath = spritePath;
            this._spriteScale = spriteScale;
            this.BodySize = bodySize;
            this._body = body;
            _body.Mass = 1;

            this._body.OnCollision += CollisionHandler;
            this._body.FixedRotation = true;
        }

        /**** FUN STUFF ****/

        public void Walk(Direction direction)
        {
            //if (Math.Abs(_body.LinearVelocity.X) > _maxWalkSpeed)
            //    return;
            switch (direction)
            {
                case Direction.LEFT:
                    _body.ApplyLinearImpulse(new Vector2(-1 * _walkAccel, 0));
                    break;
                case Direction.RIGHT:
                    _body.ApplyLinearImpulse(new Vector2(_walkAccel, 0));
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
                //_hasJumped = true;
            }
        }

        /**** DATA ****/

        /**** MONOGAME PLUMBING ****/

        /// <summary>
        /// Draws the ball using the provided spritebatch
        /// </summary>
        /// <param name="gameTime">an object representing time in the game</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        /*public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _sprite,
                new Rectangle(
                    (int)((_body.Position.X * _spriteScale)),
                    (int)((_body.Position.Y * _spriteScale)),
                    (int)(BodySize.X * _spriteScale),
                    (int)(BodySize.Y * _spriteScale)
                ),
                Color.White
            );
        }*/

        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Colliding = true;
            _hasJumped = false;
            return true;
        }

    }
}
