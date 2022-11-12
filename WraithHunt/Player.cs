using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

namespace WraithHunt
{
    public class Player : WorldObject 
    {
        private int _gravityMax = -3; // The maximum speed you can fall
        private int _gravityAccel = -1; // Acceleration


        private int _currentGravity = 0;

        public Player(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
        }

        public void UpdatePhysics(List<WorldObject> platforms)
        {
            _currentGravity = -1;
            // Check if we're colliding with a world object.
            foreach(WorldObject platform in platforms)
            {
                /*
                Console.WriteLine($"Our Space:{space.Y}, Their Space: {platform.space.Y}");
                if (platform.space.Y < space.Y)*/
                if (
                    platform.space.X < space.X + space.width &&
                    space.X < platform.space.X + platform.space.width &&
                    platform.space.Y < space.Y + space.height &&
                    space.Y < platform.space.Y + platform.space.height
                )
                {
                    _currentGravity = 0;
                }
            }

            space.Y -= _currentGravity;
        }
    }
}
