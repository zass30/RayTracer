﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Canvas
    {
        public int width { get; set; }
        public int height { get; set; }
        private Color[,] pixels;

        public Canvas (int width, int height)
        {
            this.width = width;
            this.height = height;
            pixels = new Color[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pixels[i, j] = new Color(0, 0, 0);
                }
            }
        }

        public static Color pixel_at(Canvas c, int x, int y)
        {
            return c.pixels[x,y];
        }

        public static void write_pixel(Canvas c, int x, int y, Color color)
        {
            c.pixels[x, y] = color;
        }

        public static string canvas_to_ppm(Canvas c)
        {
            string s = "P3\n";
            s += c.width + " " + c.height + "\n255\n";
            for (int j = 0; j < c.height; j++)
            {
                for (int i = 0; i < c.width; i++)
                {
                    var color = c.pixels[i, j];
                    s += color.rI + " " + color.gI + " " + color.bI + " ";
                }
                s = s.Trim();
                s += "\n";
            }
            return s;
        }
    }
}
