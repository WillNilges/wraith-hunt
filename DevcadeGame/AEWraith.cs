using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEWraith : AEPlayer
    {
        public AEWraith(
            string spritePath, float spriteScale, Vector2 bodySize, Body body, AEObjectType playerType
        ) : base(
            spritePath, spriteScale, bodySize, body, playerType
        )
        {
            this._body.OnCollision -= base.CollisionHandler;
            this._body.OnCollision += CollisionHandler;
        }

        bool CollisionHandler(Fixture fixture, Fixture other, Contact contact)
        {
            Colliding = true;
            _hasJumped = false;

            if (other.Tag != null && (int)other.Tag > 0)
            {
                health -= (int)other.Tag;
                other.Tag = 0;
            }

            return true;
        }


    }
}