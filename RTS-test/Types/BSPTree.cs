//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;

//namespace RTS_test.Types
//{
//    class BSPTree<T>
//    {
//        class Node
//        {
//            public Node a;
//            public Node b;
//            public float line;
//            public Vector2 pos;
//            public T t;
//            public RectangleF rootRect;

//            public Node(Node a, Node b, float line, Vector2 pos, T t)
//            {
//                this.a = a;
//                this.b = b;
//                this.line = line;
//                this.pos = pos;
//                this.t = t;
//            }
//        }

//        private Node root;
//        T defaultValue;

//        public BSPTree(T defaultValue, RectangleF rect)
//        {
//            this.defaultValue = defaultValue;
//            root = new Node(null, null, 0.5, null, defaultValue);
//            this.rootRect = rect;
//        }

//        public T searchNearest(Vector2 pos)
//        {
//            int treeLevel = 0;

//            return searchNearest(pos, root, isXLine, rootRect);
//        }

//        private T searchNearest(Vector2 pos, Node node, int treeLevel, RectangleF rectangle)
//        {
//            if (node == null)
//                return defaultValue;

//            if (node.t != defaultValue)
//                return node.t;

//            RectangleF a;
//            RectangleF b;

//            if (treeLevel % 2 == 0)
//            {
//                A = new RectangleF(rectangle.X, rectangle.Y, node.line, rectangle.Height);
//                B = new RectangleF(rectangle.X + node.line, rectangle.Y, rectangle.Width - node.line, rectangle.Height);
//            }
//            else
//            {
//                A = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, node.line);
//                B = new RectangleF(rectangle.X, rectangle.Y + node.line, rectangle.Width, rectangle.Height - node.line);
//            }

//            float aDis = getRectDisSquared(a, pos);
//            float bDis = getRectDisSquared(b, pos);

//            if (aDis < bDis)
//            {
//                T aValue = searchNearest(pos, node.a, treeLevel + 1, a);
//                if (aValue != defaultValue)
//                    return aValue;

//                return searchNearest(pos, node.b, treeLevel + 1, b);
//            }
//            else
//            {
//                T bValue = searchNearest(pos, node.b, treeLevel + 1, b);
//                if (bValue != defaultValue)
//                    return bValue;

//                return searchNearest(pos, node.b, treeLevel + 1, b);
//            }

//        }



//        float getRectDisSquared(RectangleF rectangle,  Vector2 pos)
//        {
//            Vector2 p = pos + new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
//            Vector2 b = new Vector2(Math.Abs(rectangle.Width / 2), Math.Abs(rectangle.Height / 2));
//            return new Vector2(Math.Max(p.x - b.x, 0f), Math.Max(p.Y - b.Y, 0f)).LengthSquared();
//        }

//    }
//}
