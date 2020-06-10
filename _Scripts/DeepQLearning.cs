using Godot;
using System;
using System.Collections.Generic;

public class DeepQLearning : Node
{
    public bool Training {get; set;}

    public int Episodes { get; set; }

    public int MaxSteps {get; set;}    

    public int TrainingEpochs {get; set;}

    public int PreTrainEpisodes {get; set;}

    public int MemorySize {get; set;} = 2000;
    private int SampleSize {get; set;}

    private int stateSize = 4;
    private int actionSize = 5;

    private float LearningRate = 0.00025f;
    private float gamma = 0.9f; // Discounting rate

    private float exploreStart = 1f;
    private float exploreStop = 0.01f;
    private float decayRate = 0.00001f; // Exponential decay rate for exploration probability

    private DllLoader loader = new DllLoader();
    private NeuralNetworkLinker linker;
    private DeepQLearningCanvas canvas;


    public override void _Ready()
    {
        canvas = this.Get<DeepQLearningCanvas>("Canvas");
        linker = loader.GetNeuralNetworkLinker();

        // Set up the brain

        linker.SetupDenseNetwork(new List<int> {stateSize, stateSize, stateSize, stateSize, actionSize});

        // Set up the memory

        SampleSize = PreTrainEpisodes;
    }

    public void UpdateHealth(int health) {
        canvas?.SetHealth(health);
        if (health == 0) {
            EndGame();
        }
    }

    public void EndGame() {
    
    }
}
