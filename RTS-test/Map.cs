using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class Map<T>
    {
        private T[,] data;

        public Map(int width, int height)
        {
            data = new T[width, height];
        }

        public T getData(int x, int y)
        {
            return data[x, y];
        }

        public void setData(int x, int y, T data)
        {
            this.data[x, y] = data;
        }
    }
}
