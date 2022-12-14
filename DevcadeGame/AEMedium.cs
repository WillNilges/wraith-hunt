using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Security.Cryptography;
using tainicom.Aether.Physics2D.Dynamics;
using static System.Net.Mime.MediaTypeNames;
using Transform;
using Color = Microsoft.Xna.Framework.Color;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEMedium : AEPlayer
    {
        TimeSpan _beamAttackCooldown = new TimeSpan(0,0,0,0,500);
        TimeSpan _beamAttackTick;

        public AEMedium(
            string spritePath, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base (
            spritePath, spriteScale, bodySize, body, playerType
        )
        {
            this._body.OnCollision -= CollisionHandler;
            this._body.OnCollision += base.PlayerCollisionHandler;
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _beamAttackTick -= gameTime.ElapsedGameTime;
        }

        public AEDamageBox Attack(World world)
        {
            if (_beamAttackTick < TimeSpan.Zero)
            {
                _beamAttackTick = _beamAttackCooldown;
                Vector2 spriteSize = new Vector2(15f, .5f);
                return new AEDamageBox(
                    _spritePath,
                    _spriteScale,
                    spriteSize,
                    world.CreateRectangle(
                        15f,
                        .5f,
                        1,
                        new Vector2(
                            _body.LinearVelocity.X > 0 ? _body.Position.X + spriteSize.X/2 + .5f : _body.Position.X - spriteSize.X/2 - .5f,
                            _body.Position.Y
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 2, new Vector2(10, -10)),
                    new TimeSpan(0,0,0,1,500),
                    true,
                    new Color(198, 107, 255),
                    new Vector2(_body.LinearVelocity.X > 0 ? 100 : -100, 0)
                    ); 


                    //true, 1, new TimeSpan(0, 0, 0, 0, 500), Color.Red, playerType, new Vector2(_body.LinearVelocity.X > 0 ? 30 : -30, 0));
            }
            return null;
        }
    }
}