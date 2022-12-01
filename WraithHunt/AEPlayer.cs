using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using Transform;

namespace WraithHunt
{
    public class AEPlayer
    {
        private string spritePath;
        private Texture2D sprite;
        public Vector2 spriteSize;
        private Body body;

        // Health
        public int healthMax = 10;
        public int health = 10;

        // Planar nonsense

        public Plane currentPlane;

        /// <summary>
        /// A boolean indicating if this ball is colliding with another
        /// </summary>
        public bool Colliding { get; protected set; }

        public AEPlayer(string spritePath, Vector2 spriteSize, Body body)
        {
            this.spritePath = spritePath;
            this.spriteSize = spriteSize;
            this.body = body;
        }

        /**** FUN STUFF ****/

        public void Walk(Direction dir)
        {
            switch (dir)
            {
                case Direction.LEFT:
                    
                    break;
                case Direction.RIGHT:
                    
                    break;
            }
        }

        public void Jump()
        {

        }

        /**** DATA ****/

        public Vector2 Position() => body.Position;

        /**** MONOGAME PLUMBING ****/

        /// <summary>
        /// Loads the ball's texture
        /// </summary>
        /// <param name="contentManager">The content manager to use</param>
        public void LoadContent(ContentManager contentManager)
        {
            sprite = contentManager.Load<Texture2D>(spritePath);
        }

        /// <summary>
        /// Updates the ball
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public void Update(GameTime gameTime)
        {
            // Clear the colliding flag 
            Colliding = false;
        }

        /// <summary>
        /// Draws the ball using the provided spritebatch
        /// </summary>
        /// <param name="gameTime">an object representing time in the game</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Use Green for visual collision indication
            Color color = (Colliding) ? Color.Green : Color.White;

            spriteBatch.Draw(
                sprite,
                new Rectangle(
                    (int) body.Position.X,
                    (int) body.Position.Y,
                    (int) spriteSize.X,
                    (int) spriteSize.Y
                ),
                null,
                Color.White,
                0,
                new Vector2(0, 0),
                SpriteEffects.None,
                0
            );

        }
    }
}
