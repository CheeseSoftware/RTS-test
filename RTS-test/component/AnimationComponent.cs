using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class AnimationComponent : IComponent
    {
        private Dictionary<int, Animation> animations;
        private Animation currentAnimation;
        private int currentTexture;

        public AnimationComponent()
        {
            animations = new Dictionary<int, Animation>();
        }

        public void addAnimation(int texture, Animation animation)
        {
            animations.Add(texture, animation);
        }

        public void startAnimation(int texture)
        {
            currentAnimation = animations[texture];
            currentTexture = texture;
            currentAnimation.start();
        }

        public void stopAnimation()
        {
            currentAnimation.stop();
        }

        public bool isAnimating()
        {
            return currentAnimation != null && currentAnimation.isAnimating();
        }

        public Rectangle getCurrentFrame()
        {
            return currentAnimation.getCurrentFrame();
        }

        public int getCurrentTexture()
        {
            return currentTexture;
        }

    }
}
