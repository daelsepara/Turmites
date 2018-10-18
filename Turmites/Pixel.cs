using Gdk;
using System.Runtime.InteropServices;

public class Pixel
{
    public Color Color;
    public int X;
    public int Y;

    public Pixel(int x, int y, Color color)
    {
        X = x;
        Y = y;

        Color.Red = color.Red;
        Color.Green = color.Green;
        Color.Blue = color.Blue;
    }

    public void Write(Pixbuf pixbuf, int OriginX, int OriginY)
    {
        if (pixbuf == null || pixbuf.NChannels < 3 || pixbuf.Colorspace != Colorspace.Rgb)
        {
            return;
        }

        int x = OriginX + X;
        int y = OriginY + Y;

        System.IntPtr offset;

        if (y >= 0 && y < pixbuf.Height && x >= 0 && x < pixbuf.Width)
        {
            offset = pixbuf.Pixels + y * pixbuf.Rowstride + x * pixbuf.NChannels;

            Marshal.WriteByte(offset, 0, (byte)(Color.Red & 0xff));
            Marshal.WriteByte(offset, 1, (byte)(Color.Green & 0xff));
            Marshal.WriteByte(offset, 2, (byte)(Color.Blue & 0xff));
        }
    }

    public void Write(Pixbuf pixbuf)
    {
        Write(pixbuf, 0, 0);
    }
}
