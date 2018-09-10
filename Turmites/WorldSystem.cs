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
