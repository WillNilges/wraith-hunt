using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WraithHunt
{
    public class Npc : AEPlayer
    {
        public Npc (
            string spritePath, float spriteOffset, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteOffset, spriteScale, bodySize, body, playerType
        )
        {
        }

        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Walk(Direction.LEFT);
        }
    }
}
