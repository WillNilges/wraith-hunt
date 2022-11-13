using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;


namespace WraithHunt
{
    public class Medium : Player 
    {
        private int _beamDuration = 50;
        private int _beamDamage = 3;
        private int _beamRange = 150;
        private int _beamHeight = 10;
        private bool _beamDecays = true;

        public Medium(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
        }

        // Creates a DamageBox for the Demon with a specified duration
        // and damage value.
        public DamageBox BeamAttack(Direction dir)
        {
            return new DamageBox(space.X, space.Y, dir, _beamRange, _beamHeight, Color.BlueViolet, _beamDuration, _beamDamage, _beamDecays, this);
        }
    }
}
