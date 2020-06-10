using Godot;

public class DeepQLearningCanvas : Control
{
    private ProgressBar healthBar;
    private Label episodeIndicator;

    public override void _Ready()
    {
        healthBar = this.Get<ProgressBar>("Health/ProgressBar");
        episodeIndicator = this.Get<Label>("Episode/Label");        
    }

    public void SetHealth(int health) {
        healthBar.Value = health;
    }

    public void SetEpisode(int episode) {
        episodeIndicator.Text = "Episode #: " + episode;
    }
}
