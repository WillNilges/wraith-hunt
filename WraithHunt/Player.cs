using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

namespace WraithHunt
{
    public enum Direction
    {
        LEFT,
        RIGHT
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

        private int _speed = 4;

        public int healthMax = 10;
        public int health = 10;

        public Plane currentPlane {set; get;}

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

        public void Draw(SpriteBatch batch)
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
            _jumpGrace--;

            // Check if we're colliding with a world object.
            foreach(WorldObject platform in platforms)
            {
                if (
                    _hasJumped && _jumpTick > 0 &&
                    platform.space.X < space.X + space.width &&
                    space.X < platform.space.X + platform.space.width &&
                    space.Y > platform.space.Y + platform.space.height &&
                    space.Y - _jumpInc < platform.space.Y + platform.space.height
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
                    _hasJumped = false;
                    _collidingWith = platform;
                    space.Y = platform.space.Y - space.height + 1;
                    break;
                }
            }

            if (_collide) 
            {
                _currentGravity = 0;
                _jumpGrace = _jumpGraceMax;
                _jumpTick = 0;
            }
            else if (_currentGravity < _gravityMax && _jumpTick == 0)
            {
                _currentGravity += _gravityAccel;
            }

            if (_jumpTick > 0)
            {
                space.Y -= (int)((double) _jumpInc * ((double) _jumpTick / (double) _jumpPower));
                _jumpTick--;
            }
            else
            {
                space.Y += (int)((double) _gravityInc * ((double) _currentGravity / (double) _gravityMax));
            }
        }

        public void Walk(Direction dir)
        {
            facing = dir;
            switch (dir)
            {
                case Direction.LEFT:
                    space.X -= _speed;
                    break;
                case Direction.RIGHT:
                    space.X += _speed;
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
