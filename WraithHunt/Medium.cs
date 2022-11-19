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

        public int blinkCooldown = 300;
        public int blinkTick = 0;
        private int _blinkRange = 192;

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
            if (blinkTick > 0)
                blinkTick--;
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

        // Dash abilitiy, enter the ethereal plane, pop out like 4 tiles in a direction
        public DamageBox blink(Direction dir)
        {
            if (currentPlane == Plane.MATERIAL && blinkTick <= 0)
            {
                currentPlane = Plane.ETHEREAL;
                _collide = false;
                switch (dir)
                {
                    case Direction.UP:
                        space.Y -= _blinkRange;
                        break;
                    case Direction.DOWN:
                        space.Y += _blinkRange;
                        break;
                    case Direction.LEFT:
                        space.X -= _blinkRange;
                        break;
                    case Direction.RIGHT:
                        space.X += _blinkRange;
                        break;
                }
                currentPlane = Plane.MATERIAL;
                blinkTick = blinkCooldown;
                return new DamageBox(space.X+30, space.Y, Direction.LEFT, 30, _blinkRange, Color.BlueViolet, _beamDuration, 0, _beamDecays, this);
            }
            return new DamageBox(0, 0, facing, 0, 0, Color.Orange, 0, 0, true, this);
        }
    }
}
