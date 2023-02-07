using Microsoft.Xna.Framework;

namespace WraithHunt
{
    public struct DamageFrom
    {
        public AEPlayer player;
        public int damage;
        public Vector2 knockback;

        public DamageFrom(AEPlayer player, int damage, Vector2 knockback)
        {
            this.player = player;
            this.damage = damage;
            this.knockback = knockback;
        }
    }

    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        NONE
    }
    public enum WHPlane
    {
        MATERIAL,
        ETHEREAL
    }
    public enum AETag
    {
        WORLD, // For maptiles and other solid things part of the material geometry
        MEDIUM, // Is, or originated from, the Medium
        WRAITH, // Is, originated from, the Wraith
        MEDIUMATTACK, // An attack from the Medium
        WRAITHATTACK, // An attack from the Wraith
        KILLPLANE, // Touch this, you die
        NONE // For things that shouldn't collide at all, like the background.
    }
}
