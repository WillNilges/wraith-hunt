using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;


namespace WraithHunt
{
    public class Medium : Player 
    {
        private int _beamDuration = 50;
        private int _beamDamage = 10;
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
            switch (dir)
            {
                case Direction.LEFT:
                    // TODO: Find a way to center the beam effect in the middle of the player sprite
                    // (something something space.Y)
                    return new DamageBox(space.X-_beamRange-space.width, space.Y, _beamRange, _beamHeight, Color.BlueViolet, _beamDuration, _beamDamage, _beamDecays, this);
                    break;
                case Direction.RIGHT:
                    return new DamageBox(space.X+space.width*2, space.Y, _beamRange, _beamHeight, Color.BlueViolet, _beamDuration, _beamDamage, _beamDecays, this);
                    break;
            }
            return null;
        }
    }
}
