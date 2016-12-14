using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;
using ProjectAlgorithm.Lights;
using Point = ProjectAlgorithm.Entities.Point;

namespace FormsPL
{
    public static class PixelDrawer
    {
        public static void DrawXY(IFace face, Graphics graphics, float deltaX, float deltaY, ILight light, IPoint viewPoint)
        {
            var points = GetFacePoints(face);

            var lightDrawer = new ReflectedLight();

            foreach (var point in points)
            {
                var color = lightDrawer.GetColor(point, face.Color, viewPoint,
                    light, face.Normal, 1, 1, 1, 20, 500);

                graphics.DrawRectangle(new Pen(color), point.X + deltaX, -point.Y + deltaY, 1, 1);
            }
        }

        private static List<Point> GetFacePoints(IFace face)
        {
            var linesPoints = new List<IPoint>();

            foreach (var faceLine in face.Lines)
            {
                linesPoints.AddRange(BresenhamLine(faceLine.First.X, faceLine.First.Y, faceLine.Second.X, faceLine.Second.Y, face.Center.ElementAt(2)));
            }

            var minX = (int)linesPoints.Min(p => p.X);
            var minY = (int)linesPoints.Min(p => p.Y);
            var maxX = (int)linesPoints.Max(p => p.X);
            var maxY = (int)linesPoints.Max(p => p.Y);
            
            var bitmap = new float[Math.Abs(maxX) + Math.Abs(minX) + 1, Math.Abs(maxY) + Math.Abs(minY) + 1];

            linesPoints.ForEach(p => bitmap[(int)p.X + Math.Abs(minX), (int)p.Y + Math.Abs(minY)] = 1);

            var result = new List<Point>();
            
            for (int i = 0; i < bitmap.GetLength(0); i++)
            {
                bool inFlag = false;
                bool first = true;

                for (var j = 0; j < bitmap.GetLength(1); j++)
                {
                    if (bitmap[i, j] == 1)
                    {
                        if (inFlag && !first)
                        {
                            break;
                        }

                        result.Add(new Point(i - Math.Abs(minX), j - Math.Abs(minY), face.Center.ElementAt(2)));
                        inFlag = true;
                    }
                    else
                    {
                        if (!inFlag) continue;

                        first = false;

                        bool yes = false;

                        for (int k = j + 1; k < bitmap.GetLength(1); k++)
                        {
                            if (bitmap[i, k] == 1)
                            {
                                yes = true;
                            }
                        }

                        if (yes)
                        {
                            bitmap[i, j] = 1;
                            result.Add(new Point(i - Math.Abs(minX), j - Math.Abs(minY), face.Center.ElementAt(2)));
                        }
                        
                    }
                }
            }

            return result;
        }

        private static int Sign(float x)
        {
            return (x > 0) ? 1 : (x < 0) ? -1 : 0;
        }

        private static List<IPoint> BresenhamLine(float xstart, float ystart, float xend, float yend, float z)
        {
            var list = new List<IPoint>();

            float pdx, pdy, es, el;

            var dx = xend - xstart;
            var dy = yend - ystart;

            var incx = Sign(dx);
            var incy = Sign(dy);

            if (dx < 0) dx = -dx;
            if (dy < 0) dy = -dy;

            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                es = dx;
                el = dy;
            }

            var x = xstart;
            var y = ystart;
            var err = el / 2;

            list.Add(new Point {X = x, Y = y, Z = z});

            for (int t = 0; t < el; t++)
            {
                err -= es;
                if (err < 0)
                {
                    err += el;
                    x += incx;
                    y += incy;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                list.Add(new Point { X = x, Y = y, Z = z });
            }

            return list;
        }
    }
}