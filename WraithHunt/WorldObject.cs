using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

namespace WraithHunt
{
    public class SpaceProperties
    {
        public int X {get; set;}
        public int Y {get; set;}
        public int width {get; set;}
        public int height {get; set;}
    }

    public class WorldObject 
    {
        public SpaceProperties space;
        public Color color;

        public Direction facing;
        public Texture2D sprite;
        public int spriteSize;
        public Rectangle spriteParams;
        public int spriteOffsetLeft {set; get;}
        public int spriteOffsetRight {set; get;}

        public Plane currentPlane {set; get;}

        public WorldObject(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor)
        {
            space = new SpaceProperties();
            space.X = xPos;
            space.Y = yPos;
            space.width = dimWidth;
            space.height = dimHeight;
            color = objColor;
            currentPlane = Plane.MATERIAL;
            spriteOffsetLeft = 0;
            spriteOffsetRight = 0;
        }

        public virtual void Draw(SpriteBatch batch)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (facing == Direction.LEFT)
            {
                effects = SpriteEffects.FlipHorizontally;
                spriteParams.X = space.X-spriteOffsetLeft;
            }
            else
            {
                spriteParams.X = space.X-spriteOffsetRight;
            }

            spriteParams.Y = space.Y-(15);

            Color planeColor = Color.White;
            
            switch (currentPlane)
            {
                case (Plane.MATERIAL):
                    planeColor = Color.White;
                    break;
                case (Plane.ETHEREAL):
                    planeColor = Color.DarkSlateBlue;
                    break;
            }

            batch.Draw(
                sprite, 
                spriteParams,
                null,
                planeColor,
                0,
                new Vector2(0,0),
                effects,
                0
            );
        }

        // Ignores SpriteParams and draws/scales sprite to hitbox
        public void DirectDraw(SpriteBatch batch)
        {
            batch.Draw(
                sprite,
                new Rectangle(
                    space.X,
                    space.Y,
                    space.width,
                    space.height 
                ),
                null,
                Color.White,
                0,
                new Vector2(0,0),
                SpriteEffects.None,
                0
            );
        }


        public void DrawBox(SpriteBatch batch)
        {
            RectangleSprite.FillRectangle(
                batch,
                new Rectangle(
                    space.X,
                    space.Y,
                    space.width,
                    space.height 
                ),
                color
            );
        }

        public Rectangle getHitbox()
        {
            return new Rectangle(
                space.X,
                space.Y,
                space.width,
                space.height 
            );
        }
    }
}
