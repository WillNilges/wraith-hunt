﻿using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
    public class AEWraith : AEPlayer
    {
        TimeSpan _blastAttackCooldown = new TimeSpan(0, 0, 0, 0, 500);
        TimeSpan _blastAttackTick;

        public AEWraith(
            string spritePath, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteScale, bodySize, body, playerType
        )
        {
            this._body.OnCollision -= CollisionHandler;
            this._body.OnCollision += base.PlayerCollisionHandler;
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _blastAttackTick -= gameTime.ElapsedGameTime;
        }

        public AEDamageBox Attack(World world)
        {
            if (_blastAttackTick < TimeSpan.Zero)
            {
                _blastAttackTick = _blastAttackCooldown;
                return new AEDamageBox(
                    _spritePath,
                    _spriteScale,
                    new Vector2(5.5f, 5.5f),
                    world.CreateRectangle(
                        5.5f,
                        5.5f,
                        1,
                        new Vector2(
                            _body.LinearVelocity.X > 0 ? _body.Position.X + .5f : _body.Position.X - 1.5f - .5f,
                            _body.Position.Y
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 4),
                    new TimeSpan(0, 0, 0, 0, 500),
                    true,
                    Color.Red,
                    new Vector2(_body.LinearVelocity.X > 0 ? 10 : -10, 0)
                    );

                //true, 1, new TimeSpan(0, 0, 0, 0, 500), Color.Red, playerType, new Vector2(_body.LinearVelocity.X > 0 ? 30 : -30, 0));
            }
            return null;
        }
    }
}