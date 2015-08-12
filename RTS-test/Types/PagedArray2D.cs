using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.Types
{
    public class PagedArray2D<T>
    {
        public struct Page
        {
            public T[,] nodes;
        }

        private Dictionary<Point, Page> pages = new Dictionary<Point, Page>();
        private T nullNode;
        private int sizeX;
        private int sizeY;

        public PagedArray2D(T nullNode, int sizeX, int sizeY)
        {
            this.nullNode = nullNode;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        public T this[int x, int y]
        {
            get
            {
                Point pos = new Point(x, y);
                Point localPos = new Point((int)((uint)pos.X % sizeX), (int)((uint)pos.Y % sizeY));
                Point pagePos = pos - localPos;
                if (pos.X < 0)
                    pagePos.X--;
                if (pos.Y < 0)
                    pagePos.Y--;

                if (!pages.ContainsKey(pagePos))
                    return nullNode;

                return pages[pagePos].nodes[localPos.X, localPos.Y];
            }
            set
            {
                Point pos = new Point(x, y);
                Point localPos = new Point((int)((uint)pos.X % sizeX), (int)((uint)pos.Y % sizeY));
                Point pagePos = pos - localPos;
                if (pos.X < 0)
                    pagePos.X--;
                if (pos.Y < 0)
                    pagePos.Y--;

                if (!pages.ContainsKey(pagePos))
                {
                    Page page = new Page();
                    page.nodes = new T[sizeX, sizeY];
                    page.nodes[localPos.X, localPos.Y] = value;
                    pages.Add(pagePos, page);

                }
                else
                    pages[pagePos].nodes[localPos.X, localPos.Y] = value;
            }
        }

        public Dictionary<Point, Page> Pages
        {
            get { return pages; }
        }

        public int SizeX
        {
            get { return sizeX; }
        }

        public int SizeY
        {
            get { return sizeY; }
        }
    }
}
