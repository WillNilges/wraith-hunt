using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using TiledSharp;
using System.Collections.Generic;
using WraithHunt;

namespace DevcadeGame
{
    public class Map
    {

        // New Map Stuff
        private string tmxPath;
        private string tilesetPath;
        private TmxMap map;
        private Texture2D tileset;

        protected float _spriteScale;
        protected float _worldScale;

        private int tileWidth;
        private int tileHeight;
        private int tilesetTilesWide;
        private int tilesetTilesHigh;

        private List<AEObject> tileBodies;

        public Map(string tmxPath, string tilesetPath, float spriteScale)
        {
            // map = new TmxMap("Content/apartment_block.tmx");
            // tileset = Content.Load<Texture2D>("chom_map_2");
            this.tmxPath = tmxPath;
            this.tilesetPath = tilesetPath;
            this._spriteScale = 10f; // Oops lol I gotta manually hardcode this. Tiled moment!
            this._worldScale = 10f; // Oops lol I gotta manually hardcode this. Tiled moment!
        }

        /**** MONOGAME PLUMBING ****/

        public void LoadContent(ContentManager contentManager, World world)
        {
            this.map = new TmxMap(tmxPath);
            this.tileset = contentManager.Load<Texture2D>(tilesetPath);

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;

            tileBodies = new List<AEObject>();

            // Map tile locations in the tile map to space in the world.
            for (var layer = 1; layer < map.Layers.Count; layer++)
            {
                for (var i = 0; i < map.Layers[layer].Tiles.Count; i++)
                {
                    int gid = map.Layers[layer].Tiles[i].Gid;

                    // Empty tile, do nothing
                    if (gid == 0)
                    {

                    }
                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        // Load in the Aether Body to do collision
                        AEObject block = new AEObject(
                           "ground_placeholder",
                           _worldScale,
                           _worldScale,
                           new Vector2(tileWidth / _worldScale, tileHeight / _worldScale),
                           world.CreateRectangle(
                               tileWidth / _worldScale,
                               tileHeight / _worldScale,
                               1,
                               new Vector2(x / _worldScale, y / _worldScale),
                               0,
                               BodyType.Static
                           )
                        );
                        block.LoadContent(contentManager);
                        tileBodies.Add(block);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (var layer = 0; layer < map.Layers.Count; layer++)
            {
                for (var i = 0; i < map.Layers[layer].Tiles.Count; i++)
                {
                    int gid = map.Layers[layer].Tiles[i].Gid;
                    // Empty tile, do nothing
                    if (gid == 0)
                    {

                    }
                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        Rectangle tilesetRec =
                            new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                        spriteBatch.Draw(
                            tileset,
                            new Rectangle(
                                (int)(x * _spriteScale - (tileWidth * _spriteScale) / 2.0f),
                                (int)(y * _spriteScale - (tileHeight * _spriteScale) / 2.0f),
                                (int)(tileWidth * _spriteScale),
                                (int)(tileHeight * _spriteScale)
                            ),
                            tilesetRec,
                            Color.White
                        );
                    }
                }
            }

            // This is an interesting idea... Instead of having Bodies, have AEObjects and somehow use the
            // map data to get sprites, and build these instead of just straight-up bodies.

            // Regardless, I think the above WILL NOT work. We'll need to use the positional data from the bodies in order to
            // correctly draw. Chicken-and-egg scenario here.
            /*foreach (AEObject AEObj in tileBodies)
                AEObj.Draw(gameTime, spriteBatch);*/
        }
    }
}
