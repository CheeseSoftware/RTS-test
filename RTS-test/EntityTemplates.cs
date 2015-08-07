using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    [ArtemisEntityTemplate("Test")]
    public class TestTemplate : IEntityTemplate {
	    public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args) {
            TextureManager textureManager = EntitySystem.BlackBoard.GetEntry<TextureManager>("TextureManager");

            Vector2 pos = new Vector2(0f, 0f);
            Vector2 velocity = new Vector2(0f, 0f);

            if (args.Length >= 1)
                pos = (Vector2)args[0];

            if (args.Length >= 2)
                velocity = (Vector2)args[1];

		    entity.AddComponent(new component.Position(pos));
			entity.AddComponent(new component.Velocity(new Vector2(velocity.X, velocity.Y)));
			entity.AddComponent(new component.Thrust());
			entity.AddComponent(new component.Drawable(textureManager.getTexture(2)));
            return entity;
	    }

    }

	[ArtemisEntityTemplate("Tree")]
	public class TreeTemplate : IEntityTemplate
	{
		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			TextureManager textureManager = EntitySystem.BlackBoard.GetEntry<TextureManager>("TextureManager");

			Vector2 pos = new Vector2(0f, 0f);

			if (args.Length >= 1)
				pos = (Vector2)args[0];

			Texture2D texture = textureManager.getTexture(5);

			entity.AddComponent(new component.Position(pos));
			entity.AddComponent(new component.Size(texture.Width, texture.Height));
			entity.AddComponent(new component.Drawable(texture));
			entity.AddComponent(new component.DepletableResource(500, "Tree"));
			return entity;
		}

	}

	[ArtemisEntityTemplate("Stone")]
	public class StoneTemplate : IEntityTemplate
	{
		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			TextureManager textureManager = EntitySystem.BlackBoard.GetEntry<TextureManager>("TextureManager");

			Vector2 pos = new Vector2(0f, 0f);

			if (args.Length >= 1)
				pos = (Vector2)args[0];

			Texture2D texture = textureManager.getTexture(8);

			entity.AddComponent(new component.Position(pos));
			entity.AddComponent(new component.Size(texture.Width, texture.Height));
			entity.AddComponent(new component.Drawable(texture));
			entity.AddComponent(new component.DepletableResource(500, "Stone"));
			return entity;
		}

	}

	[ArtemisEntityTemplate("BerryBush")]
	public class BerryBushTemplate : IEntityTemplate
	{
		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			TextureManager textureManager = EntitySystem.BlackBoard.GetEntry<TextureManager>("TextureManager");

			Vector2 pos = new Vector2(0f, 0f);

			if (args.Length >= 1)
				pos = (Vector2)args[0];

			Texture2D texture = textureManager.getTexture(6);

			entity.AddComponent(new component.Position(pos));
			entity.AddComponent(new component.Size(texture.Width, texture.Height));
			entity.AddComponent(new component.Drawable(texture));
			entity.AddComponent(new component.DepletableResource(500, "BerryBush"));
			return entity;
		}

	}
}
