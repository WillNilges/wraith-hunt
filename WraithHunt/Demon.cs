using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;


namespace WraithHunt
{
    public class Demon : Player 
    {
        private int _blastDuration = 50;
        private int _blastDamage = 5;
        private int _blastRange = 50;
        private int _blastHeight = 50;
        private bool _blastDecays = true;

        public Demon(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
        }

        // Creates a DamageBox for the Demon with a specified duration
        // and damage value.
        public DamageBox BlastAttack()
        {
            return new DamageBox(space.X, space.Y-(_blastRange/2), facing, _blastRange, _blastHeight, Color.Orange, _blastDuration, _blastDamage, _blastDecays, this);
        }
    }
}
