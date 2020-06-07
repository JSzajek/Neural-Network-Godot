# Neural Network Visualizer-Godot #
An Neural Network Visualizer in Godot game engine.

This project was done in order to gain a better understanding of neural networks.
As well as utilizing Godot's C++ bindings for better performance. 

### Capabilities / Advantages: ###
* Variability - Create a variable dense neural network
* Visualization - Connection between neurons within a neural network
    * Bias is displayed on each neuron.
    * Negative or positive weight value is displayed as red or blue lines respectively.
    * Input value on input layer and output value on output layer.
* Import/Export - The neural network weights in unique .net/.network file format

### Task List: ###
- [x] Move core functionality to native C++ bindings.
- [ ] Add convolutional layers.
- [ ] Improve styling of UI.
- [ ] Improve line grouping.
- [ ] Use matrices for increased efficiency

#### Detailed Description ####
The main scence demonstrates these key features within only a few steps. 
1. Set up layers of neural network through dials
2. Click and drag to draw a custom image and input data
3. Input valid number of training epochs, learning rate, and class index.
4. Begin training.

#### Included ####
* Primary Scripts, Assets, Scenes
* Built Executable Project

#### Network Setup Example ####
![Alt-Text](/Network_Setup_Example.png)

#### Network Training Example ####
![Alt-Text](/Network_Training_Example.png)