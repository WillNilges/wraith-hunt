using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEPlayer
    {
        private string _spritePath;
        private Texture2D _sprite;
        private float _spriteScale;
        private Body _body;
        public Vector2 BodySize;

        // Health
        public int healthMax = 10;
        public int health = 10;

        // Grounded
        private bool _hasJumped = true;

        // Planar nonsense
        public Plane currentPlane;

        /// <summary>
        /// A boolean indicating if this ball is colliding with another
        /// </summary>
        public bool Colliding { get; protected set; }

        public AEPlayer(string spritePath, float spriteScale, Vector2 bodySize, Body body)
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

        public void Jump()
        {
            if (!_hasJumped)
            {
                _body.ApplyLinearImpulse(new Vector2(0, -30));
                _hasJumped = true;
            }
        }

        /**** DATA ****/

        public Vector2 Position() => _body.Position;

        /**** MONOGAME PLUMBING ****/

        /// <summary>
        /// Loads the ball's texture
        /// </summary>
        /// <param name="contentManager">The content manager to use</param>
        public void LoadContent(ContentManager contentManager)
        {
            _sprite = contentManager.Load<Texture2D>(_spritePath);
        }

        /// <summary>
        /// Updates the ball
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public void Update(GameTime gameTime)
        {
            // Clear the colliding flag 
            Colliding = false;
            _body.Rotation = 0;
        }

        /// <summary>
        /// Draws the ball using the provided spritebatch
        /// </summary>
        /// <param name="gameTime">an object representing time in the game</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 screenCoords = new Vector2(_body.Position.X, _body.Position.Y);

            spriteBatch.Draw(_sprite, new Rectangle((int)(screenCoords.X * _spriteScale), (int)(screenCoords.Y * _spriteScale), (int)(BodySize.X * _spriteScale), (int)(BodySize.Y * _spriteScale)), Color.White);

            /*
            spriteBatch.Draw(
                _sprite,
                screenCoords,
                null,
                Color.White,
                _body.Rotation,
                new Vector2(0,0),
                new Vector2(.5f, .5f),
                SpriteEffects.None,
                0f
            );
            */
        }


        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Colliding = true;
            _hasJumped = false;
            return true;
        }

    }
}
