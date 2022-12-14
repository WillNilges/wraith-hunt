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

    public enum Plane
    {
        MATERIAL,
        ETHEREAL
    }

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
            this._body.Tag = 0;
        }

        /**** FUN STUFF ****/

        public void Walk(Direction direction)
        {
            if (Math.Abs(_body.LinearVelocity.X) > _maxWalkSpeed)
                return;
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
                _hasJumped = true;
            }
        }

        public AEDamageBox Attack(World world)
        {
            // Create a DamageBox that will last for 3 seconds.
            return new AEDamageBox(_spritePath, _spriteScale, BodySize, world.CreateRectangle(1.5f, 1.5f, 1, new Vector2(_body.Position.X+2f, _body.Position.Y), 0, BodyType.Dynamic), true, 1, new TimeSpan(0, 0, 3), Color.Red);
        }

        /**** DATA ****/

        /**** MONOGAME PLUMBING ****/

        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Colliding = true;
            _hasJumped = false;

            if ((AEObjectType) other.Tag == AEObjectType.DAMAGE)
            {
                health -= 2;
            }

            return true;
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
