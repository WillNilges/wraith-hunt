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
            this._spriteScale = spriteScale;
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

                        Console.WriteLine($"Loading map body at {x / _spriteScale}, {y / _spriteScale}");

                        tileBodies.Add(
                            new AEObject(
                               "ground_placeholder",
                               _spriteScale,
                               new Vector2(tileWidth / _spriteScale, tileHeight / _spriteScale),
                               world.CreateRectangle(
                                   tileWidth / _spriteScale,
                                   tileHeight / _spriteScale, 
                                   1,
                                   new Vector2(x / _spriteScale, y / _spriteScale),
                                   0,
                                   BodyType.Static
                               )
                            )
                        );
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*
            foreach (AEObject block in tileBodies)
            {
                block.Draw(gameTime, spriteBatch);
            }*/
            
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

                        Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                        spriteBatch.Draw(
                            tileset,
                            new Rectangle(
                                (int)(x / _spriteScale),
                                (int)(y / _spriteScale),
                                (int)(tileWidth / _spriteScale),
                                (int) (tileHeight / _spriteScale)
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
