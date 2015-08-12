using Microsoft.Xna.Framework;
using RTS_test.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class Area : IEnumerable
    {
        private PagedArray2D<bool> tiles;

        public Area()
        {
            this.tiles = new PagedArray2D<bool>(false, 16, 16);
        }
        public Area(PagedArray2D<bool> tiles)
        {
            this.tiles = tiles;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<Point, PagedArray2D<bool>.Page> page in tiles.Pages)
            {
                for (int x = 0; x < tiles.SizeX; ++x)
                {
                    for (int y = 0; y < tiles.SizeY; ++y)
                    {
                        if (page.Value.nodes[x, y])
                            yield return new Point(page.Key.X+x, page.Key.Y+y);
                    }
                }
            }
        }

        public bool this[int x, int y]
        {
            get
            {
                return tiles[x, y];
            }
            set
            {
                tiles[x, y] = value;
            }
        }

    }
}
