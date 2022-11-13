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
        private bool _decay;
        public int timer {get; set;}

        private Player _caster;
        private Player _hasHit;

        public DamageBox(
            int xPos,
            int yPos, 
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
            duration = dur;
            timer = dur;
            damage = dmg;
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
                    if (_decay)
                    {
                        player.health -= (int) ((float) damage * ((float) timer/(float) duration));

                        // Knockback
                        // 2.0f is a magic number. It just halves the distances
                        // because I think it's unreasonable to be thrown back
                        // that much. This ought to be handled by the collision
                        // system.
                        player.space.Y -= (int) ((float) space.height/2.0f * ((float) timer/ (float) duration));
                        player.space.X -= (int) ((float) space.width/2.0f * ((float) timer/ (float) duration)); // TODO: Add horizontal velocity to player
                    }

                    // Make player invulnerable to this hitbox, since they've been hit by it once.
                    _hasHit = player;
                }
            }
        }

        public void update()
        {
            // Make the attack fade out
            color = color * ((float) timer/(float) duration);
            timer--;
        }
    }
}
