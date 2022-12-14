using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEWraith : AEPlayer
    {
        public AEWraith(
            string spritePath, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteScale, bodySize, body, playerType
        )
        {
            this._body.OnCollision -= CollisionHandler;
            this._body.OnCollision += base.PlayerCollisionHandler;
        }
    }
}