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
        private int _beamRange = 300;
        private int _beamHeight = 10;
        private bool _beamDecays = true;
        private int _beamCooldown = 40;
        private int _beamTick = 0;

        public Medium(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
        }

        public void mediumReset(Vector2 pos)
        {
            this.reset(pos);
            _beamTick = 0;
        }

        public void abilitiesTick()
        {
            if (_beamTick > 0)
                _beamTick--;
        }

        // Creates a DamageBox for the Demon with a specified duration
        // and damage value.
        public DamageBox BeamAttack()
        {
            if (_beamTick <= 0)
            {
                _beamTick = _beamCooldown;
                return new DamageBox(space.X, space.Y, facing, _beamRange, _beamHeight, Color.BlueViolet, _beamDuration, _beamDamage, _beamDecays, this);
            }
            return new DamageBox(0, 0, facing, 0, 0, Color.Orange, 0, 0, true, this);
        }
    }
}
