using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowGame
{
    class ShadowCastermine
    {
        private struct DirectionVector
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public DirectionVector(int x, int y) : this()
            {
                this.X = x;
                this.Y = y;
            }
        }

        private struct ColumnPortion
        {
            public int X { get; private set; }
            public DirectionVector bottomVector { get; private set; }
            public DirectionVector topVector { get; private set; }
            public ColumnPortion(int x, DirectionVector bottom, DirectionVector top) : this()
            {
                this.X = x;
                this.bottomVector = bottom;
                this.topVector = top;
            }
        }

        public static void ComputeFieldOfViewWithShadowCasting(int x, int y, int radius, Func<int, int, bool> isOpaque, Action<int, int> setFoV)
        {
            Func<int, int, bool> opaque = TranslateOrigin(isOpaque, x, y);
            Action<int, int> fov = TranslateOrigin(setFoV, x, y);

            //for (int octant = 0; octant < 8; ++octant)
            //{
            //    ComputeFieldOfViewInOctantZero(
            //        TranslateOctant(opaque, octant),
            //        TranslateOctant(fov, octant),
            //        radius);
            //}
            ComputeFieldOfViewInOctantZero(
                   opaque,fov,
                   radius);
        }

        private static Func<int, int, T> TranslateOrigin<T>(Func<int, int, T> f, int x, int y)
        {
            return (a, b) => f(a + x, b + y);
        }

        private static Action<int, int> TranslateOrigin(Action<int, int> f, int x, int y)
        {
            return (a, b) => f(a + x, b + y);
        }

        private static void ComputeFoVColumnPortion(int x, DirectionVector topVector, DirectionVector bottomVector, Func<int, int, bool> isOpaque, Action<int, int> setFieldOfView, int radius, Queue<ColumnPortion> queue)
        {
            int topY = x * topVector.X / topVector.Y;
            int bottomY = x * bottomVector.X / bottomVector.Y;

            int quotient = ((2 * x + 1) * topVector.Y) / (2 * topVector.X);
            int remainder = ((2 * x + 1) * topVector.Y) % (2 * topVector.X);

            bool? wasLastCellOpaque = null;
            for (int y = topY; y >= bottomY; --y)
            {
                bool inRadius = isInRadius(x, y, radius);
                if (inRadius)
                {
                    //the current cell is in the view.
                    setFieldOfView(x, y);
                }

                bool currentIsOpaque = !inRadius || isOpaque(x, y);
                if (wasLastCellOpaque != null)
                {
                    if (currentIsOpaque)
                    {
                        if (!wasLastCellOpaque.Value)
                        {
                            queue.Enqueue(new ColumnPortion(x + 1, new DirectionVector(x * 2 - 1, y * 2 + 1), topVector));
                        }
                    }
                    else if (wasLastCellOpaque.Value)
                    {
                        topVector = new DirectionVector(x + 2 + 1, y * 2 + 1);
                    }
                }
                wasLastCellOpaque = currentIsOpaque;
            }
            if (wasLastCellOpaque != null && !wasLastCellOpaque.Value)
                queue.Enqueue(new ColumnPortion(x + 1, bottomVector, topVector));

        }

        private static bool isInRadius(int x, int y, int radius)
        {
            return (2 * x - 1) * (2 * x - 1) + (2 * y - 1) * (2 * y - 1) <= 4 * radius * radius;
        }


        private static void ComputeFieldOfViewInOctantZero(Func<int, int, bool> isOpaque, Action<int, int> setFieldOfView, int radius)
        {
            var queue = new Queue<ColumnPortion>();
            queue.Enqueue(new ColumnPortion(0, new DirectionVector(1, 0), new DirectionVector(1, 1)));
            while (queue.Count() != 0)
            {
                var current = queue.Dequeue();
                if (current.X >= radius)
                    continue;
                ComputeFoVColumnPortion(current.X, current.topVector, current.bottomVector, isOpaque, setFieldOfView, radius, queue);
            }
        }

        private static Func<int, int, T> TranslateOctant<T>(Func<int, int, T> f, int octant)
        {
            switch (octant)
            {
                default: return f;
                case 1: return (x, y) => f(y, x);
                case 2: return (x, y) => f(-y, x);
                case 3: return (x, y) => f(-x, y);
                case 4: return (x, y) => f(-x, -y);
                case 5: return (x, y) => f(-y, -x);
                case 6: return (x, y) => f(y, -x);
                case 7: return (x, y) => f(x, -y);
            }
        }

        private static Action<int, int> TranslateOctant(Action<int, int> f, int octant)
        {
            switch (octant)
            {
                default: return f;
                case 1: return (x, y) => f(y, x);
                case 2: return (x, y) => f(-y, x);
                case 3: return (x, y) => f(-x, y);
                case 4: return (x, y) => f(-x, -y);
                case 5: return (x, y) => f(-y, -x);
                case 6: return (x, y) => f(y, -x);
                case 7: return (x, y) => f(x, -y);
            }
        }
    }
}
