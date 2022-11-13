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

        public WorldObject(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor)
        {
            space = new SpaceProperties();
            space.X = xPos;
            space.Y = yPos;
            space.width = dimWidth;
            space.height = dimHeight;
            color = objColor;
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
    }
}
