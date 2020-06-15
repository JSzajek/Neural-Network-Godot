#include "pch.h"
#include "nativeneuralnetwork.h"
#include "neuralnetworklinker.hpp"
#include <tuple>

using namespace neuralnetwork;
using namespace std;

void native_lib_init()
{
	// Implement initializer here
}

void native_lib_destroy()
{
	// Implement destroy here.
}

// Sets up the relevent parameters involved.
void setupDenseNetwork(int* layers, int d1)
{
	vector<int> topology;
	for (int i = 0; i < d1; i++) {
		topology.push_back(layers[i]);
	}
	NeuralNetworkLinker::setupDenseNetwork(topology);
}

// Sets the learning rate of the current network
void setLearningRate(float learningRate)
{
	NeuralNetworkLinker::setLearningRate(learningRate);
}

// Trains the network with the passed input and output array values
float* trainNetwork(float* inputData, int id1, float* outputData, int od1)
{
	vector<float> input;
	for (int i = 0; i < id1; i++) {
		input.push_back(inputData[i]);
	}
	vector<float> output;
	for (int i = 0; i < od1; i++) {
		output.push_back(outputData[i]);
	}
	return NeuralNetworkLinker::trainNetwork(input, output);
}

// Retrieves the neuron bias at the passed indices
float getAxonBias(int layerIndex, int axonIndex)
{
	return NeuralNetworkLinker::getAxonBias(layerIndex, axonIndex);
}

// Retrieves the dendrite weight at the passed indices
float getDendriteWeight(int layerIndex, int axonIndex, int dendriteIndex)
{
	return NeuralNetworkLinker::getDendriteWeight(layerIndex, axonIndex, dendriteIndex);
}

// Clears the current network
void clearNetwork()
{
	NeuralNetworkLinker::clearNetwork();
}

// Retrieves the export array of the current network
float* exportNetwork()
{
	return NeuralNetworkLinker::exportNetwork();
}

// Imports a network based on the passed array
void importNetwork(float* data, int d1)
{
	vector<float> importData;
	for (int i = 0; i < d1; i++) {
		importData.push_back(data[i]);
	}
	NeuralNetworkLinker::importNetwork(importData);
}

// Retrieves the network topology of the current network
int* networkTopology() {
	return NeuralNetworkLinker::networkTopology();
}

// Sets up the deep q learning network
void setupDeepQLearning(int stateSize, int actionSize, int memorySize, int sampleSize)
{
	NeuralNetworkLinker::setupDeepQNetwork(stateSize, actionSize, memorySize, sampleSize);
}

// Pre trains the deep q learning network with starting memories
void preTrainNetwork(float* data, int d1, int d2)
{
	// convert float data into list of memories
	// Memory format: [state... , action, reward, next_state... , done]
	int stateSize = (d2 - 3) / 2;

	vector<Memory*> memories;
	for (int i = 0; i < d1; i++) {
		vector<float> state;
		int j = 0;
		for (; j < stateSize; j++) {
			state.push_back(data[(i*d2) + j]);
		}
		int action = data[(i * d2) + j++];
		int reward = data[(i * d2) + j++];

		vector<float> nextState;
		int k = j;
		for (; k < j + stateSize; k++) {
			nextState.push_back(data[(i * d2) + k]);
		}
		bool done = data[(i * d2) + k] == 1;
		memories.push_back(new Memory(state, action, reward, nextState, done));
	}
	NeuralNetworkLinker::preTrainQLearningNetwork(memories);
}

// Adds the memory to the deep q learning temporal lobe
void addMemory(float* data, int d1, int stateSize) {
	vector<float> state;
	int j = 0;
	for (; j < stateSize; j++) {
		state.push_back(data[j]);
	}
	int action = data[j++];
	int reward = data[j++];

	vector<float> nextState;
	int k = j;
	for (; k < j + stateSize; k++) {
		nextState.push_back(data[k]);
	}
	bool done = data[k] == 1;
	NeuralNetworkLinker::addMemory(new Memory(state, action, reward, nextState, done));
}

// Predicts the best possible action based on the current state and current network bias
int predictAction(int decayStep, float* state, int d1)
{
	vector<float> data;
	for (int i = 0; i < d1; i++) {
		data.push_back(state[i]);
	}
	return NeuralNetworkLinker::predictQLearning(decayStep, data);
}

// Trains the current deep q learning network with the passed amount of iterations
void trainQLearningNetwork(int epochs)
{
	NeuralNetworkLinker::trainQLearningNetwork(epochs);
}