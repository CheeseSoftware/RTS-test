using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
	class TextureManager
	{
		private Texture2D[] textures;
		private int nextSlot = 0;

		private ContentManager content; //USCH

		public TextureManager()
		{
			textures = new Texture2D[256];
        }

		private void insertTexture(String fileName)
		{
			Texture2D texture = content.Load<Texture2D>(fileName);
			textures[nextSlot] = texture;
			nextSlot++;
		}

		public void loadTextures(ContentManager content)
		{
			this.content = content;

			insertTexture("border");
			insertTexture("grass");
			insertTexture("player");
			insertTexture("sand");
			insertTexture("water");
		}

		public Texture2D getTexture(int index)
		{
			return textures[index];
		}
	}
}
