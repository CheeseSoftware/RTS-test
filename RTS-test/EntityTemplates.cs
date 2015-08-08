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
            FarseerPhysics.Dynamics.World world = EntitySystem.BlackBoard.GetEntry<FarseerPhysics.Dynamics.World>("PhysicsWorld");

            Vector2 pos = new Vector2(0f, 0f);
            Vector2 velocity = new Vector2(0f, 0f);

            if (args.Length >= 1)
                pos = (Vector2)args[0];

            if (args.Length >= 2)
                velocity = (Vector2)args[1];



            FarseerPhysics.Dynamics.Body body = FarseerPhysics.Factories.BodyFactory.CreateEllipse(world, 1.0f, 0.5f, 4, 1.0f);
            body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            body.Position = pos;
			body.Friction = 0.0f;
			body.Restitution = 0.0f;
			body.Mass = 0.0f;
			body.FixedRotation = true;
            body.LinearVelocity = velocity;


            entity.AddComponent(new component.Physics(body));
            entity.AddComponent(new component.MaxVelocity(0.95f));
            entity.AddComponent(new component.Drawable(textureManager.getTexture(9)));
            entity.AddComponent(new component.Goal());
            return entity;
	    }

    }

	[ArtemisEntityTemplate("Resource")]
	public class ResourceTemplate : IEntityTemplate
	{
		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			TextureManager textureManager = EntitySystem.BlackBoard.GetEntry<TextureManager>("TextureManager");
			FarseerPhysics.Dynamics.World world = EntitySystem.BlackBoard.GetEntry<FarseerPhysics.Dynamics.World>("PhysicsWorld");

			Vector2 pos = new Vector2(0f, 0f);
			float rotation = 0.0f;
			int resourceAmount = 500;
			String resourceType = "No type";
			int textureId = 0;

			Random r = new Random();

			if (args.Length >= 1)
				pos = (Vector2)args[0];

			if (args.Length >= 2)
				rotation = (float)args[1];

			if (args.Length >= 3)
				resourceType = (String)args[2];

			if (args.Length >= 4)
				resourceAmount = (int)args[3];

			if (args.Length >= 5)
				textureId = (int)args[4];

			Texture2D texture = textureManager.getTexture(textureId);

			FarseerPhysics.Dynamics.Body body = FarseerPhysics.Factories.BodyFactory.CreateRectangle(world, 2f, 2f, 1.0f);
			body.BodyType = FarseerPhysics.Dynamics.BodyType.Static;
			body.Position = pos;
			body.Rotation = rotation;
			body.FixedRotation = false;
			body.CollidesWith = FarseerPhysics.Dynamics.Category.None;

			//entity.AddComponent(new component.Physics(body));
			entity.AddComponent(new component.Size(texture.Width, texture.Height));
			entity.AddComponent(new component.Drawable(texture));
			entity.AddComponent(new component.DepletableResource(resourceAmount, resourceType));
			return entity;
		}

	}
}
