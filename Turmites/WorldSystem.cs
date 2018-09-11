using Gdk;
using System.Collections.Generic;

namespace WorldSystem
{
	public class Point
	{
		public int X;
		public int Y;

		public Point()
		{

		}

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public class TurmiteCode
	{
		public string Name;
		public string Source;
		public int States;

		public TurmiteCode()
		{
			Default();
		}

		public TurmiteCode(string name, string source)
		{
			if (name.Trim().Length > 0 && name.Trim().Length > 0)
			{
				Name = name;
				Source = source;
				States = 2;
			}
			else
			{
				Default();
			}
		}

		public TurmiteCode(string name, string source, int states)
		{
			if (name.Trim().Length > 0 && name.Trim().Length > 0 && states > 1)
			{
				Name = name;
				Source = source;
				States = states;
			}
			else
			{
				Default();
			}
		}

		protected void Default()
		{
			Name = "Default";
			Source = "1OX2R,1XO2R,2OX1S,2XX2S";
			States = 2;
		}
	}

	public static class World
	{
		public static void AddNeighbor(List<Point> Neighborhood, Point neighbor)
		{
			if (!Neighborhood.Contains(neighbor))
			{
				Neighborhood.Add(neighbor);
			}
		}

		public static List<Point> EmptyNeighborhood()
		{
			return new List<Point>();
		}

		public static List<Point> MooreNeighborhood()
		{
			var neighborhood = new List<Point>();

			AddNeighbor(neighborhood, new Point(-1, -1));
			AddNeighbor(neighborhood, new Point(0, -1));
			AddNeighbor(neighborhood, new Point(1, -1));
			AddNeighbor(neighborhood, new Point(1, 0));
			AddNeighbor(neighborhood, new Point(1, 1));
			AddNeighbor(neighborhood, new Point(0, 1));
			AddNeighbor(neighborhood, new Point(-1, 1));
			AddNeighbor(neighborhood, new Point(-1, 0));

			return neighborhood;
		}

        // 6 Neighbor approximation of the hexagonal lattice
        public static List<Point> HexNeighborhood()
		{
			var neighborhood = new List<Point>();

			AddNeighbor(neighborhood, new Point(-1, -1));
			AddNeighbor(neighborhood, new Point(0, -1));
			AddNeighbor(neighborhood, new Point(1, 0));
			AddNeighbor(neighborhood, new Point(1, 1));
			AddNeighbor(neighborhood, new Point(0, 1));
			AddNeighbor(neighborhood, new Point(-1, 0));

			return neighborhood;
		}

        // 4 Neighbor Von Neumann
        public static List<Point> VonNeumannNeighborhood()
		{
			var neighborhood = new List<Point>();

			AddNeighbor(neighborhood, new Point(0, -1));
			AddNeighbor(neighborhood, new Point(1, 0));
			AddNeighbor(neighborhood, new Point(0, 1));
			AddNeighbor(neighborhood, new Point(-1, 0));

			return neighborhood;
		}

		public static int Cyclic(int location, int offset, int limit)
		{
			return offset >= 0 ? (location + offset) % limit : (location + limit + offset) % limit;
		}

		public static Color DefaultColor = new Color(0, 255, 255);
		public static Color EmptyColor = new Color(0, 0, 0);

		public static int[,] Grid;
		public static Color[,] Owner;

		public static List<Trail> ChangeList = new List<Trail>();

		public static void InitGrid()
		{
			Grid = new int[WorldParameters.Width, WorldParameters.Height];
			Owner = new Color[WorldParameters.Width, WorldParameters.Height];
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
			if (x >= 0 && x < WorldParameters.Width && y >= 0 && y < WorldParameters.Height)
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
				if (change.Cell.X >= 0 && change.Cell.X < WorldParameters.Width && change.Cell.Y >= 0 && change.Cell.Y < WorldParameters.Height)
					Grid[change.Cell.X, change.Cell.Y] = change.Value;
			}

			ChangeList.Clear();
		}

		public static void RefreshWindow(int MinX, int MinY, int MaxX, int MaxY)
		{
            ClearPixelWriteBuffer();
            
            ChangeList.Clear();
            
			for (int y = MinY; y < MaxY + 1; y++)
			{
				for (int x = MinX; x < MaxX + 1; x++)
				{
					if (x >= 0 && x < WorldParameters.Width && y >= 0 && y < WorldParameters.Height)
					{
						if (Grid[x, y] > 0)
						{
							WriteCell(x, y, Grid[x, y], Owner[x, y]);
						}
					}
				}
			}
		}

		public static void Refresh()
		{
			RefreshWindow(0, 0, WorldParameters.Width - 1, WorldParameters.Height - 1);
		}

		public static void ClearWindow(int MinX, int MinY, int MaxX, int MaxY)
		{
			if (Grid == null)
				return;

			for (int y = MinY; y < MaxY + 1; y++)
			{
				for (int x = MinX; x < MaxX + 1; x++)
				{
					if (x >= 0 && x < WorldParameters.Width && y >= 0 && y < WorldParameters.Height)
					{
                        WriteCell(x, y, 0, new Color(0, 0, 0));
					}
				}
			}
		}

		public static void MoveWindow(int MinX, int MinY, int MaxX, int MaxY, int dx, int dy)
		{
			if (Grid == null)
				return;

            if (dx == 0 && dy == 0)
                return;

            var width = MaxX - MinX + 1;
            var height = MaxY - MinY + 1;

            var tempv = new int[width, height];
            var tempc = new Color[width, height];

            for (int y = MinY; y < MaxY + 1; y++)
            {
                for (int x = MinX; x < MaxX + 1; x++)
                {
                    if (x >= 0 && x < WorldParameters.Width && y >= 0 && y < WorldParameters.Height)
                    {
                        tempv[x - MinX, y - MinY] = Grid[x, y];
                        tempc[x - MinX, y - MinY] = Owner[x, y];
                        
                        WriteCell(x, y, 0, new Color(0, 0, 0));
                    }
                }
            }
            
            for (int y = MinY; y < MaxY + 1; y++)
            {
                for (int x = MinX; x < MaxX + 1; x++)
                {
                    var xx = x + dx;
                    var yy = y + dy;
                    
                    if (xx >= 0 && xx < WorldParameters.Width && yy >= 0 && yy < WorldParameters.Height)
                    {
                        if (tempv[x - MinX, y - MinY] > 0)
                            WriteCell(xx, yy, tempv[x - MinX, y - MinY], tempc[x - MinX, y - MinY]);
                    }
                }
            }
		}

		public static void Clear()
		{
			if (Grid == null)
				return;

            ClearPixelWriteBuffer();

            ChangeList.Clear();

            for (int y = 0; y < WorldParameters.Height; y++)
			{
				for (int x = 0; x < WorldParameters.Width; x++)
				{
					Grid[x, y] = 0;
					Owner[x, y] = new Color(0, 0, 0);
				}
			}
		}

	}

	public static class IntTrail
	{
		public static int Factor = 10000;

		public static int Convert(int x, int y)
		{
			return y * Factor + x;
		}
	}

	public class TrailInt
	{
		public int Cell;
		public int Value;
		const int Factor = 10000;

		public TrailInt()
		{
			Cell = 0;
			Value = 0;
		}

		public TrailInt(int x, int y, int value)
		{
			Cell = IntTrail.Convert(x, y);

			Value = value;
		}

		public int Y()
		{
			return Cell / Factor;
		}

		public int X()
		{
			return Cell % Factor;
		}
	}

	public class Trail
	{
		public Point Cell;
		public int Value;

		public Trail()
		{
			Cell = new Point(0, 0);

			Value = 0;
		}

		public Trail(int x, int y, int value)
		{
			Cell = new Point(x, y);

			Value = value;
		}
	}
}
