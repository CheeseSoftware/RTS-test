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
            public int numNodes;
        }

        private Dictionary<Point, Page> pages = new Dictionary<Point, Page>();
        private T defaultValue;
        private int sizeX;
        private int sizeY;

        public PagedArray2D(T defaultValue, int sizeX, int sizeY)
        {
            this.defaultValue = defaultValue;
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
                    return defaultValue;

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
                    page.numNodes = 1;
                    page.nodes[localPos.X, localPos.Y] = value;
                    pages.Add(pagePos, page);

                }
                else
                {
                    //T oldValue = pages[pagePos].nodes[localPos.X, localPos.Y];
                    //if (oldValue.Equals(defaultValue))
                    //    pages[pagePos].numNodes++;
                    //if (value == defaultValue)
                    //    pages[pagePos].numNodes--;

                    pages[pagePos].nodes[localPos.X, localPos.Y] = value;

                    //if (pages[pagePos].numNodes == 0)
                    //    pages.Remove(pagePos);
                }
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
