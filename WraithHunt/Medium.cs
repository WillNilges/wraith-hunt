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
                    return new DamageBox(space.X-200, space.Y, 200, 50, Color.BlueViolet, _beamDuration, _beamDamage, this);
                    break;
                case Direction.RIGHT:
                    return new DamageBox(space.X+space.width, space.Y, 200, 50, Color.BlueViolet, _beamDuration, _beamDamage, this);
                    break;
            }
            return null;
        }
    }
}
