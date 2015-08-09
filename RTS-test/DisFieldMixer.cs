using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class DisFieldMixer
    {
        private List<DisField> disFields = new List<DisField>();

        public DisFieldMixer()
        {
        }

        public void addDisField(DisField disField)
        {
            disFields.Add(disField);
        }

        public bool getTile(int x, int y)
        {
            bool isSolid = false;
            foreach (DisField disField in disFields)
            {
                if (disField != null)
                    isSolid |= disField.getTile(x, y);
            }
            return isSolid;
        }

        public float getDis(Vector2 pos)
        {
            float dis = float.MaxValue;
            foreach(DisField disField in disFields)
            {
                if (disField != null)
                    dis = Math.Min(dis, disField.getDis(pos));
            }
            return dis;
        }

        public Vector2 getNormal(Vector2 pos)
        {
            Vector2 v1 = pos + new Vector2(-0.1f, -0.1f);
            Vector2 v2 = Vector2.Normalize(new Vector2(-0.1f, -0.1f));
            float f1 = getDis(pos + new Vector2(-0.1f, -0.1f));
            Vector2 f2 = f1 * v2;

            Vector2 a = getDis(pos + new Vector2(-0.1f, -0.1f)) * Vector2.Normalize(new Vector2(-0.1f, -0.1f));
            Vector2 b = getDis(pos + new Vector2(+0.1f, -0.1f)) * Vector2.Normalize(new Vector2(+0.1f, -0.1f));
            Vector2 c = getDis(pos + new Vector2(-0.1f, +0.1f)) * Vector2.Normalize(new Vector2(-0.1f, +0.1f));
            Vector2 d = getDis(pos + new Vector2(+0.1f, +0.1f)) * Vector2.Normalize(new Vector2(+0.1f, +0.1f));

            Vector2 normal = new Vector2(a.X + b.X + c.X + d.X, a.Y + b.Y + c.Y + d.Y);
            if (normal.Length() > 0f)
                normal.Normalize();
            return normal;
        }
    }
}
