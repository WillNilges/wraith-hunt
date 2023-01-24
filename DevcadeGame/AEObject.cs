using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public enum AETag
    {
        WORLD,
        MEDIUM,
        WRAITH,
        MEDIUMATTACK,
        WRAITHATTACK,
        KILLPLANE,
        NONE
    }

    public class AEObject
    {
        protected string _spritePath;
        protected Texture2D _sprite;
        protected float _spriteScale;
        protected Body _body;
        public Vector2 BodySize;

        /// <summary>
        /// A boolean indicating if this ball is colliding with another
        /// </summary>
        public bool Colliding { get; protected set; }

        public AEObject(string spritePath, float spriteScale, Vector2 bodySize, Body body)
        {
            this._spritePath = spritePath;
            this._spriteScale = spriteScale;
            this.BodySize = bodySize;
            this._body = body;
            _body.Mass = 1;
            _body.SetFriction(0.75f);

            this._body.OnCollision += CollisionHandler;
            this._body.FixedRotation = true;
            //this._body.FixtureList[0].Tag = AEObjectType.NONE;
            this._body.FixtureList[0].Tag = AETag.WORLD;
        }

        /**** FUN STUFF ****/

        // Nothing, bozo.

        /**** DATA ****/

        public Vector2 Position() => _body.Position;
        public Vector2 getBodySize() => this.BodySize;
        public float getSpriteScale() => this._spriteScale;

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
            spriteBatch.Draw(
                _sprite,
                GetCameraCoords(),
                Color.White
            );
        }

        public void DirectDraw(SpriteBatch spriteBatch)
        {
            RectangleSprite.FillRectangle(
                spriteBatch,
                GetCameraCoords(),
                Color.Yellow
            );
        }

        public void DrawOutline(SpriteBatch spriteBatch)
        {
            RectangleSprite.DrawRectangle(
                spriteBatch,
                GetCameraCoords(),
                Color.Yellow,
                20
            );
        }

        public Rectangle GetCameraCoords()
        {
            Rectangle dimensions = new Rectangle(
                    (int)((_body.Position.X * _spriteScale) - (BodySize.X * _spriteScale) / 2.0f),
                    (int)((_body.Position.Y * _spriteScale) - (BodySize.Y * _spriteScale) / 2.0f),
                    (int)(BodySize.X * _spriteScale),
                    (int)(BodySize.Y * _spriteScale)
                );
            return dimensions;
        }

        // Draw debug info on the screen, like position.
        // TODO: Boy howdy this is not how I'm using this at all. I think this should go into some kind of separate function. Would be nice if this could be global.
        public void DebugDraw(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position)
        {
            spriteBatch.DrawString(
                font,
                text,
                position,
                Color.Yellow
            );
        }

        public bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Colliding = true;
            return true;
        }

    }
}
