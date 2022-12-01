using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

namespace WraithHunt
{
    public class DamageBox : WorldObject 
    {
        public int duration {get;}
        public int damage {get;}
        private Direction _direction;
        private bool _decay;
        public int timer {get; set;}

        private Player _caster;
        private Player _hasHit;

        public DamageBox(
            int xPos,
            int yPos, 
            Direction dir,
            int dimWidth,
            int dimHeight,
            Color objColor,
            int dur,
            int dmg,
            bool damageDecay,
            Player castBy
        ) : base(
            xPos, 
            yPos,
            dimWidth,
            dimHeight,
            objColor
        )
        {
            // TODO: Find a way to center the beam effect in the middle of the player sprite
            // (something something space.Y)
            
            // Direction ENUM to control which direction the beam gets cast in
            // Magic number: 10. The player's width
            switch (dir)
            {
                case Direction.LEFT:
                    space.X -= space.width + 10; // Make it spawn a distance away from the player
                    break;
                case Direction.RIGHT:
                    space.X += 10 * 2; // Make it spawn a distance away from the player
                    break;
            }

            duration = dur;
            timer = dur;
            damage = dmg;
            _direction = dir;
            _decay = damageDecay;
            _caster = castBy;
        }

        public void checkCollision(Player player)
        {
            if (
                space.X < player.space.X + player.space.width &&
                player.space.X < space.X + space.width &&
                space.Y < player.space.Y + player.space.height &&
                player.space.Y < space.Y + space.height
            )
            {
                // No self-damage or hitting a million times
                if (player != _caster && player != _hasHit)
                {
                    // Do damage to the other player
                    // Optionally, decay the damage as the effect wears off
                    float decayed = 1.0f;
                    if (_decay)
                        decayed = ((float) timer/(float) duration);

                    player.health -= (int) ((float) damage * decayed);

                    // Knockback
                    // 2.0f is a magic number. It just halves the distances
                    // because I think it's unreasonable to be thrown back
                    // that much. This ought to be handled by the collision
                    // system.
                    player.space.Y -= (int) ((float) space.height/2.0f * decayed);
                    int dirMultiplier = 1;
                    if (_direction == Direction.LEFT)
                        dirMultiplier = -1;
                    player.VelocityX += (int) ((float) 10.0f * decayed) * dirMultiplier; // TODO: Add horizontal velocity to player

                    // Make player invulnerable to this hitbox, since they've been hit by it once.
                    _hasHit = player;
                }
            }
        }

        public void update()
        {
            // Only tick if the duration is positive
            if (timer > 0) {
                // Make the attack fade out
                color = color * ((float) timer/(float) duration);
                timer--;
            }
        }

        public void ClearHit()
        {
            _hasHit = null;
        }
    }
}
