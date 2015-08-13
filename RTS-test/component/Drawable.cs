using Artemis.Interface;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class Drawable : IComponent
    {
        public Texture2D texture;
        private float additionalRotation;

        public Drawable(Texture2D texture, float additionalRotation = 0f)
        {
            this.texture = texture;
            this.additionalRotation = additionalRotation;
        }

        public float AdditionalRotation { get { return this.additionalRotation; } }
    }
}
