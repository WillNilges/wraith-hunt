using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WraithHunt
{
    public enum MillStage
    {
        WAITINGLEFT,
        MOVINGLEFT,
        WAITINGRIGHT,
        MOVINGRIGHT
    }
    public class Npc : AEPlayer
    {
        private bool _possessed = false;
        private TimeSpan _millDuration = new TimeSpan(0, 0, 0, 8, 0);
        private MillStage _millStage = MillStage.WAITINGLEFT;
        private TimeSpan _millTick = TimeSpan.Zero;
        private Direction _millDirection = Direction.LEFT;
        private float _millSpeed = 0.3f;

        TimeSpan _blastAttackCooldown = new TimeSpan(0, 0, 5);
        TimeSpan _blastAttackTick = TimeSpan.Zero;

        Random random;

        public Npc(
            string spritePath, float spriteOffset, float spriteScale, Vector2 bodySize, Body body, AETag playerType
        ) : base(
            spritePath, spriteOffset, spriteScale, bodySize, body, playerType
        )
        {
            setTag(AETag.NONE); // Oops no collision. 
            random = new Random();
        }

        public new void Update(GameTime gameTime, World world)
        {
            base.Update(gameTime, world);
            if (!_possessed)
            {
                _millTick -= gameTime.ElapsedGameTime;
                if (_millTick < TimeSpan.Zero)
                {
                    _millTick = new TimeSpan(0, 0, 0, 0, (int)((float)_millDuration.TotalMilliseconds * ((float)random.Next() / (float)Int32.MaxValue)));
                    _millStage = (MillStage)(((int)_millStage + 1) % System.Enum.GetNames(typeof(MillStage)).Length);
                }
                switch (_millStage)
                {
                    case MillStage.WAITINGLEFT:
                        Walk(Direction.NONE, _millSpeed);
                        break;
                    case MillStage.MOVINGLEFT:
                        Walk(Direction.LEFT, _millSpeed);
                        break;
                    case MillStage.WAITINGRIGHT:
                        Walk(Direction.NONE, _millSpeed);
                        break;
                    case MillStage.MOVINGRIGHT:
                        Walk(Direction.RIGHT, _millSpeed);
                        break;
                    default:
                        break;
                }

                if (_millTick.TotalMilliseconds <= 500 && random.Next() > (int)((double)Int32.MaxValue * 0.7))
                {
                    Jump();
                }
            } else {
                _blastAttackTick -= gameTime.ElapsedGameTime;
            }
        }

        public AEDamageBox Attack(World world)
        {
            if (_blastAttackTick < TimeSpan.Zero)
            {
                _blastAttackTick = _blastAttackCooldown;
                Vector2 attackSize = new Vector2(1f, 1f);
                return new AEDamageBox(
                    _spritePath,
                    _spriteOffset,
                    _spriteOffset,
                    attackSize,
                    world.CreateRectangle(
                        attackSize.X,
                        attackSize.Y,
                        1,
                        new Vector2(
                            _body.Position.X + (facing == Direction.RIGHT ? attackSize.X / 2 + .5f : -1 * (attackSize.Y / 2 + .5f)),
                            _body.Position.Y - attackSize.Y / 4
                        ),
                        0,
                        BodyType.Dynamic
                    ),
                    new DamageFrom(this, 1, new Vector2(5, -5)),
                    new TimeSpan(0, 0, 0, 0, 500),
                    true,
                    Color.Red,
                    new Vector2(facing == Direction.RIGHT ? 10 : -10, 0)
                    );

                //true, 1, new TimeSpan(0, 0, 0, 0, 500), Color.Red, playerType, new Vector2(_body.LinearVelocity.X > 0 ? 30 : -30, 0));
            }
            return null;
        }

        public void TogglePossessed()
        {
            _possessed = !_possessed;
        }
    }
}
