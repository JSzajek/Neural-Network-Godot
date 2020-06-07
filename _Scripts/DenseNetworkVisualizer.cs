using Godot;
using System.Collections.Generic;
using System.Linq;

public class DenseNetworkVisualizer : Control
{
	private DllLoader loader;
	private NeuralNetworkLinker linker;
	private Axon axon;
	private MouseCapture mouseCapture;
	private Label outputDisplay;
	private HBoxContainer networkWindow;
	private Button trainButton;
	private Label inputDisplay;
	private SpinBox classificationInput;
    private Popup errorIndicator;
	private List<float> _inputData;
	private List<int> topology;

	public List<float> InputData 
	{
		get => _inputData; 
		set {
			if (value == null && inputDisplay != null) {
				inputDisplay.Text = "Input Data Not Set";
			} else {
				_inputData = value;
				SetInput();
			}
		}
	}

	public int Epochs {get; set;} = 1;

	public float LearningRate {get; set;} = 0.1f;

	public int Classification {get; set;}

	public bool DisplayWeightChange {get; set;}

	public bool Training {get; set;}

	/// <summary>
	/// Initializes parameters
	/// </summary>
	public override void _Ready()
	{
		axon = (GD.Load("res://_Assets/Axon.tscn") as PackedScene)?.Instance() as Axon;
		mouseCapture = GetNode<MouseCapture>("Interface/MouseCapture");
		outputDisplay = GetNode<Label>("Canvas/Output");
		networkWindow = GetNode<HBoxContainer>("Network_Display/Container");
		trainButton = GetNode<Button>("Interface/Train");
		inputDisplay = GetNode<Label>("Interface/InputSet");
		classificationInput = this.Get<SpinBox>("Interface/Classification");
        errorIndicator = this.Get<Popup>("Popup");

		loader = new DllLoader();
		linker = loader.GetNeuralNetworkLinker();

		this.Get<SetupWindow>("SetupWindow")?.Show();
	}

	/// <summary>
	/// Sets up the dense network visualizer based on the passed
	/// topology and whether it is imported or not. 
	/// </summary>
	/// <param name="topology">The topology of the current network</param>
	/// <param name="imported">Whether the setup was imported or not</param>
	/// <returns></returns>
	public async void SetUp(List<int> topology, bool imported) {
		this.topology = topology;
		ClearDisplay();
		
		// Set up mouse capture
		mouseCapture.ImageSize = (int)Mathf.Sqrt(1024);
		mouseCapture.OutputSize = topology.First();

		// Max classification input to output layer indices
		classificationInput.MaxValue = topology.Last() - 1;

		if (!imported) {
            linker.SetupDenseNetwork(this.topology);
		}
		linker.SetLearningRate(LearningRate);

		for (int i = 0; i < topology.Count; i++) {
			CreateAxonLayer(topology[i], i == topology.Count - 1);
		}

		// Wait for layouts to update positions
		await ToSignal(GetTree(), "idle_frame");

		// Connect Axons
		DisplayWeights();
	}

	/// <summary>
	/// Helper method called when after setting input,
	/// updating the input display
	/// </summary>
	private void SetInput() {
		inputDisplay.Text = "Input Data Set";
		var containerChildren = networkWindow.GetChildren();
		var inputLayer = (containerChildren[0] as Node)?.GetChildren();
		for (int i = 0; i < inputLayer.Count; i++) {
			if (inputLayer[i] is Axon axon) {
				axon.Bias = InputData[i];
			}
		}
	}

	/// <summary>
	/// Clears the input
	/// </summary>
	public void ClearInput() {
		InputData = null;
	}

	/// <summary>
	/// Clears the network display
	/// </summary>
	public void ClearDisplay() {

		outputDisplay.Text = "Current Prediction %:";
		outputDisplay.Modulate = Colors.Black; 

		foreach(var child in networkWindow.GetChildren<VBoxContainer>()) {
			networkWindow.RemoveChild(child);
		}
	}

	/// <summary>
	/// Helper method that creates a 
	/// </summary>
	/// <param name="size"></param>
	/// <param name="last"></param>
	private void CreateAxonLayer(int size, bool last) {
		var vContainer = new VBoxContainer();
		vContainer.Alignment = BoxContainer.AlignMode.Center;
		networkWindow.AddChild(vContainer);

		for (int i = 0; i < size; i++) {
            
			var newNeuron = axon.Duplicate() as Axon;
			newNeuron.Bias = linker.GetAxonBias(networkWindow.GetChildCount() - 1, i);
			newNeuron.Name = (networkWindow.GetChildCount() - 1) + "_" + vContainer.GetChildCount();

            // Label last layer with classification index
            if (last) {
                newNeuron.IndexIndicator = i;
            }

			vContainer.AddChild(newNeuron);
		}
	}

	private void DisplayWeights() {
		var containerChildren = networkWindow.GetChildren();
		for (int i = 1; i < topology.Count; i++) {
			ConnectNeurons(containerChildren, i);
		}
	}

	private void ConnectNeurons(Godot.Collections.Array containerChildren, int index) {
		var containerLayer = (containerChildren[index] as Node).GetChildren();
		var prevContainerLayer = (containerChildren[index - 1] as Node).GetChildren();
		for (int i = 0; i < containerLayer.Count; i++) {
			if (containerLayer[i] is Axon axon) {
				if (i != containerLayer.Count - 1) {
					axon.Bias = linker.GetAxonBias(index, i);
				}
				for (int j = 0; j < prevContainerLayer.Count; j++) {
					var weight = linker.GetDendriteWeight(index, i, j);
					axon.ConnectToNode(prevContainerLayer[j] as Axon, weight);
				}
			}
		}
	}

	private async void RunNetwork(List<float> input, int outputIndex, int numEpochs) {
		int n = 0;
		List<float> output;
		while (n < numEpochs) {
			output = yHotEncoding(outputIndex);
			var prediction = linker.TrainNetwork(input, output);
			outputDisplay.Text = "Current Prediction %: [" + string.Join(", ", prediction) + "]\nRun: " + (n + 1); 

			var lastLayer = networkWindow.GetChildren<VBoxContainer>()?.Last()?.GetChildren<Axon>();
			for (int index = 0; index < lastLayer.Count; index++) {
				lastLayer[index].Bias = prediction[index];
			}

			if(!Training) {
				break;
			}

			await ToSignal(GetTree().CreateTimer(0.01f), "timeout");

			if (ArgMax(prediction.ToList()) == outputIndex) {
				outputDisplay.Modulate = Colors.Green;
			}
			else {
				outputDisplay.Modulate = Colors.Red;
			}

			if (DisplayWeightChange) {
				DisplayWeights();
			}
            n++;
		}
		DisplayWeights();
		ClearInput();
		Training = false;
		trainButton.Text = "Train";
	}

	private List<float> yHotEncoding(int classIndex) {
		var result = new float[topology[topology.Count - 1]].ToList();
		result[classIndex] = 1;
		return result;
	}

	private int ArgMax(List<float> values) {
		return values.IndexOf(values.Max());
	}

	private void _on_Epochs_value_changed(float value) {
		Epochs = Mathf.RoundToInt(value);
	}

	private void _on_LearningRate_value_changed(float value) {
		LearningRate = value;
	}

	private void _on_Classification_value_changed(float value) {
		Classification = Mathf.RoundToInt(value);
	}

	private async void _on_Train_pressed() {
		if (Training) {
			Training = false;
		}
		else if (InputData != null && InputData.Count > 0 && Classification >= 0 && LearningRate > 0 && Epochs > 0) {
			Training = true;
			trainButton.Text = "Stop Training";
			RunNetwork(InputData, Classification, Epochs);
		}
		else {
			errorIndicator?.Show();
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			errorIndicator?.Hide();
		}
	}

	private void _on_WeightOption_pressed() {
		DisplayWeightChange = !DisplayWeightChange;
	}

	private void _on_Export_pressed() {
		this.Get<FileDialog>("SaveDialog")?.PopupCentered(Vector2.Zero);
	}

	private void _on_New_pressed() {
		this.Get<SetupWindow>("SetupWindow")?.Show();
	}

	private void SaveWeights(string filepath) {
        linker.ExportNetwork(filepath);
	}

	public void LoadWeights(string filepath) {
        linker.ImportNetwork(filepath);
		SetUp(linker.NetworkTopology(), true);
		this.Get<SetupWindow>("SetupWindow")?.Hide();
	}

    private void _on_SaveDialog_file_selected(string filepath) {
        SaveWeights(filepath);
    }

}
