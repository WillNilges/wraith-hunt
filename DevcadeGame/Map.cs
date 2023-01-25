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

        protected float _spriteOffset;
        protected float _spriteScale;

        private int tileWidth;
        private int tileHeight;
        private int tilesetTilesWide;
        private int tilesetTilesHigh;

        private List<AEObject> tileBodies;

        public Map(string tmxPath, string tilesetPath, float spriteScale, float spriteOffset)
        {
            this.tmxPath = tmxPath;
            this.tilesetPath = tilesetPath;
            this._spriteScale = spriteScale;
            this._spriteOffset = spriteOffset;
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

                        // Load in the Aether Body to do collision

                        // Convert from screen coords to world coords
                        Vector4 worldDimensions = new Vector4(
                                (float)((x / _spriteOffset) + ((float) tileWidth / _spriteOffset) * 2.0f),
                                (float)((y / _spriteOffset) + ((float) tileHeight / _spriteOffset) * 2.0f),
                                (float)((float) tileWidth / _spriteScale),
                                (float)((float) tileHeight / _spriteScale)
                            );

                        float mapScale = 0.125f;

                        Vector2 plat1BodySize = new Vector2(worldDimensions.Z, worldDimensions.W) * mapScale;
                        Vector2 plat1BodyPosition = new Vector2(worldDimensions.X, worldDimensions.Y) * mapScale;

                        AEObject block = new AEObject(
                           "medium_placeholder",
                           _spriteOffset,
                           _spriteScale,
                           plat1BodySize,
                           world.CreateRectangle(
                               plat1BodySize.X, plat1BodySize.Y, 1, plat1BodyPosition, 0, BodyType.Static //TODO: Step through this in debug mode.
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
            // This is an interesting idea... Instead of having Bodies, have AEObjects and somehow use the
            // map data to get sprites, and build these instead of just straight-up bodies.

            // Regardless, I think the above WILL NOT work. We'll need to use the positional data from the bodies in order to
            // correctly draw. Chicken-and-egg scenario here.
            foreach (AEObject AEObj in tileBodies)
                AEObj.Draw(gameTime, spriteBatch);
        }
    }
}
