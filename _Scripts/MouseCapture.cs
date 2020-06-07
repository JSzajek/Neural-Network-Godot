using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Custom control representing a mouse drawing
/// capture box
/// </summary>
public class MouseCapture : Control
{
	public bool CanDraw {get; set;}
	public bool Draw {get; set;}
	public List<float> output { get; set; } = new List<float>();
	public int ImageSize {get; set;}
	public List<Line2D> lines {get; set;}
	public int OutputSize {get; set;}

	private DenseNetworkVisualizer visualizer;
	private TextureRect texRect;
	private Viewport viewport;
	private Line2D drawLine;
	private Control debug;

	/// <summary>
	/// Initializes the parameters 
	/// </summary>
	public override void _Ready() {
		visualizer = GetParent().GetParentOrNull<DenseNetworkVisualizer>();
		lines = new List<Line2D>();
		texRect = GetNode<TextureRect>("TextureRect");
		viewport = GetNode<Viewport>("ViewportContainer/Drawport");
		drawLine = GetNode<Line2D>("ViewportContainer/Drawport/Line2D");
		debug = this.Get<Control>("ViewportContainer/Drawport/Debug");
	}

	/// <summary>
	/// Catches input draw events
	/// </summary>
	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("draw") && !Draw && CanDraw) {
			texRect.Texture = null;
			ClearInput();
			Draw = true;
			drawLine.Show(); 
			AddVec();
		}
		if ((!CanDraw && Draw) || @event.IsActionReleased("draw") && Draw) {
			CloseLine();
			Draw = false;
		}
	}

	/// <summary>
	/// Closes the drawing of the current line. Doing post-processing of
	/// line analysis and trimming and creating the output data
	/// </summary>
	public async void CloseLine() {
		var points = drawLine?.Points;
		var PointAnalysis = new PointAnalysis();
		var drawnLines = PointAnalysis.Run(points);
		foreach(var line in drawnLines) {
			var drawnLine = new Line2D();
			debug?.AddChild(drawnLine);
			drawnLine.Modulate = new Color(GD.Randf(), GD.Randf(), GD.Randf());
			drawnLine.AddPoint(line.A);
			drawnLine.AddPoint(line.B);
			lines.Add(drawnLine);
		}

		lines.OrderBy(x => x.Points[0].DistanceSquaredTo(x.Points[1])); 
		lines = lines.Take(OutputSize).ToList();

		List<float> outputs = new List<float>();
		for (int i = 0; i < OutputSize; i++) {
			if (lines[i] == null) {
				outputs.Add(0);
				outputs.Add(0);
			}
			else {
				output.Add(lines[i].Points[0].DistanceSquaredTo(lines[i].Points[1]));
				output.Add(calculateDirection(lines[i].Points[0], lines[i].Points[1]));
			}
		}

		await ToSignal(GetTree(), "idle_frame");
		await ToSignal(GetTree(), "idle_frame");
		var image = viewport.GetTexture().GetData();

		image.FlipY();
		(var minX, var width, var maxX) = GetImageWidth(points);
		(var minY, var height, var maxY) = GetImageHeight(points);
		

		var rect = new Rect2(minX, minY, width, height);
		image = image.GetRect(rect);
		if (ImageSize > 0) {
			image.Resize(ImageSize, ImageSize);
		}
		ClearInput();
		drawLine?.ClearPoints();
		drawLine?.Hide();

		image.Resize((int)viewport?.Size.x, (int)viewport?.Size.y);
		var tex = new ImageTexture();
		tex.CreateFromImage(image);
		texRect.Texture = tex;
	}

	/// <summary>
	/// Helper method calculating the direction from point1 to point2
	/// </summary>
	/// <param name="point1">The first point</param>
	/// <param name="point2">The second point</param>
	/// <returns>The direction from point1 to point2</returns>
	private float calculateDirection(Vector2 point1, Vector2 point2) {
		return Mathf.Atan2(point2.y - point1.y, point2.x - point1.x);
	}
	
	/// <summary>
	/// Adds the point vector of the current draw
	/// </summary>
	private async void AddVec() {
		if (Draw) {
			drawLine?.AddPoint(GetLocalMousePosition());
			await ToSignal(GetTree().CreateTimer(0.005f), "timeout");
			AddVec();
		}
	}

	/// <summary>
	/// Gets the image width based parameters
	/// </summary>
	/// <param name="arr">The point values of the drawn line</param>
	/// <returns>The min and max X coordinate and width capturing the drawn line</returns>
	private (float, float, float) GetImageWidth(Vector2[] arr) {
		var max = float.MinValue;
		var min = float.MaxValue;
		foreach(Vector2 vec in arr) {
			if (vec.x < min) {
				min = vec.x;
			}
			if (vec.x > max) {
				max = vec.x;
			}
		}
		min = Mathf.Max(viewport.CanvasTransform.origin.x, min);
		max = Mathf.Min(viewport.Size.x, max);
		return (min, max - min, max);
	}

	/// <summary>
	/// Gets the image height based parameters
	/// </summary>
	/// <param name="arr">The point values of the drawn line</param>
	/// <returns>The min and max Y coordinate and height capturing the drawn line</returns>
	private (float, float, float) GetImageHeight(Vector2[] arr) {
		var max = float.MinValue;
		var min = float.MaxValue;
		foreach(Vector2 vec in arr) {
			if (vec.y < min) {
				min = vec.y;
			}
			if (vec.y > max) {
				max = vec.y;
			}
		}
		min = Mathf.Max(viewport.CanvasTransform.origin.y, min);
		max = Mathf.Min(viewport.Size.y, max);
		return (min, max - min, max);
	}

	/// <summary>
	/// mouse entered signal event catch
	/// </summary>
	private void _on_MouseCapture_mouse_entered() {
		CanDraw = true;
	}

	/// <summary>
	/// mouse exit signal event catch
	/// </summary>
	private void _on_MouseCapture_mouse_exited() {
		CanDraw = false;
	}

	/// <summary>
	/// Submit button pressed signal catch
	/// </summary>
	private void _on_Submit_pressed() {
		if (output?.Count > 0 && visualizer != null) {
			output = output.Take(OutputSize).ToList();
			visualizer.InputData = output;
		}
	}

	/// <summary>
	/// Clear button pressed signal catch
	/// </summary>
	private void _on_Clear_pressed() {
		texRect.Texture = null;
		if (GetParent().HasMethod("ClearInput")) {
			ClearInput();
			visualizer?.ClearInput();
		}
	}

	/// <summary>
	/// Clears the input viewport of previous drawn line
	/// </summary>
	private void ClearInput() {
		foreach(var child in debug.GetChildren<Line2D>()) {
			debug.RemoveChild(child);
		}
		lines.Clear();
	}
}

/// <summary>
/// Struct representing a simple line
/// </summary>
public struct Line {
	public Vector2 A;
	public Vector2 B;

	public Line(Vector2 a, Vector2 b) {
		A = a;
		B = b;
	}
}

/// <summary>
/// Point analysis class simplifying drawn line
/// </summary>
public class PointAnalysis {
	private readonly float tolerance = 0.5f;

	/// <summary>
	/// Runs the analysis to simplify the drawn line's points
	/// </summary>
	/// <param name="points">The points of the drawn line</param>
	/// <returns>Simplified list of lines</returns>
	public List<Line> Run(Vector2[] points){
		if (points.Length < 1) {
			return null;
		}
		List<Line> results = new List<Line>();

		Vector2 startPoint = points[0];
		Vector2 curPoint = points[0];
		float curSlope = 0.0f;

		for (int i = 0; i < points.Length - 1; i++) {
			if (i == 0) {
				startPoint = points[i];
				curPoint = points[i];
				curSlope = pointSlope(curPoint, points[i+1]);
				continue;
			}
			curPoint = points[i];
			var testSlope = pointSlope(curPoint, points[i+1]);
			if (relDifference(curSlope, testSlope) > tolerance) {
				results.Add(new Line(startPoint, points[i+1]));
				startPoint = points[i];
				curSlope = testSlope;
			}
		}
		return results;
	}

	/// <summary>
	/// Helper method that calculates the slope of the points
	/// </summary>
	/// <param name="start">The start vector</param>
	/// <param name="end">The end vector</param>
	/// <returns>The slope</returns>
	private float pointSlope(Vector2 start, Vector2 end) {
		var denom = (end.x - start.x);
		return denom != 0 ? (float) (end.y - start.y) / denom : 0; 
	}

	/// <summary>
	/// Helper method that calculates the relative difference between two slope values
	/// </summary>
	/// <param name="slopeA">The first slope value</param>
	/// <param name="slopeB">The second slope value</param>
	/// <returns>The relative difference</returns>
	private float relDifference(float slopeA, float slopeB) {
		var diff = Mathf.Abs(slopeA - slopeB);
		var denom = Mathf.Max(Mathf.Abs(slopeA), Mathf.Abs(slopeB));
		return denom == 0 ? 0 : diff/denom;
	}

}
