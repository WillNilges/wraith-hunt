using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using TiledSharp;
using System.Collections.Generic;

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

        private List<Body> tileBodies;

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

                        Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                        spriteBatch.Draw(
                            tileset,
                            new Rectangle(
                                (int)(x * _spriteScale - (tileWidth * _spriteScale) / 2.0f),
                                (int)(y * _spriteScale - (tileHeight * _spriteScale) / 2.0f),
                                (int)(tileWidth * _spriteScale),
                                (int) (tileHeight * _spriteScale)
                            ),
                            tilesetRec,
                            Color.White
                        );
                    }
                }
            }
        }
    }
}