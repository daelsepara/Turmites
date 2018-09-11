using Gdk;
using System.Collections.Generic;
using WorldSystem;

public static class WorldParameters
{
	public static int Width = 1280;
	public static int Height = 960;

	public static int WindowX = 640;
	public static int WindowY = 480;

	public static int CursorX;
	public static int CursorY;

	public static int[,] Grid;
	public static Color[,] Owner;

	public static List<Trail> ChangeList = new List<Trail>();

	public static void InitGrid()
	{
		Grid = new int[Width, Height];
		Owner = new Color[Width, Height];
	}

	static List<Pixel> PixelWriteBuffer = new List<Pixel>();

    public static List<Pixel> GetPixelWriteBuffer()
    {
        return new List<Pixel>(PixelWriteBuffer);
    }

    public static void ClearPixelWriteBuffer()
    {
        PixelWriteBuffer.Clear();
    }

    public static void PushPixel(Pixel pixel)
    {
        if (pixel != null)
        {
            PixelWriteBuffer.Add(pixel);
        }
    }

	public static void WriteCell(int x, int y, int value, Color color)
	{
		if (x >= 0 && x < Width && y >= 0 && y < Height)
		{
			Grid[x, y] = value;

			Owner[x, y] = color;

			PushPixel(new Pixel(x, y, color));

			ChangeList.Add(new Trail(x, y, value));
		}
	}

	public static void ApplyChanges()
    {
        foreach (var change in ChangeList)
        {
			if (change.Cell.X >= 0 && change.Cell.X < Width && change.Cell.Y >= 0 && change.Cell.Y < Height)
				Grid[change.Cell.X, change.Cell.Y] = change.Value;
        }

        ChangeList.Clear();
    }

	public static void RefreshWindow(int MinX, int MinY, int MaxX, int MaxY)
    {
		for (int y = MinY; y < MaxY + 1; y++)
        {
			for (int x = MinX; x < MaxX; x++)
            {
                if (Grid[x, y] > 0)
                {
                    WriteCell(x, y, Grid[x, y], Owner[x, y]);
                }
            }
        }
    }

	public static void Refresh()
    {
		RefreshWindow(0, 0, Width - 1, Height - 1);
    }

	public static void ClearWindow(int MinX, int MinY, int MaxX, int MaxY)
    {
        if (Grid == null)
            return;

		for (int y = MinY; y < MaxY + 1; y++)
        {
            for (int x = MinX; x < MaxX + 1; x++)
            {
				if (x >= 0 && x < Width && y >= 0 && y < Height)
				{
					Grid[x, y] = 0;
					Owner[x, y] = new Color(0, 0, 0);
				}
            }
        }
    }

	public static void MoveWindow(int MinX, int MinY, int MaxX, int MaxY, int dx, int dy)
    {
        if (Grid == null)
            return;

        for (int y = MinY; y < MaxY + 1; y++)
        {
            for (int x = MinX; x < MaxX + 1; x++)
            {
				var val = Grid[x, y];
				var color = Owner[x, y];

                Grid[x, y] = 0;
                Owner[x, y] = new Color(0, 0, 0);

				var xx = x + dx;
				var yy = y + dy;

				if (xx >= 0 && xx <  Width && yy >= 0 && yy < Height)
				{
					Grid[xx, yy] = val;
                    Owner[xx, yy] = color;
				}
            }
        }
    }

    public static void Clear()
	{
		if (Grid == null)
			return;
		
		for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
				Grid[x, y] = 0;
				Owner[x, y] = new Color(0, 0, 0);
            }
        }
	}
}
