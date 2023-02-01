using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEObject
    {
        protected string _spritePath;
        protected Texture2D _sprite;
        protected float _spriteOffset;
        protected float _spriteScale;
        public Body _body;
        public Vector2 BodySize;

        public AEObject(string spritePath, float spriteOffset, float spriteScale, Vector2 bodySize, Body body)
        {
            this._spritePath = spritePath;
            this._spriteOffset = spriteOffset;
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

        /**** DATA ****/

        public Body GetBody() => _body;
        public Vector2 Position() => _body.Position;
        public Vector2 Velocity() => _body.LinearVelocity;
        public Vector2 getBodySize() => this.BodySize;
        public float getSpriteScale() => this._spriteScale;

        public void setPosition(Vector2 position) => _body.Position = position;
        public void setVelocity(Vector2 velocity) => _body.LinearVelocity = velocity;

        public void setTag(AETag tag) => _body.FixtureList[0].Tag = tag;

        /**** MONOGAME PLUMBING ****/

        public void LoadContent(ContentManager contentManager)
        {
            _sprite = contentManager.Load<Texture2D>(_spritePath);
        }

        public void Update(GameTime gameTime)
        {

        }

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

        public void DrawOutline(SpriteBatch spriteBatch, Color color)
        {
            RectangleSprite.DrawRectangle(
                spriteBatch,
                GetCameraCoords(),
                color,
                3
            );
        }

        // Converts from world units (Aether2D units) to screen units
        public Rectangle GetCameraCoords()
        {
            return new Rectangle(
                    (int)Math.Floor((_body.Position.X * _spriteOffset) - (BodySize.X * _spriteOffset) / 2.0f),
                    (int)Math.Floor((_body.Position.Y * _spriteOffset) - (BodySize.Y * _spriteOffset) / 2.0f),
                    (int)Math.Ceiling(BodySize.X * _spriteScale) + 1,
                    (int)Math.Ceiling(BodySize.Y * _spriteScale) + 1
                );
        }

        // Draw debug info on the screen, like position.
        // TODO: Boy howdy this is not how I'm using this at all. I think this
        // should go into some kind of separate function. Would be nice if
        // this could be global. Maybe a singleton object.
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
            //AETag collidedWith;
            //if (other.Tag is AETag)
            //    collidedWith = (AETag)other.Tag;

            if (other.Tag is AETag && (AETag)other.Tag == AETag.WORLD)
                return true;

            return false;
        }
    }
}
