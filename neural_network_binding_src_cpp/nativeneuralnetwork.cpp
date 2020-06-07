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