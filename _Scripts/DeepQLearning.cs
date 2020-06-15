using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DeepQLearning : Node
{
    [Export]
    public bool Training {get; set;}

    [Export]
    public int Episodes { get; set; }

    [Export]
    public int MaxSteps {get; set;}    

    [Export]
    public int TrainingEpochs {get; set;}

    [Export]
    public int PreTrainEpisodes {get; set;}
    [Export]
    private int SampleSize {get; set;}

    [Export]
    public PackedScene trainingWorld { get; set; }

    

    private Spatial instancedWorld;
    private Spatial world;

    public int MemorySize {get; set;} = 2000;

    private int stateSize = 12;
    private int actionSize = 3;

    private float LearningRate = 0.00025f;
    private float gamma = 0.9f; // Discounting rate

    private float exploreStart = 1f;
    private float exploreStop = 0.01f;
    private float decayRate = 0.00001f; // Exponential decay rate for exploration probability
    private bool pretrained = false;
    private string exportPath = null;

    private DllLoader loader = new DllLoader();
    private DeepQLinker linker;
    private DeepQLearningCanvas canvas;

    public override void _Ready()
    {
        canvas = this.Get<DeepQLearningCanvas>("Canvas");
        world = this.Get<Spatial>("World");

        world.RemoveAllChildren<Spatial>();
        instancedWorld = trainingWorld.Instance() as Spatial;       
        
        linker = loader.GetDeepQLinker();
        linker.setupDeepQ(stateSize, actionSize, MemorySize, SampleSize);
        linker.SetLearningRate(LearningRate);  
    }

    public async void StartTraining() {
        if (!pretrained) {
            linker.PreTrainMemory(await PreTrainMemory());
        }
        Train();
    }

    private async void Train() {
        List<float> state = new List<float>(new float[stateSize]);
        int reward;

        for (int i = 0; i < Episodes; i++) {
            var step = 0;
            var decayStep = 0;

            var currentWorld = instancedWorld.Duplicate() as Spatial;
            var currentPlayer = currentWorld.Get<Log>("Log");
            world.AddChild(currentWorld);
            
            canvas.SetEpisode(i);
            if (i == 0) {
                state = currentPlayer.getCurrentState();
            }

            while(step < MaxSteps) {
                step++;
                decayStep++;

                if(exportPath != null) {
                    linker.ExportNetwork(exportPath);
                    exportPath = null;
                    await ToSignal(GetTree(), "idle_frame");
                }

                var predictedAction = linker.PredictAction(decayStep, state);

                List<float> nextState;
                bool done;
                reward = -10;

                currentPlayer.performAction(predictedAction);
                await ToSignal(GetTree(), "idle_frame");
                await ToSignal(GetTree(), "idle_frame");
                await ToSignal(GetTree(), "idle_frame");
                
                var values = currentPlayer.getCurrentValue();
                reward += values[0];
                done = values[1] <= 0;

                nextState = currentPlayer.getCurrentState();

                if (done) {
                    nextState = new List<float>(new float[stateSize]);

                    // Add memory
                    linker.AddMemory(ConvertToMemory(state, predictedAction, reward, nextState, done), stateSize);
                }
                else {
                    // Add memory
                    linker.AddMemory(ConvertToMemory(state, predictedAction, reward, nextState, done), stateSize);
                    state = nextState;
                }

                linker.TrainNetwork(TrainingEpochs);

                if (done) { break; }
            }
            world.RemoveChild(currentWorld);
        }
        linker.ExportNetwork("C://Users/Justin/Documents/Godot Projects/Neural Network Visualizer/finished.net");
    }

    private async Task<List<List<float>>> PreTrainMemory() 
    {
        var memories = new List<List<float>>();
        List<float> state = new List<float>(new float[stateSize]);
        int action;
        int reward;

        var currentWorld = instancedWorld.Duplicate() as Spatial;
        var currentPlayer = currentWorld.Get<Log>("Log");
        world.AddChild(currentWorld);

        
        for (int i = 0; i < PreTrainEpisodes; i++) {
            canvas.SetEpisode(i, true);
            if (i == 0) {
                state = currentPlayer.getCurrentState();
            }

            reward = -10;
            action = (int)((long)GD.Randi() % actionSize);
            currentPlayer.performAction(action);

            await ToSignal(GetTree(), "idle_frame");
            await ToSignal(GetTree(), "idle_frame");
            await ToSignal(GetTree(), "idle_frame");

            var values = currentPlayer.getCurrentValue();
            reward += values[0];
            var done = values[1] <= 0;
            var nextState = currentPlayer.getCurrentState();

            if (done) {
                nextState = new List<float>(new float[stateSize]);

                // Add memory
                memories.Add(ConvertToMemory(state, action, reward, nextState, done));

                world.RemoveChild(currentWorld);
                currentWorld = instancedWorld.Duplicate() as Spatial;
                currentPlayer = currentWorld.Get<Log>("Log");
                world.AddChild(currentWorld);
            }
            else {
                // Add memory
                memories.Add(ConvertToMemory(state, action, reward, nextState, done));
                state = nextState;
            }
        }
        world.RemoveChild(currentWorld);
        pretrained = true;
        return memories;
    }

    private List<float> ConvertToMemory(List<float> state, int action, int reward, List<float> nextState, bool done) {
        var memory = new List<float>();
        memory.AddRange(state);
        memory.Add(action);
        memory.Add(reward);
        memory.AddRange(nextState);
        memory.Add(done ? 1 : 0);
        return memory;
    }

    public void ExportNetwork(string path) {
        exportPath = path;
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
