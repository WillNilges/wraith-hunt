using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace WraithHunt
{
    public struct DamageFrom {
        public AEPlayer player;
        public int damage;

        public DamageFrom(AEPlayer player, int damage)
        {
            this.player = player;
            this.damage = damage;
        }
    }
    public class AEDamageBox : AEObject
    {
        private bool _hasHit = false;
        private bool _decay;
        public int damage;
        public TimeSpan duration;
        public TimeSpan tick;

        private Color color;

        public AEPlayer owner; // Who owns this device?

        public AEDamageBox(
            string spritePath, float spriteScale, Vector2 bodySize, Body body,
            DamageFrom damage, TimeSpan duration, bool decays, Color color, Vector2 velocity
         ) : base(spritePath, spriteScale, bodySize, body) {
            this._decay = decays;
            this.duration = duration;
            this.tick = duration;
            //this.damage = damage;
            this.color = color;
            //this._body.OnCollision += CollisionHandler;

            this._body.FixtureList[0].Tag = damage;
            this._body.IgnoreGravity = true;
            this._body.LinearVelocity = velocity;
        }

        /// <summary>
        /// Updates the ball
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public void Update(GameTime gameTime, World world)
        {
            tick -= gameTime.ElapsedGameTime;
            color = color * (float) (tick.TotalMilliseconds / duration.TotalMilliseconds);
            if (tick < TimeSpan.Zero)
            {
                world.Remove(_body);
                
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*spriteBatch.Draw(
                _sprite,
                new Rectangle(
                    (int)((_body.Position.X * _spriteScale) - (BodySize.X * _spriteScale) / 2.0f),
                    (int)((_body.Position.Y * _spriteScale) - (BodySize.Y * _spriteScale) / 2.0f),
                    (int)(BodySize.X * _spriteScale),
                    (int)(BodySize.Y * _spriteScale)
                ),
                Color.White
            );*/
        }

        public void DrawBox(GameTime gameTime, SpriteBatch spriteBatch)
        {
            RectangleSprite.FillRectangle(
                spriteBatch,
                new Rectangle(
                    (int)((_body.Position.X * _spriteScale) - (BodySize.X * _spriteScale) / 2.0f),
                    (int)((_body.Position.Y * _spriteScale) - (BodySize.Y * _spriteScale) / 2.0f),
                    (int)(BodySize.X * _spriteScale),
                    (int)(BodySize.Y * _spriteScale)
                ),
                color
            );
        }
    }
}