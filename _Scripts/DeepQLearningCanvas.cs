using Godot;

public class DeepQLearningCanvas : Control
{
    private ProgressBar healthBar;
    private Label episodeIndicator;
    private FileDialog saveDialog;
    private DeepQLearning learning;

    public override void _Ready()
    {
        healthBar = this.Get<ProgressBar>("Health/ProgressBar");
        episodeIndicator = this.Get<Label>("Episode/Label");
        saveDialog = this.Get<FileDialog>("SaveDialog");
        learning = this.Get<DeepQLearning>("/root/DeepQLearning");        
    }

    public void SetHealth(int health) {
        healthBar.Value = health;
    }

    public void SetEpisode(int episode, bool pre = false) {
        episodeIndicator.Text = (pre ? "Pretrain-" : "") + "Episode #: " + episode;
    }

    private void _on_Export_pressed() {
        saveDialog.Show();
    }

    private void _on_SaveDialog_file_selected(string filepath) {
        learning?.ExportNetwork(filepath);
    }

    private void _on_Start_pressed() {
        this.Get<Control>("Cover").Visible = false;
        learning?.StartTraining();
    }
}
