# Neural Network Visualizer-Godot #
An Neural Network Visualizer in Godot game engine.

This is an rework of the [previous attempt](https://gitlab.com/jszajek/neural-network-visualizer-godot) at neural network visualization within the 
Godot game engine - Version 3.2 mono. It utilizes C# over gdscript along with loaded C++ libraries containing neural network algorithm. In attempts
to bring a form of neural networks in the Godot game engine. Furthermore, the compiled dlls have been made in such a way that they can be utilized in 
other game engines as well (eg. Unity).

**Disclaimer:** All implementations refernce their relevent sources. As well as none of these implementations claim to be the
most efficent implementation - probably more efficent implementations within the engine. 
But these implementations are done in an exercise to learn about the involved algorithms. 

### Capabilities / Advantages: ###
* Variability - Create a variable dense neural network
* Visualization - Connection between neurons within a neural network
    * Bias is displayed on each visual node.
    * Negative or positive weight value is displayed as red or blue lines respectively.
    * Input value on input layer and output value on output layer.
* Import/Export - The neural network weights in unique .net/.network (binary file) format

### Task List: ###
- [x] Move core functionality to native C++ bindings.
- [ ] Add convolutional neural network.
- [x] Add deep q-learning.
- [ ] Add importing option on q learning network.
- [ ] Include memories in export of q learning network.
- [ ] Improve styling of UI.
- [ ] Use matrices for increased efficiency.

#### Detailed Description ####
The dense network scence demonstrates these key features within only a few steps. 
1. Set up layers of neural network through dials
2. Click and drag to draw a custom image and input data
3. Input valid number of training epochs, learning rate, and class index.
4. Begin training.

The q learning network scene demonstrates these key features almost immediately
1. Pressing the start button
2. After pretraining of the memory storage, the training will begin and avoidance of obstacles will increase

#### Included ####
* Primary Scripts, Assets, Imports, Scenes
* Built Executable Project ([here](/_Build/Build_1.1/))

#### Network Setup Example ####
![Alt-Text](/Network_Setup_Example.png)

#### Network Training Example ####
![Alt-Text](/Network_Training.gif)

#### Deep Q Learning Example ####
![Alt-Text](/DeepQTraining_90.gif)

The above image demonstrates the Q learning network after 90 episodes such that obstacle avoidance is improved in comparison to the beginning exploration stages.