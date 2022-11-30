using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TiledSharp;

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
        private int _blastCooldown = 20;
        private int _blastTick = 0;

        public int planeSwitchCooldown = 200;
        public int planeSwitchTick = 0;

        // Telekinesis
        public int TKCooldown = 80;
        public int TKTick = 0;
        private int _TKRange = 200;
        private Furniture _TKCandidate;

        public Demon(int xPos, int yPos, int dimWidth, int dimHeight, Color objColor) : base(xPos, yPos, dimWidth,  dimHeight, objColor)
        {
            currentPlane = Plane.ETHEREAL;
        }

        public void demonReset(Vector2 pos)
        {
            this.reset(pos);
            _blastTick = 0;
            planeSwitchTick = 0;
            TKTick = 0;
            currentPlane = Plane.ETHEREAL;
        }

        public void UpdatePhysics(List<WorldObject> platforms, TmxMap map, Texture2D tileset, List<Furniture> furniture)
        {
            base.UpdatePhysics(platforms, map, tileset);
            abilitiesTick();
            TKSearch(furniture);
        }

        public void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            if (_TKCandidate != null)
            {
                RectangleSprite.DrawRectangle(
                        batch,
                        new Rectangle(
                            _TKCandidate.space.X,
                            _TKCandidate.space.Y,
                            _TKCandidate.space.width,
                            _TKCandidate.space.height
                            ),
                        Color.Yellow,
                        3
                );
            }
        }

        public void abilitiesTick()
        {
            if (_blastTick > 0)
                _blastTick--;
            if (planeSwitchTick > 0)
                planeSwitchTick--;
            if (TKTick > 0)
                TKTick--;
        }

        public void TKSearch(List<Furniture> furniture)
        {
            _TKCandidate = furniture[0];
            foreach (Furniture furn in furniture)
            {
                // How far away this furn is
                int furnDistX = Math.Abs(furn.space.X - space.X);
                int furnDistY = Math.Abs(furn.space.Y - space.Y);
                int furnPythag = (int) Math.Sqrt(furnDistX*furnDistX + furnDistY*furnDistY);

                // How far away the closest furn is
                int closestDistX = Math.Abs(_TKCandidate.space.X - space.X);
                int closestDistY = Math.Abs(_TKCandidate.space.Y - space.Y);
                int closestPythag = (int) Math.Sqrt(closestDistX*closestDistX + closestDistY*closestDistY);

                if (
                    furnPythag < _TKRange &&
                    furnPythag < closestPythag
                )
                {
                    _TKCandidate = furn;
                }
            }
        }

        public DamageBox TKBlast()
        {
            if (_TKCandidate != null && TKTick <= 0)
            {
                _TKCandidate.space.X -= 20;
                _TKCandidate.space.Y -= 20;
                _TKCandidate._velocityX -= 10;
                _TKCandidate._velocityY -= 100;
                TKTick = TKCooldown;
                return new DamageBox(_TKCandidate.space.X, _TKCandidate.space.Y, Direction.LEFT, _blastRange, _blastHeight, Color.Orange, _blastDuration, 0, _blastDecays, this);
            }
            return new DamageBox(0, 0, facing, 0, 0, Color.Orange, 0, 0, true, this);
        }

        public void SwitchPlanes()
        {
            if (planeSwitchTick <= 0)
            {
                if (currentPlane == Plane.MATERIAL)
                {
                    currentPlane = Plane.ETHEREAL;
                }
                else if (currentPlane == Plane.ETHEREAL)
                {
                    currentPlane = Plane.MATERIAL;
                }
                planeSwitchTick = planeSwitchCooldown;
            }
        }

        // Creates a DamageBox for the Demon with a specified duration
        // and damage value.
        public DamageBox BlastAttack()
        {
            if (_blastTick <= 0)
            {
                _blastTick = _blastCooldown;
                return new DamageBox(space.X, space.Y-(_blastRange/2), facing, _blastRange, _blastHeight, Color.Orange, _blastDuration, _blastDamage, _blastDecays, this);
            }
            return new DamageBox(0, 0, facing, 0, 0, Color.Orange, 0, 0, true, this);
        }
    }
}
