using System;
using System.Collections.Generic;
using System.Drawing;
using GameLibrary.Dirigible;
using OpenTK;
using OpenTK.Input;

namespace GameLibrary.DirigibleDecorators
{
    public abstract class DirigibleDecorator : AbstractDirigible
    {
        protected AbstractDirigible _dirigible;

        public DirigibleDecorator(AbstractDirigible dirigible)
        {
            _dirigible = dirigible;
        }

        public override int Health
        {
            get { return _dirigible.Health; }
            set { _dirigible.Health = value; }
        }
        public override int Armor
        {
            get { return _dirigible.Armor; }
            set { _dirigible.Armor = value; }
        }
        public override int Fuel
        {
            get { return _dirigible.Fuel; }
            set { _dirigible.Fuel = value; }
        }
        public override int Ammo
        {
            get { return _dirigible.Ammo; }
            set { _dirigible.Ammo = value; }
        }
        public override float Speed
        {
            get { return _dirigible.Speed; }
            set { _dirigible.Speed = value; }
        }
        public override int DirigibleID
        {
            get { return _dirigible.DirigibleID; }
            set { _dirigible.DirigibleID = value; }
        }

        public override int NumberOfPrizesReceived
        {
            get { return _dirigible.NumberOfPrizesReceived; }
            set { _dirigible.NumberOfPrizesReceived = value; }
        }

        public override bool IsTurnedLeft
        {
            get { return _dirigible.IsTurnedLeft; }
            set { _dirigible.IsTurnedLeft = value; }
        }

        public override void GetDamage(int damage)
        {
            _dirigible.GetDamage(damage);

        }
        public override void ChangeDirectionWithWind(Vector2 newWindSpeed)
        {
            _dirigible.ChangeDirectionWithWind(newWindSpeed);
        }
        public override void ChangeWindDirection(bool turnOver)
        {
            _dirigible.ChangeWindDirection(turnOver);
        }

        public override Vector2 GetGunPosition()
        {
            Vector2 gunPosition = _dirigible.GetGunPosition() + gunOffset;

            if (!IsShoot)
            {
                gunPosition.X = _dirigible.GetGunPosition().X - gunOffset.X;
            }

            return gunPosition;
        }
        public override void Idle()
        {
            _dirigible.Idle();
        }
        public override void Move(Vector2 movement)
        {
            _dirigible.Move(movement);
        }
        public override void Render()
        {
            _dirigible.Render();
        }
        public override RectangleF GetCollider()
        {
            return _dirigible.GetCollider();
        }


    }
}
