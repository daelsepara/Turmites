using Gdk;
using GLib;
using Gtk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WorldSystem;

public partial class MainWindow : Gtk.Window
{
	Pixbuf WorldPixbuf;

	FileChooserDialog ImageChooser;
	Dialog Confirm;

	List<Turmite> Turmites = new List<Turmite>();
	Stopwatch timer = new Stopwatch();
	Mutex Rendering = new Mutex();

	bool ShowTurmites = true;
	bool Paused = true;
	bool IsDragging;

	int X0, Y0, X1, Y1, prevX, prevY;
	int Selected;
	int Epoch;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();

		InitializeUserInterface();
	}

	protected void Tic()
	{
		timer.Restart();
	}

	protected long Ticks()
	{
		return timer.ElapsedMilliseconds;
	}

	protected long Toc()
	{
		var elapsed = Ticks();

		timer.Restart();

		return elapsed;
	}

	protected void InitializeUserInterface()
	{
		InitializeWorldPixbuf();

		WorldImage.Pixbuf = InitializePixbuf(WorldParameters.WindowX, WorldParameters.WindowY);

		WorldPixbuf.Fill(0);

		RenderWorldPixbuf();

		ResetWorldScrollBars();

		Confirm = new Dialog(
			"Are you sure?",
			this,
			DialogFlags.Modal,
			"Yes", ResponseType.Accept,
			"No", ResponseType.Cancel
		)
		{
			Resizable = false,
			KeepAbove = true,
			TypeHint = WindowTypeHint.Dialog,
			WidthRequest = 250
		};

		Confirm.ActionArea.LayoutStyle = ButtonBoxStyle.Center;
		Confirm.WindowStateEvent += OnWindowStateEvent;

		var bg = MainNoteBook.Style.Background(MainNoteBook.State);
		MainToolbar.ModifyBg(StateType.Normal, bg);
		MainToolbar.BorderWidth = 0;

		Idle.Add(new IdleHandler(OnIdle));

		var Defaults = ParameterSets.Default();

		var source = Utility.GetString(Defaults, "Source");
		var states = Utility.GetNumeric(Defaults, "States");

		if (!String.IsNullOrEmpty(source))
			TurmiteProgram.Buffer.Text = Utility.SetText(source);

		TurmiteStates.Value = Convert.ToInt32(states);

		CopyNeighborhood(World.VonNeumannNeighborhood());

		WorldHeight.Value = Convert.ToInt32(WorldParameters.Height);
		WorldWidth.Value = Convert.ToInt32(WorldParameters.Width);

		TurmiteColor.Color = new Color(255, 0, 0);

		ParameterSets.InitializeTurmiteLibrary();

		UpdateTurmiteLibrary(TurmiteLibrary, ParameterSets.TurmiteLibrary);

		TurmiteHeadX.Value = 0;
		TurmiteHeadY.Value = 0;

		ImageChooser = new FileChooserDialog(
			"Save colony snapshot",
			this,
			FileChooserAction.Save,
			"Cancel", ResponseType.Cancel,
			"Save", ResponseType.Accept
		);

		Cyclic.Active = false;

		if (Paused)
			Pause();
	}

	protected Pixbuf InitializePixbuf(int width, int height)
	{
		return new Pixbuf(Colorspace.Rgb, false, 8, width, height);
	}

	protected void InitializeWorldPixbuf()
	{
		if (WorldPixbuf != null)
			WorldPixbuf.Dispose();

		WorldPixbuf = InitializePixbuf(WorldParameters.Width, WorldParameters.Height);

		WorldPixbuf.Fill(0);

		RemoveAllTurmites();

		WorldParameters.InitGrid();

		Epoch = 0;

		UpdateEpoch();
	}

	protected bool GetConfirmation()
	{
		var confirm = Confirm.Run() == (int)ResponseType.Accept;

		Confirm.Hide();

		return confirm;
	}

    protected void UpdateEpoch()
	{
		WorldEpoch.Text = Epoch.ToString();
	}

	protected void RenderTurmite(Pixbuf pixbuf, Turmite turmite)
	{
		var width = turmite.MaxX - turmite.MinX + 1;
		var height = turmite.MaxY - turmite.MinY + 1;

		WorldPixbuf.CopyArea(turmite.MinX, turmite.MinY, width, height, pixbuf, 0, 0);
	}

	protected void RenderTurmiteHeads(Pixbuf pixbuf)
	{
		if (pixbuf != null)
		{
			foreach (var turmite in Turmites)
			{
				var pixel = new Pixel(turmite.Head.X, turmite.Head.Y, turmite.TurmiteColor);

				pixel.Write(pixbuf, 0, 0);
			}
		}
	}

	protected void RenderTurmites(Pixbuf pixbuf)
	{
		WorldParameters.Refresh();

		if (pixbuf != null)
		{
			pixbuf.Fill(0);

			var writeBuffer = WorldParameters.GetPixelWriteBuffer();

			int Updates = writeBuffer.Count;

            if (writeBuffer.Count > 0)
            {
                foreach (var pixel in writeBuffer)
                {
                    pixel.Write(pixbuf, 0, 0);
                }

                WorldParameters.ClearPixelWriteBuffer();
            }

			if (Updates <= 0)
				Pause();
		}
	}

	protected void RefreshTurmites()
	{
		/*
		foreach (var turmite in Turmites)
			turmite.Refresh();
			*/

		WorldParameters.Refresh();
	}

	protected void Refresh()
	{
		RefreshTurmites();
		RenderTurmites(WorldPixbuf);
		RenderWorldPixbuf();
	}

	protected void Quit()
	{
		CleanShutdown();

		Application.Quit();
	}

	protected void ToggleNeighborhood(bool toggle)
	{
		TL.Sensitive = toggle;
		TM.Sensitive = toggle;
		TR.Sensitive = toggle;
		MR.Sensitive = toggle;
		BR.Sensitive = toggle;
		BM.Sensitive = toggle;
		BL.Sensitive = toggle;
		ML.Sensitive = toggle;
	}

	protected void ToggleControls(bool toggle)
	{
		RunButton.Sensitive = toggle;
		PauseButton.Sensitive = !toggle;
		ClearButton.Sensitive = toggle;
		SaveButton.Sensitive = toggle;
		QuitButton.Sensitive = toggle;
		AddButton.Sensitive = toggle;

		TurmiteAddButton.Sensitive = toggle;
		TurmiteColor.Sensitive = toggle;
		TurmiteHeadX.Sensitive = toggle;
		TurmiteHeadY.Sensitive = toggle;
		TurmiteProgram.Sensitive = toggle;
		TurmiteStates.Sensitive = toggle;
		TurmitesList.Sensitive = toggle;

		CopyProgramButton.Sensitive = toggle;
		TurmiteLibrary.Sensitive = toggle;

		WorldWidth.Sensitive = toggle;
		WorldHeight.Sensitive = toggle;

		ToggleNeighborhood(toggle);
	}

	protected void Pause()
	{
		ToggleControls(true);

		Paused = true;
	}

	protected void Run()
	{
		ToggleControls(false);

		Paused = false;
	}

    protected void UpdateTurmiteLocation()
	{
		if (Selected > 0 && Selected <= Turmites.Count)
		{
			var turmite = Turmites[Selected - 1];

			TurmiteHeadX.Value = turmite.Head.X;
            TurmiteHeadY.Value = turmite.Head.Y;

			Age.Text = turmite.Age.ToString();
		}
	}

	protected void InitializeSelected()
	{
		UpdateTurmiteLocation();

		if (Selected > 0 && Selected <= Turmites.Count)
		{
			var turmite = Turmites[Selected - 1];

			TurmiteStates.Value = turmite.CellStates;
			TurmiteProgram.Buffer.Text = Utility.SetText(turmite.Source);
			TurmiteColor.Color = turmite.TurmiteColor;
			Cyclic.Active = turmite.GetCyclic();
			Birth.Text = turmite.Birth.ToString();
			TurmitesList.Active = Selected - 1;
		}
	}

	protected void RenderWorldPixbuf()
	{
		RenderToImage(WorldImage, WorldPixbuf, WorldParameters.CursorX, WorldParameters.CursorY);
	}

	protected void RenderToImage(Gtk.Image image, Pixbuf pixbuf, int OriginX, int OriginY)
	{
		if (pixbuf != null)
		{
			var dest = image.GdkWindow;
			var gc = new Gdk.GC(dest);

			dest.DrawPixbuf(gc, pixbuf, OriginX, OriginY, 0, 0, Math.Min(WorldParameters.WindowX, pixbuf.Width), Math.Min(WorldParameters.WindowY, pixbuf.Height), RgbDither.None, 0, 0);
		}
	}

	protected void CleanShutdown()
	{
		if (WorldPixbuf != null)
			WorldPixbuf.Dispose();

		if (WorldImage.Pixbuf != null)
			WorldImage.Pixbuf.Dispose();
	}

	protected void ResetWorldScrollBars()
	{
		WorldImageScrollX.Value = 0;
		WorldImageScrollY.Value = 0;

		WorldImageScrollX.Sensitive = WorldParameters.Width > WorldParameters.WindowX;
		WorldImageScrollY.Sensitive = WorldParameters.Height > WorldParameters.WindowY;

		if (WorldImageScrollX.Sensitive)
		{
			WorldImageScrollX.SetRange(0.0, WorldParameters.Width - WorldParameters.WindowX);
		}
		else
		{
			WorldImageScrollX.SetRange(0.0, WorldParameters.WindowX);
		}

		if (WorldImageScrollY.Sensitive)
		{
			WorldImageScrollY.SetRange(0.0, WorldParameters.Height - WorldParameters.WindowY);
		}
		else
		{
			WorldImageScrollY.SetRange(0.0, WorldParameters.WindowY);
		}

		WorldParameters.CursorX = 0;
		WorldParameters.CursorY = 0;
	}

	protected void ClearNeighborhood()
	{
		TL.Active = false;
		TM.Active = false;
		TR.Active = false;
		ML.Active = false;
		MR.Active = false;
		BL.Active = false;
		BM.Active = false;
		BR.Active = false;
	}

	protected void CopyNeighborhood(List<WorldSystem.Point> neighborhood)
	{
		ClearNeighborhood();

		foreach (var neighbor in neighborhood)
		{
			TL.Active |= (neighbor.X == -1 && neighbor.Y == -1);
			TM.Active |= (neighbor.X == 0 && neighbor.Y == -1);
			TR.Active |= (neighbor.X == 1 && neighbor.Y == -1);
			ML.Active |= (neighbor.X == -1 && neighbor.Y == 0);
			MR.Active |= (neighbor.X == 1 && neighbor.Y == 0);
			BL.Active |= (neighbor.X == -1 && neighbor.Y == 1);
			BM.Active |= (neighbor.X == 0 && neighbor.Y == 1);
			BR.Active |= (neighbor.X == 1 && neighbor.Y == 1);
		}
	}

	protected List<WorldSystem.Point> SetNeighborhood()
	{
		var neighborhood = new List<WorldSystem.Point>();

		if (TL.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(-1, -1));

		if (TM.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(0, -1));

		if (TR.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(1, -1));

		if (MR.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(1, 0));

		if (BR.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(1, 1));

		if (BM.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(0, 1));

		if (BL.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(-1, 1));

		if (ML.Active)
			World.AddNeighbor(neighborhood, new WorldSystem.Point(-1, 0));

		return neighborhood;
	}

	protected void UpdateTurmitesList(ComboBox combo, List<Turmite> turmites)
	{
		combo.Clear();

		var cell = new CellRendererText();
		combo.PackStart(cell, false);
		combo.AddAttribute(cell, "text", 0);
		var store = new ListStore(typeof(string));
		combo.Model = store;

		var index = 0;

		foreach (var turmite in turmites)
		{
			store.AppendValues(String.Format("Turmite {0}", index));

			index++;
		}

		TurmitesList.Active = turmites.Count > 0 ? turmites.Count - 1 : -1;
	}

	protected void UpdateTurmiteLibrary(ComboBox combo, List<TurmiteCode> turmites)
	{
		combo.Clear();

		var cell = new CellRendererText();
		combo.PackStart(cell, false);
		combo.AddAttribute(cell, "text", 0);
		var store = new ListStore(typeof(string));
		combo.Model = store;

		foreach (var turmite in turmites)
		{
			store.AppendValues(turmite.Name);
		}

		TurmiteLibrary.Active = turmites.Count > 0 ? 0 : -1;
	}

	protected void AddTurmite(int X, int Y)
	{
		var source = Utility.GetText(TurmiteProgram);
		var neighborhood = SetNeighborhood();
		var states = Convert.ToInt32(TurmiteStates.Value);
		var turmite = new Turmite(X, Y, states, source, TurmiteColor.Color, neighborhood, Epoch);

		turmite.SetCyclic(Cyclic.Active);

		turmite.SetLimits(WorldParameters.Width, WorldParameters.Height);

		Turmites.Add(turmite);

		UpdateTurmitesList(TurmitesList, Turmites);
	}

	protected void RemoveAllTurmites()
	{
		Turmites.Clear();

		TurmitesList.Clear();

		Selected = 0;

		WorldPixbuf.Fill(0);

		RenderWorldPixbuf();

		WorldParameters.ClearPixelWriteBuffer();

		WorldParameters.Clear();
	}

	protected void SaveImageFile()
	{
		if (Selected > 0 && Turmites.Count > 0 && (Selected - 1) < Turmites.Count)
		{
			ImageChooser.Title = "Save turmite";
		}
		else
		{
			ImageChooser.Title = "Save world snapshot";
		}

		// Add most recent directory
		if (!string.IsNullOrEmpty(ImageChooser.Filename))
		{
			var directory = System.IO.Path.GetDirectoryName(ImageChooser.Filename);

			if (Directory.Exists(directory))
			{
				ImageChooser.SetCurrentFolder(directory);
			}
		}
		if (ImageChooser.Run() == (int)ResponseType.Accept)
		{
			if (!string.IsNullOrEmpty(ImageChooser.Filename))
			{
				var FileName = ImageChooser.Filename;

				if (!FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
				{
					FileName += ".png";
				}

				if (Selected > 0 && Turmites.Count > 0 && (Selected - 1) < Turmites.Count)
				{
					var num = Selected - 1;
					var turmite = Turmites[num];

					// Populate Write Buffer
					WorldParameters.Refresh();

					var width = turmite.MaxX - turmite.MinX + 1;
					var height = turmite.MaxY - turmite.MinY + 1;

					var temp = InitializePixbuf(width, height);

					if (temp != null)
					{
						temp.Fill(0);

						Console.WriteLine("width: {0} height: {1}", width, height);

						RenderTurmite(temp, turmite);

						temp.Save(FileName, "png");

						temp.Dispose();
					}
				}
				else
				{
					if (WorldPixbuf != null)
					{
						WorldPixbuf.Save(FileName, "png");
					}
				}
			}
		}

		ImageChooser.Hide();
	}

	void OnQuitButtonClicked(object sender, EventArgs args)
	{
		OnDeleteEvent(sender, new DeleteEventArgs());
	}

	void OnShowButtonClicked(object sender, EventArgs args)
	{
		if (Turmites.Count > 0)
		{
			ShowTurmites = !ShowTurmites;

			if (Paused)
				Refresh();
		}
	}

	void OnClearButtonClicked(object sender, EventArgs args)
	{
		if (!Paused)
			return;

		if (Turmites.Count > 0)
		{
			if (GetConfirmation())
			{
				Epoch = 0;

				RemoveAllTurmites();
			}
		}
		else
		{
			Epoch = 0;
		}

		UpdateEpoch();
	}

	void OnRunButtonClicked(object sender, EventArgs args)
	{
		if (!Paused)
			return;

		if (Turmites.Count > 0)
		{
			Run();
		}
	}

	void OnPauseButtonClicked(object sender, EventArgs args)
	{
		if (Paused)
			return;

		Pause();
	}

	void OnSaveButtonClicked(object sender, EventArgs args)
	{
		if (!Paused)
			return;

		SaveImageFile();
	}

	void OnCopyProgramButtonClicked(object sender, EventArgs args)
	{
		if (!Paused)
			return;

		if (TurmiteLibrary.Active >= 0 && TurmiteLibrary.Active < ParameterSets.TurmiteLibrary.Count)
		{
			var source = ParameterSets.TurmiteLibrary[TurmiteLibrary.Active].Source;

			if (!String.IsNullOrEmpty(source))
				TurmiteProgram.Buffer.Text = Utility.SetText(source);

			TurmiteStates.Value = ParameterSets.TurmiteLibrary[TurmiteLibrary.Active].States;
		}
	}

	void OnAddButtonClicked(object sender, EventArgs args)
	{
		if (!Paused)
			return;

		Refresh();

		var originX = Convert.ToInt32(TurmiteHeadX.Value);
		var originY = Convert.ToInt32(TurmiteHeadY.Value);

		AddTurmite(originX, originY);
	}

	protected void OnWindowStateEvent(object sender, WindowStateEventArgs args)
	{
		var state = args.Event.NewWindowState;

		if (state == WindowState.Iconified)
		{
			Confirm.Hide();
		}

		args.RetVal = true;
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		if (GetConfirmation())
		{
			Quit();
		}

		a.RetVal = true;
	}

	bool OnIdle()
	{
		Rendering.WaitOne();

		if (!Paused && Turmites.Count > 0)
		{
			Epoch++;

			UpdateEpoch();

			Toc();

			var start = Ticks();

			foreach (var turmite in Turmites)
				turmite.Update();

			if (Selected > 0)
			{
				UpdateTurmiteLocation();
			}

			RenderTurmites(WorldPixbuf);

			Console.WriteLine("Turmites Rendered in {0} ms", Ticks() - start);
		}

		if (ShowTurmites)
			RenderTurmiteHeads(WorldPixbuf);

		RenderWorldPixbuf();

		Rendering.ReleaseMutex();

		return true;
	}

	protected void OnWorldEventBoxButtonPressEvent(object o, ButtonPressEventArgs args)
	{
		X0 = Convert.ToInt32(args.Event.X);
		Y0 = Convert.ToInt32(args.Event.Y);

		X1 = X0;
		Y1 = Y0;

		if (!Paused)
			return;

		if (args.Event.Button == 3)
		{
			IsDragging = false;

			for (int i = 0; i < Turmites.Count; i++)
			{
				var minX = Turmites[i].MinX;
				var minY = Turmites[i].MinY;
				var maxX = Turmites[i].MaxX;
				var maxY = Turmites[i].MaxY;

				var box = new Box(minX, minY, maxX, maxY);

				var cx = X1 + WorldParameters.CursorX;
				var cy = Y1 + WorldParameters.CursorY;

				if (GtkSelection.Selection.InBox(cx, cy, box))
				{
					Turmites.RemoveAt(i);

					Refresh();

					System.GC.Collect();

					System.GC.WaitForPendingFinalizers();

					break;
				}
			}

			if (Turmites.Count <= 0)
				TurmitesList.Clear();

			UpdateTurmitesList(TurmitesList, Turmites);
		}
		else
		{
			if (args.Event.Button == 1)
			{
				Selected = 0;

				for (int i = 0; i < Turmites.Count; i++)
				{
					var minX = Turmites[i].MinX;
					var minY = Turmites[i].MinY;
					var maxX = Turmites[i].MaxX;
					var maxY = Turmites[i].MaxY;

					var box = new Box(minX, minY, maxX, maxY);

					var cx = X1 + WorldParameters.CursorX;
					var cy = Y1 + WorldParameters.CursorY;

					if (GtkSelection.Selection.InBox(cx, cy, box))
					{
						IsDragging = true;

						prevX = X0;
						prevY = Y0;

						Selected = i + 1;

						InitializeSelected();

						break;
					}
				}
			}
		}
	}

	protected void OnWorldEventBoxButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
	{
		X1 = Convert.ToInt32(args.Event.X);
		Y1 = Convert.ToInt32(args.Event.Y);

		if (!Paused)
			return;

		if (args.Event.Button == 1)
		{
			if (IsDragging)
			{
				IsDragging = false;

				if (Selected > 0)
				{
					InitializeSelected();

					Refresh();
				}
			}
			else
			{
				if (TurmiteAddButton.Active)
				{
					Refresh();

					var originX = X1 + WorldParameters.CursorX;
					var originY = Y1 + WorldParameters.CursorY;

					AddTurmite(originX, originY);

					TurmiteHeadX.Value = originX;
					TurmiteHeadY.Value = originY;
				}
				else
				{
					if (Selected == 0)
						TurmitesList.Active = -1;
				}
			}
		}
	}

	protected void OnWorldEventBoxMotionNotifyEvent(object o, MotionNotifyEventArgs args)
	{
		X1 = Convert.ToInt32(args.Event.X);
		Y1 = Convert.ToInt32(args.Event.Y);

		if (!IsDragging || !Paused)
			return;

		if (IsDragging)
		{
			var dx = X1 - prevX;
			var dy = Y1 - prevY;

			prevX = X1;
			prevY = Y1;

			// move turmite
			if (Selected > 0 && Selected <= Turmites.Count)
			{
				Turmites[Selected - 1].Shift(dx, dy);
			}
		}
	}

	protected void OnWorldImageScrollXValueChanged(object sender, EventArgs e)
	{
		WorldParameters.CursorX = Convert.ToInt32(WorldImageScrollX.Value);
	}

	protected void OnWorldImageScrollYValueChanged(object sender, EventArgs e)
	{
		WorldParameters.CursorY = Convert.ToInt32(WorldImageScrollY.Value);
	}

	protected void OnTurmitesListChanged(object sender, EventArgs e)
	{
		if (TurmitesList.Active >= 0 && TurmitesList.Active < Turmites.Count)
		{
			Selected = TurmitesList.Active + 1;

			InitializeSelected();
		}
	}

	protected void OnWorldWidthValueChanged(object sender, EventArgs e)
	{
		if (!Paused)
			return;

		WorldParameters.Width = Convert.ToInt32(WorldWidth.Value);

		InitializeWorldPixbuf();

		ResetWorldScrollBars();

		Refresh();
	}

	protected void OnWorldHeightValueChanged(object sender, EventArgs e)
	{
		if (!Paused)
			return;

		WorldParameters.Height = Convert.ToInt32(WorldHeight.Value);

		InitializeWorldPixbuf();

		ResetWorldScrollBars();

		Refresh();
	}
}
