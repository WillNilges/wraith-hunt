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
                    player.health -= damage;

                    // Knockback
                    player.space.Y -= space.height/2;
                    player.space.X -= space.width/2; // TODO: Add horizontal velocity to player

                    // Make player invulnerable to this hitbox, since they've been hit by it once.
                    _hasHit = player;
                }
            }
        }

        public void update()
        {
            timer--;
        }
    }
}
