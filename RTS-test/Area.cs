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
        
        public Area(PagedArray2D<bool> tiles)
        {
            this.tiles = tiles;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<Point, PagedArray2D<bool>.Page> page in tiles.Pages)
            {
                for (int x = 0; x < 16; ++x)
                {
                    for (int y = 0; y < 16; ++y)
                    {
                        if (page.Value.nodes[x, y])
                            yield return new Point(page.Key.X+x, page.Key.Y+y);
                    }
                }
            }
        }

    }
}
