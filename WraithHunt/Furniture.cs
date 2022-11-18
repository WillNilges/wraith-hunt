using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Devcade;

namespace WraithHunt
{
    public class Furniture : WorldObject
    {
        protected int _gravityMax = 30; // The maximum speed you can fall
        protected int _gravityAccel = 1; // Acceleration
        protected int _gravityInc = 8;
        protected int _currentGravity = 0;

        protected bool _collide = false;
        protected bool _airborne = false;

        protected int _velocityY = 0;
        protected int _velocityX = 0;

        protected WorldObject _collidingWith = null;

        public Furniture(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
        }

        public void UpdatePhysics(List<WorldObject> platforms)
        {

            // If you die, reset position to the top of screen and fall back into the world (for now)
            /*if (health <= 0)
            {
                space.X = 150;
                space.Y = 0;
                health = healthMax;
            }*/

            _collide = false;

            // Check if we're colliding with a world object.
            foreach(WorldObject platform in platforms)
            {
                if (
                    _airborne && _currentGravity < 0 &&
                    platform.space.X < space.X + space.width &&
                    space.X < platform.space.X + platform.space.width &&
                    space.Y > platform.space.Y + platform.space.height &&
                    space.Y - _velocityY < platform.space.Y + platform.space.height
                )
                {
                    _collide = true;
                    _collidingWith = platform;
                    space.Y = platform.space.Y + platform.space.height + 1;
                    break;
                }
                if (
                    platform.space.X < space.X + space.width &&
                    space.X < platform.space.X + platform.space.width &&
                    platform.space.Y < space.Y + space.height &&
                    space.Y < platform.space.Y + platform.space.height
                )
                {
                    _collide = true;
                    _airborne = false;
                    _collidingWith = platform;
                    space.Y = platform.space.Y - space.height + 1;
                    break;
                }
            }

            if (_collide) 
            {
                _currentGravity = 0;
            }
            else if (_currentGravity < _gravityMax)
            {
                _currentGravity += _gravityAccel;
            }

            space.Y += (int)((double) _gravityInc * ((double) _currentGravity / (double) _gravityMax));
        }


    }
}
