using Godot;

public class BaseCanvas : Control
{
    private PackedScene mainMenu;

    /// <summary>
	/// Constructor loading in the main menu scene for backwards scene navigation
	/// </summary>
	public BaseCanvas() {
		mainMenu = ResourceLoader.Load<PackedScene>("res://_Scenes/MainMenu.tscn");
	}

    /// <summary>
    /// Back buttone pressed event catch
    /// </summary>
    private void _on_Back_pressed() {
        GetTree().ChangeSceneTo(mainMenu);
    }
}
