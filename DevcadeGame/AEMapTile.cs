using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace WraithHunt
{
	public class AEMapTile : AEObject
	{
		Rectangle spriteParameters;
		public AEMapTile(string spritePath, Rectangle spriteParameters, float spriteOffset, float spriteScale, Vector2 bodySize, Body body) : base(spritePath, spriteOffset, spriteScale, bodySize, body)
		{
			this.spriteParameters = spriteParameters;

			this._body.OnCollision -= base.CollisionHandler;
			this._body.OnCollision += MapTileCollisionHandler;
		}


		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				_sprite,
				GetCameraCoords(),
				spriteParameters,
				Color.White
			);
		}

		public bool MapTileCollisionHandler(Fixture fixture, Fixture other, Contact contact)
		{
			return true;
		}
	}
}