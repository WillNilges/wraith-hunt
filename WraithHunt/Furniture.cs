using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Devcade;

using TiledSharp;

namespace WraithHunt
{
    public class Furniture : WorldObject
    {
        protected bool _collide = false;
        protected Direction _wallDirection = Direction.NONE;
        protected bool _airborne = false;

        protected int _speed = 2; // How fast the entity is able to move on its own
        public int VelocityY = 0; // Current horizontal velocity. Positive is Down 
        public int VelocityX = 0; // Current vertical velocity. Positive is Right

        protected int _gravityAccel = 8; // Gravitational Acceleration

        public int TerminalVelocityY = 30;
        public int TerminalVelocityX = 100;

        protected Rectangle _collidingWith = new Rectangle(0,0,0,0);

        public Furniture(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
        }

        public void UpdatePhysics(List<WorldObject> platforms, TmxMap map, Texture2D tileset)
        {
            _collide = false;
            _wallDirection = Direction.NONE;

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

                        Rectangle futureMove = getHitbox();
                        if (VelocityX < 0)
                        {
                            futureMove.X -= _speed;
                            futureMove.Width += _speed;
                        } else if (VelocityX > 0)
                        {
                            futureMove.X += _speed;
                            //futureMove.Width += _speed;
                        }

                        // Left X collision
                        if (
                            layer != 2 &&
                            VelocityX < 0 &&
                            collisionRect.Intersects(futureMove) &&
                            /*Check if the player is on the same horizontal plane as the tile*/
                            space.Y > (int) y &&
                            space.Y < (int) y + tileHeight &&
                            /*Check if going further left will intersect the tile with the player*/
                            futureMove.X - _speed < (int) x + tileWidth &&
                            futureMove.X - _speed - VelocityX - 1 < (int) x + tileWidth
                        )
                        {
                            _wallDirection = Direction.LEFT;
                            VelocityX = 0;
                            space.X = (int) x + tileWidth + 1;
                            continue;
                        }

                        // Right X collision
                        if (
                            layer != 2 &&
                            VelocityX > 0 &&
                            collisionRect.Intersects(futureMove) &&
                            /*Check if the player is on the same horizontal plane as the tile*/
                            space.Y > (int) y &&
                            space.Y < (int) y + tileHeight &&
                            /*Check if going further left will intersect the tile with the player*/
                            futureMove.X + futureMove.Width + _speed > (int) x &&
                            futureMove.X + futureMove.Width + _speed + VelocityX > (int) x 
                        )
                        {
                            _wallDirection = Direction.RIGHT;
                            VelocityX = 0;
                            space.X = (int) x - space.width - 1;
                            continue;
                        }

                        // Check Y Collisions
                        if (
                            layer != 2 &&
                            x < space.X + space.width &&
                            space.X < x + tileWidth &&
                            space.Y > y + tileHeight &&
                            space.Y - VelocityY < y + tileHeight
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
                    _airborne && VelocityY < 0 &&
                    platform.space.X < space.X + space.width &&
                    space.X < platform.space.X + platform.space.width &&
                    space.Y > platform.space.Y + platform.space.height &&
                    space.Y - VelocityY < platform.space.Y + platform.space.height
                )
                {
                    _collide = true;
//                    _collidingWith = platform;
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
                    //_collidingWith = platform;
                    space.Y = platform.space.Y - space.height + 1;
                    break;
                }
            }

            if (_collide) 
            {
                VelocityY = 0;
                VelocityX = 0;
            }
            else if (VelocityY < TerminalVelocityY)
            {
                VelocityY += _gravityAccel;
            }

            space.Y += (int)((double) _gravityAccel * ((double) VelocityY / (double) TerminalVelocityY));
            space.X += VelocityX;
            if (VelocityX > 0)
                VelocityX--;
            if (VelocityX < 0)
                VelocityX++;
        }

        protected Direction checkTileCollision(TmxMap map, Texture2D tileset)
        {
            _collidingWith = new Rectangle(0,0,0,0);
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

                        // Check furniture's next position if it were to keep moving
                        Rectangle futureMove = getHitbox();
                        if (VelocityX < 0)
                        {
                            futureMove.X -= _speed;
                            futureMove.Width += _speed;
                        } else if (VelocityX > 0)
                        {
                            futureMove.X += _speed;
                            //futureMove.Width += _speed;
                        }

                        // Left X collision
                        if (
                            layer != 2 &&
                            VelocityX < 0 &&
                            collisionRect.Intersects(futureMove) &&
                            /*Check if the player is on the same horizontal plane as the tile*/
                            space.Y > (int) y &&
                            space.Y < (int) y + tileHeight &&
                            /*Check if going further left will intersect the tile with the player*/
                            futureMove.X - _speed < (int) x + tileWidth &&
                            futureMove.X - _speed - VelocityX - 1 < (int) x + tileWidth
                        )
                        {
                            /*
                            _wallDirection = Direction.LEFT;
                            VelocityX = 0;
                            space.X = (int) x + tileWidth + 1;
                            */

                            _collidingWith = collisionRect;
                            return Direction.LEFT;
                        }

                        // Right X collision
                        if (
                            layer != 2 &&
                            VelocityX > 0 &&
                            collisionRect.Intersects(futureMove) &&
                            /*Check if the player is on the same horizontal plane as the tile*/
                            space.Y > (int) y &&
                            space.Y < (int) y + tileHeight &&
                            /*Check if going further left will intersect the tile with the player*/
                            futureMove.X + futureMove.Width + _speed > (int) x &&
                            futureMove.X + futureMove.Width + _speed + VelocityX > (int) x 
                        )
                        {
                            /*
                            _wallDirection = Direction.RIGHT;
                            VelocityX = 0;
                            space.X = (int) x - space.width - 1;
                            */
                            _collidingWith = collisionRect;
                            return Direction.RIGHT;
                        }

                        // Check Y Collisions
                        // Upper collision
                        if (
                            layer != 2 &&
                            _airborne &&
                            x < space.X + space.width &&
                            space.X < x + tileWidth &&
                            space.Y > y + tileHeight &&
                            space.Y + VelocityY < y + tileHeight
                        )
                        {
                            /*
                            _collide = true;
                            //_collidingWith = platform; // FIXME? 
                            space.Y = (int)y + tileHeight + 1;
                            */
                            _collidingWith = collisionRect;
                            return Direction.UP;
                          // Lower Collision
                        } else if (
                            (int) x < space.X + space.width &&
                            space.X < (int) x + tileWidth &&
                            (int)y < space.Y + space.height &&
                            space.Y < (int) y + tileHeight
                        )
                        {
                            /*
                            _collide = true;
                            _hasJumped = false;
                            //_collidingWith = platform;

                            // Without this, you can climb anything
                            // With this, you can go through walls....
                            //if (space.Y-space.height >= (int) y)
                            if (_wallDirection == Direction.NONE)
                                space.Y = (int) y - space.height + 1;*/
                            _collidingWith = collisionRect;
                            return Direction.DOWN;
                        }
                    }
                    /*
                    if (_collide)
                        break;
                    */
                }
            }
            return Direction.NONE;
        }
    }
}
