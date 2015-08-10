using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class AnimationManager
    {
        private Dictionary<string, Animation> animations;

        public AnimationManager()
        {
            animations = new Dictionary<string, Animation>();
        }

        public void load()
        {
            registerAnimation("smallworker-gather", new Animation(0, 5, 30, createFrames(120, 41, 6)));
        }

        public void registerAnimation(string name, Animation animation)
        {
            animations.Add(name, animation);
        }

        public Animation getAnimation(string name)
        {
            return (Animation)animations[name].Clone();
        }

        private Rectangle[] createFrames(int width, int height, int frames)
        {
            Rectangle[] rectangles = new Rectangle[frames];
            int frame = 0;
            for (int x = 0; x < width; x += width / frames)
            {
                rectangles[frame] = new Rectangle(x, 0, width / frames, height);
                frame++;
            }
            return rectangles;
        }
    }
}
