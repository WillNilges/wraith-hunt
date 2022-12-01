using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

using TiledSharp;

namespace WraithHunt
{
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        NONE
    }

    public enum Plane
    {
        MATERIAL,
        ETHEREAL
        
    }

    public class Player : Furniture 
    {
        private int _jumpPower = 30;
        private int _jumpTick; // The amt of time left until the object starts falling
        private int _jumpInc = 8;
        private bool _hasJumped = false; // Checks if the player has jumped
        private int _jumpGraceMax = 10; // Allow player to jump N frames after they've stopped colliding
        private int _jumpGrace;

        private int _topSpeed = 4;

        public int healthMax = 10;
        public int health = 10;

        public Player(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
            currentPlane = Plane.MATERIAL;
        }

        public void reset(Vector2 pos)
        {
            health = healthMax;
            space.X = (int) pos.X;
            space.Y = (int) pos.Y;
        }

        public virtual void UpdatePhysics(List<WorldObject> platforms, TmxMap map, Texture2D tileset)
        {
            _airborne = _hasJumped;
            _collide = false;
            _wallDirection = Direction.NONE;
            _jumpGrace--;

            _wallDirection = checkTileCollision(map, tileset);

            switch (_wallDirection)
            {
                case Direction.LEFT:
                    VelocityX = 0;
                    space.X = (int) _collidingWith.X + _collidingWith.Width + 1;
                    break;
                case Direction.RIGHT:
                    VelocityX = 0;
                    space.X = (int) _collidingWith.X - space.width - 1;
                    break;
                case Direction.UP:
                    _collide = true;
                    //_collidingWith = platform; // FIXME? 
                    space.Y = (int) _collidingWith.Y + _collidingWith.Height + 1;
                    break;
                case Direction.DOWN:
                    _collide = true;
                    _hasJumped = false;
                    //_collidingWith = platform;

                    // Without this, you can climb anything
                    // With this, you can go through walls....
                    //if (space.Y-space.height >= (int) y)
                    /*
                    if (_wallDirection == Direction.NONE)
                        space.Y = (int) y - space.height + 1;
                    */
                    space.Y = (int) _collidingWith.Y - space.height + 1;
                    break;
                default:
                    break;
            }

            // Do stuff if we're colliding.
            if (_collide) 
            {
                VelocityY = 0;
                _gravityAccel = 0;
                _jumpGrace = _jumpGraceMax;
                _jumpTick = 0;
            }
            else if (VelocityY < TerminalVelocityY && _jumpTick == 0)
            {
                _gravityAccel += _gravitySnap;
                int grav = (int)((double) _gravitySnap * ((double) _gravityAccel / (double) TerminalVelocityY));
                VelocityY = grav > 0 ? grav : 2;
                if (VelocityY > TerminalVelocityY)
                    VelocityY = TerminalVelocityY;
            }

            // X velocity collision stuff
            if (VelocityX < 0 && _wallDirection != Direction.LEFT)
                space.X += VelocityX;

            if (VelocityX > 0 && _wallDirection != Direction.RIGHT)
                space.X += VelocityX;

            // Decay X Velocity
            if (VelocityX < 0)
                VelocityX++;
            else if (VelocityX > 0)
                VelocityX--;

            // Do vertical physics differently if we're jumping
            if (_jumpTick > 0)
            {
                VelocityY = -1 * (int)((double) _jumpInc * ((double) _jumpTick / (double) _jumpPower));
                _jumpTick--;
            }
            //else
            //{
            //    VelocityY = (int)((double) _gravityAccel * ((double) VelocityY / (double) TerminalVelocityY));
            //}
            space.Y += VelocityY;
        }

        public void Walk(Direction dir)
        {
            facing = dir;
            switch (dir)
            {
                case Direction.LEFT:
                    if (Math.Abs(VelocityX) < _topSpeed && _wallDirection != Direction.LEFT)
                        VelocityX -= _speed;
                    break;
                case Direction.RIGHT:
                    if (Math.Abs(VelocityX) < _topSpeed && _wallDirection != Direction.RIGHT)
                        VelocityX += _speed;
                    break;
            }
        }

        public void Jump()
        {
            if (!_hasJumped && (_collide || _jumpGrace >= 0))
            {
                _hasJumped = true;
                _collide = false;
                _jumpTick = _jumpPower;
                space.Y -= 5;
            }
        }
    }
}
