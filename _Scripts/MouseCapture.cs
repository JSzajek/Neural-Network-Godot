using Godot;
using System.Collections.Generic;
using System.Linq;

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


	public override void _Ready() {
		visualizer = GetParent().GetParentOrNull<DenseNetworkVisualizer>();
		lines = new List<Line2D>();
		texRect = GetNode<TextureRect>("TextureRect");
		viewport = GetNode<Viewport>("ViewportContainer/Drawport");
		drawLine = GetNode<Line2D>("ViewportContainer/Drawport/Line2D");
		debug = this.Get<Control>("ViewportContainer/Drawport/Debug");
	}


	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("draw") && !Draw && CanDraw) {
			texRect.Texture = null;
			ClearInput();
			Draw = true;
			makeLine();
			AddVec();
		}
		if ((!CanDraw && Draw) || @event.IsActionReleased("draw") && Draw) {
			CloseLine();
			Draw = false;
		}
	}

	public void setOutputSize(int newSize) {
		OutputSize = newSize;
	}

	public void makeLine() {
		drawLine.Show(); 
	}

	public void setImageSize(int newSize) {
		ImageSize = newSize;
	}

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

		lines.OrderBy(x => calculateMagnitude(x.Points[0], x.Points[1]));
		lines = lines.Take(OutputSize).ToList();

		List<float> outputs = new List<float>();
		for (int i = 0; i < OutputSize; i++) {
			if (lines[i] == null) {
				outputs.Add(0);
				outputs.Add(0);
			}
			else {
				output.Add(calculateMagnitude(lines[i].Points[0], lines[i].Points[1]));
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

	private bool lineComparison(Line line1, Line line2) {
		return calculateMagnitude(line1.A, line1.B) > calculateMagnitude(line2.A, line2.B);
	}

	private float calculateMagnitude(Vector2 point1, Vector2 point2) {
		return Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.y - point2.y, 2));
	}

	private float calculateDirection(Vector2 point1, Vector2 point2) {
		return Mathf.Atan2(point2.y - point1.y, point2.x - point1.x);
	}
	
	private async void AddVec() {
		if (Draw) {
			drawLine?.AddPoint(GetLocalMousePosition());
			await ToSignal(GetTree().CreateTimer(0.005f), "timeout");
			AddVec();
		}
	}

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

	private void _on_MouseCapture_mouse_entered() {
		CanDraw = true;
	}

	private void _on_MouseCapture_mouse_exited() {
		CanDraw = false;
	}

	private void _on_Submit_pressed() {
		if (output?.Count > 0 && visualizer != null) {
			output = output.Take(OutputSize).ToList();
			visualizer.InputData = output;
		}
	}

	private void _on_Clear_pressed() {
		texRect.Texture = null;
		if (GetParent().HasMethod("ClearInput")) {
			ClearInput();
			visualizer?.ClearInput();
		}
	}

	private void ClearInput() {
		foreach(var child in debug.GetChildren<Line2D>()) {
			debug.RemoveChild(child);
		}
		lines.Clear();
	}
}

public struct Line {
	public Vector2 A;
	public Vector2 B;

	public Line(Vector2 a, Vector2 b) {
		A = a;
		B = b;
	}
}

public class PointAnalysis {
	private readonly float tolerance = 0.5f;

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

	private float pointSlope(Vector2 start, Vector2 end) {
		var denom = (end.x - start.x);
		return denom != 0 ? (float) (end.y - start.y) / denom : 0; 
	}

	private float relDifference(float slopeA, float slopeB) {
		var diff = Mathf.Abs(slopeA - slopeB);
		var denom = Mathf.Max(Mathf.Abs(slopeA), Mathf.Abs(slopeB));
		return denom == 0 ? 0 : diff/denom;
	}

}
