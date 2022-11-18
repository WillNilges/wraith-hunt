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
        protected Direction _wallDirection = Direction.NONE;

        private int _speed = 2;

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

        public void UpdatePhysics(List<WorldObject> platforms, TmxMap map, Texture2D tileset)
        {

            // If you die, reset position to the top of screen and fall back into the world (for now)
            /*if (health <= 0)
            {
                space.X = 150;
                space.Y = 0;
                health = healthMax;
            }*/

            _collide = false;
            _wallDirection = Direction.NONE;
            _jumpGrace--;

            int tileWidth;
            int tileHeight;
            int tilesetTilesWide;
            int tilesetTilesHigh;


            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;

            // Check if we're colliding with the foreground
            for (var layer = 1; layer < 3; layer++)
            {
                for (var i = 0; i < map.Layers[layer].Tiles.Count; i++)
                {
                    int gid = map.Layers[layer].Tiles[i].Gid;
                    if (gid != 0)
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        Rectangle collisionRect = new Rectangle(
                            (int)x,
                            (int)y,
                            tileWidth,
                            tileHeight 
                        );

                        // Check X Collisions
                        //if (
                        //    layer != 2 &&
                        //    /*Check and make sure we're correct in one dimension*/
                        //    space.Y + space.height > (int) y + 1 &&
                        //    space.Y < (int) y + tileHeight - 1 &&
                        //    /*Now check directional collision*/
                        //    space.X + space.width > (int) x - tileWidth &&
                        //    space.X + space.width - _velocityX < (int) x - tileWidth
                        //)
                        //{
                        //    _collide = true;
                        //    _wallDirection = Direction.RIGHT;
                        //    _velocityX = 0;
                        //    space.X = (int) x - tileWidth;
                        //    Console.WriteLine("Colliding!");
                        //    continue;
                        //}

                        Rectangle futureMove = getHitbox();
                        if (_velocityX < 0)
                        {
                            futureMove.X -= _speed;
                            futureMove.Width += _speed;
                        } else if (_velocityX > 0)
                        {
                            futureMove.X += _speed;
                            //futureMove.Width += _speed;
                        }

                        // Left X collision
                        if (
                            layer != 2 &&
                            _velocityX < 0 &&
                            collisionRect.Intersects(futureMove) &&
                            /*Check if the player is on the same horizontal plane as the tile*/
                            space.Y > (int) y &&
                            space.Y < (int) y + tileHeight &&
                            /*Check if going further left will intersect the tile with the player*/
                            futureMove.X - _speed < (int) x + tileWidth &&
                            futureMove.X - _speed - _velocityX - 1 < (int) x + tileWidth
                        )
                        {
                            _wallDirection = Direction.LEFT;
                            _velocityX = 0;
                            space.X = (int) x + tileWidth + 1;
                            Console.WriteLine("Colliding!");
                            continue;
                        }

                        // Right X collision
                        if (
                            layer != 2 &&
                            _velocityX > 0 &&
                            collisionRect.Intersects(futureMove) &&
                            /*Check if the player is on the same horizontal plane as the tile*/
                            space.Y > (int) y &&
                            space.Y < (int) y + tileHeight &&
                            /*Check if going further left will intersect the tile with the player*/
                            futureMove.X + futureMove.Width + _speed > (int) x &&
                            futureMove.X + futureMove.Width + _speed + _velocityX > (int) x 
                        )
                        {
                            _wallDirection = Direction.RIGHT;
                            _velocityX = 0;
                            space.X = (int) x - space.width - 1;
                            Console.WriteLine("Colliding Right!");
                            continue;
                        }

                        // Check Y Collisions
                        if (
                            layer != 2 &&
                            _hasJumped && _jumpTick > 0 &&
                            x < space.X + space.width &&
                            space.X < x + tileWidth &&
                            space.Y > y + tileHeight &&
                            space.Y - _jumpInc < y + tileHeight
                        )
                        {
                            _collide = true;
                            //_collidingWith = platform; // FIXME? 
                            space.Y = (int)y + tileHeight + 1;
                        } else if (
                            (int) x < space.X + space.width &&
                            space.X < (int) x + tileWidth &&
                            (int)y < space.Y + space.height &&
                            space.Y < (int) y + tileHeight
                        )
                        {
                            _collide = true;
                            _hasJumped = false;
                            //_collidingWith = platform;

                            // Without this, you can climb anything
                            // With this, you can go through walls....
                            //if (space.Y-space.height >= (int) y)
                            if (_wallDirection == Direction.NONE)
                                space.Y = (int) y - space.height + 1;
                        }
                    }
                    if (_collide)
                        break;
                }
            }

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

            // Do stuff if we're colliding.
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

            // X velocity collision stuff
            if (_velocityX < 0 && _wallDirection != Direction.LEFT)
                space.X += _velocityX;

            if (_velocityX > 0 && _wallDirection != Direction.RIGHT)
                space.X += _velocityX;

            // Decay X Velocity
            if (_velocityX < 0)
                _velocityX++;
            else if (_velocityX > 0)
                _velocityX--;

            // Do vertical physics differently if we're jumping
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
                    if (Math.Abs(_velocityX) < _topSpeed && _wallDirection != Direction.LEFT)
                        _velocityX -= _speed;
                    break;
                case Direction.RIGHT:
                    if (Math.Abs(_velocityX) < _topSpeed && _wallDirection != Direction.RIGHT)
                        _velocityX += _speed;
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
