using Godot;

/// <summary>
/// Custom behavior class for drag and drop scrolling
/// </summary>
public class Network_Display : ScrollContainer
{
    private bool swiping = false;
    private Vector2 swipeStart;
    private Vector2 swipeMouseStart;

    /// <summary>
    /// Input event catch
    /// </summary>
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton mouseButton) {
            if (mouseButton.Pressed) {
                swiping = true;
                swipeStart = new Vector2(ScrollHorizontal, ScrollVertical);
                swipeMouseStart = mouseButton.Position;
            }
            else {
                swiping = false;
            }
        }
        else if (swiping && @event is InputEventMouseMotion motion) {
            var delta = motion.Position - swipeMouseStart;
            ScrollHorizontal = (int)(swipeStart.x - delta.x);
            ScrollVertical = (int)(swipeStart.y - delta.y);
        }
    }

}
