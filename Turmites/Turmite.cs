using Gdk;
using System;
using System.Collections.Generic;
using WorldSystem;

public class Turmite
{
	protected List<Color> ColorPalette = new List<Color>();

	protected Random random = new Random(Guid.NewGuid().GetHashCode());

	public class Tuple
	{
		public int PresentState;
		public int Read;
		public int Write;
		public int NewState;
		public int Turn;

		public Tuple(int presentState, int read, int write, int newState, int turn)
		{
			PresentState = presentState;
			Read = read;
			Write = write;
			NewState = newState;
			Turn = turn;
		}
	}

	public class Turn
	{
		public int Direction;

		public Turn(int direction)
		{
			Direction = direction;
		}
	}

	public readonly Color TurmiteColor;

	public WorldSystem.Point Head = new WorldSystem.Point(0, 0);
	public int State = 1;
	public int Color;
	public int Direction;
	public int Age;
	public int Birth;

	public List<string> Colors = new List<string>();
	public List<string> Directions = new List<string>();

	int LimitX, LimitY;
	public int MinX, MinY;
	public int MaxX, MaxY;

	bool Cyclic;

	string ColorString = "OXABCDEFGHIJKLMNPQRSTUVWYZ";
	public readonly string Source;
	public readonly int CellStates = 2;
	int Delta = 1;

	public readonly List<Tuple> Program = new List<Tuple>();
	public List<Turn> Turns = new List<Turn>();
	public List<WorldSystem.Point> Moves = new List<WorldSystem.Point>();
	protected readonly List<WorldSystem.Point> Neighborhood = new List<WorldSystem.Point>();

	string ParseString(string code, List<string> codebook)
	{
		string pattern = "";

		if (code.Length > 0)
		{
			int offset = 0;

			var current = "";

			while (offset < code.Length && !char.IsDigit(code[offset]))
			{
				current += char.IsLetter(code[offset]) ? code[offset].ToString() : "";

				if (codebook.Contains(current))
				{
					pattern = current;
				}

				offset++;
			}
		}

		return pattern;
	}

	string ParseDigit(string code)
	{
		string pattern = "";

		if (code.Length > 0)
		{
			int offset = 0;

			while (char.IsDigit(code[offset]))
			{
				pattern += code[offset];
				offset++;
			}
		}

		return pattern;
	}

	string Slice(string source, int offset)
	{
		return source.Substring(offset);
	}

	void Extract(ref string pattern, ref int variable)
	{
		variable = -1;

		var code = ParseDigit(pattern);

		if (code.Length > 0)
		{
			pattern = Slice(pattern, code.Length);
			variable = Convert.ToInt32(code);
		}
	}

	void Extract(ref string pattern, ref int variable, ref List<string> limits)
	{
		variable = -1;

		var code = ParseString(pattern, limits);

		if (code.Length > 0)
		{
			pattern = Slice(pattern, code.Length);

			if (limits.Contains(code))
			{
				variable = limits.FindIndex(item => item == code);
			}
		}
	}

	public void AddColor(string color)
	{
		if (color.Length > 0)
		{
			if (!Colors.Contains(color))
				Colors.Add(color);
		}
	}

	public void AddMove(int DeltaX, int DeltaY)
	{
		var movement = new WorldSystem.Point(DeltaX, DeltaY);

		if (!Moves.Contains(movement))
		{
			Moves.Add(movement);
		}
	}

	public void AddTurn(int Direction, string Name)
	{
		if (Name.Length > 0)
		{
			var turn = new Turn(Direction);

			if (!Turns.Contains(turn) && !Directions.Contains(Name))
			{
				Turns.Add(turn);

				Directions.Add(Name);
			}
		}
	}

	void Clip(ref int Value, int Limit)
	{
		// clip to lower bound
		var clipped = Math.Max(0, Value);

		// clip to upper bound
		Value = Math.Min(clipped, Limit - 1);
	}

	public void ParseProgram(string source)
	{
		int PresentState = 0;
		int Read = 0;
		int Write = 0;
		int NewState = 0;
		int Turn = 0;

		var codes = source.Split(',');

		if (codes.Length > 0)
		{
			foreach (var code in codes)
			{
				string copy = code.Trim();

				Extract(ref copy, ref PresentState);
				Extract(ref copy, ref Read, ref Colors);
				Extract(ref copy, ref Write, ref Colors);
				Extract(ref copy, ref NewState);
				Extract(ref copy, ref Turn, ref Directions);

				if (PresentState >= 0 && Read >= 0 && Write >= 0 && NewState >= 0 && Turn >= 0)
				{
					var tuple = new Tuple(PresentState, Read, Write, NewState, Turn);

					if (!Program.Contains(tuple))
					{
						Program.Add(tuple);
					}
				}
			}
		}
	}

	public void Move()
	{
		Head.X = Cyclic ? World.Cyclic(Head.X, Moves[Direction].X, LimitX) : Head.X + Moves[Direction].X;
		Head.Y = Cyclic ? World.Cyclic(Head.Y, Moves[Direction].Y, LimitY) : Head.Y + Moves[Direction].Y;

		Clip(ref Head.X, LimitX);
		Clip(ref Head.Y, LimitY);

		UpdateLimits(Head.X, Head.Y);
	}

	public void SetLimits(int width, int height)
	{
		LimitX = width;
		LimitY = height;
	}

	// Update Turmite Boundaries
	void UpdateLimits(int X, int Y)
	{
		// Update Turmite Boundaries
		MinX = X < MinX ? X : MinX;
		MinY = Y < MinY ? Y : MinY;
		MaxX = X > MaxX ? X : MaxX;
		MaxY = Y > MaxY ? Y : MaxY;
	}

	void ResetLimits()
	{
		// Reset Turmite Boundaries
		MinX = MinY = Int32.MaxValue;
		MaxX = MaxY = Int32.MinValue;
	}

	public List<Tuple> ConsultProgram(int state, int color)
	{
		var Tuples = new List<Tuple>();

		if (Program.Count > 0)
		{
			foreach (var tuple in Program)
			{
				if (tuple.PresentState == state && tuple.Read == color)
					Tuples.Add(tuple);
			}
		}

		return Tuples;
	}

	public void ExecuteProgram(int state, int Read, List<Tuple> procedures)
	{
		foreach (var procedure in procedures)
		{
			if (procedure.PresentState == state && Read == procedure.Read)
			{
				Color = procedure.Write;
				State = procedure.NewState;
				Direction = World.Cyclic(Direction, Turns[procedure.Turn].Direction, Moves.Count);
			}
		}
	}

	public bool GetCyclic()
	{
		return Cyclic;
	}

	public void SetCyclic(bool cyclic)
	{
		Cyclic = cyclic;
	}

	public void ListProgram()
	{
		if (Program.Count > 0)
		{
			foreach (var tuple in Program)
			{
				Console.WriteLine("{0}{1}{2}{3}{4}", tuple.PresentState, Colors[tuple.Read], Colors[tuple.Write], tuple.NewState, Directions[tuple.Turn]);
			}
		}
	}

	void AddMoves()
	{
		Moves.Clear();

		foreach (var cell in Neighborhood)
		{
			AddMove(cell.X, cell.Y);
		}
	}

	void AddTurns()
	{
		Turns.Clear();
		Directions.Clear();

		AddTurn(-1, "L");
		AddTurn(1, "R");

		// Add backward turn
		AddTurn((int)Math.Ceiling((double)Neighborhood.Count / 2), "B");

		// Add No-Turn
		AddTurn(0, "S");
	}

	void AddColors()
	{
		Colors.Clear();

		for (int i = 0; i < CellStates; i++)
		{
			if (i < ColorString.Length)
				AddColor(ColorString[i].ToString());
		}
	}

	public void WriteCell(int x, int y, int val)
	{
		if (x >= 0 && x < LimitX && y >= 0 && y < LimitY)
		{
			World.WriteCell(x, y, val, val >= 0 && val < CellStates ? ColorPalette[val * Delta] : World.EmptyColor);
		}
	}

	public void GradientPalette()
	{
		ColorPalette.Clear();

		ColorPalette.AddRange(Utility.Gradient(TurmiteColor, CellStates));
	}

	public void Update()
	{
		Age++;

		Move();

		var color = World.Grid[Head.X, Head.Y];

		var state = State;

		var procedures = ConsultProgram(state, color);

		if (procedures.Count > 0)
		{
			ExecuteProgram(state, color, procedures);

			if (color != Color || Color > 0)
			{
				WriteCell(Head.X, Head.Y, Color);
			}

			World.ApplyChanges();
		}
	}

	void Generate(int X, int Y)
	{
		AddColors();
		AddMoves();
		AddTurns();

		ParseProgram(Source);

		SetCyclic(Cyclic);

		SetLimits(LimitX, LimitY);

		Head.X = X;
		Head.Y = Y;

		State = 1;

		Direction = random.Next(0, Moves.Count);
	}

	public void Shift(int dx, int dy)
	{
		Head.X += dx;
		Head.Y += dy;

		World.MoveWindow(MinX, MinY, MaxX, MaxY, dx, dy);

		MinX += dx;
		MinY += dy;
		MaxX += dx;
		MaxY += dy;
	}

	public Turmite(int X, int Y, int cellStates, string source, Color color, List<WorldSystem.Point> neighborhood, int birth = 0)
	{
		Birth = birth;

		Neighborhood.Clear();

		Neighborhood.AddRange(neighborhood);

		CellStates = cellStates;

		Source = source;

		TurmiteColor = color;

		Delta = CellStates > 0 ? (256 / (CellStates)) : 0;

		GradientPalette();

		Generate(X, Y);

		ResetLimits();

		UpdateLimits(X, Y);
	}
}
