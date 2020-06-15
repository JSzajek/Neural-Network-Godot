#include "pch.h"
#include "neuralnetworklinker.hpp"

#include <iostream>

using namespace neuralnetwork;
using namespace std;

NeuralNetwork* NeuralNetworkLinker::network;
DeepQNetwork* NeuralNetworkLinker::qNetwork;

// Linker destructor
NeuralNetworkLinker::~NeuralNetworkLinker() {
	clearNetwork();
}

// Sets up the relevent parameters involved.
void NeuralNetworkLinker::setupDenseNetwork(vector<int> topology) {
	network = new DenseNetwork(topology);
}

// Sets up the deep q learning network with the parameters involved
void NeuralNetworkLinker::setupDeepQNetwork(int stateSize, int actionSize, int memorySize, int sampleSize) {
	qNetwork = new DeepQNetwork(stateSize, actionSize, memorySize, sampleSize);
}

// Sets the learning rate of the current network
float* NeuralNetworkLinker::trainNetwork(vector<float> input, vector<float> output)
{
	const vector<float> result = network->Train(input, output);
	float* data = new float[result.size() + 1];
	data[0] = result.size();
	copy(result.begin(), result.end(), data + 1);
	return data;
}

// Pre trains the deep q learning network with starting memories
void NeuralNetworkLinker::preTrainQLearningNetwork(vector<Memory*> data) 
{
	qNetwork->PreTrainMemory(data);
}

// Adds the memory to the deep q learning temporal lobe
void NeuralNetworkLinker::addMemory(Memory* memory) {
	qNetwork->AddMemory(memory);
}

// Predicts the best possible action based on the current state and current network bias
int NeuralNetworkLinker::predictQLearning(int decayStep, vector<float> state)
{
	return qNetwork->PredictAction(decayStep, state);
}

// Trains the current deep q learning network with the passed amount of iterations
void NeuralNetworkLinker::trainQLearningNetwork(int epochs) 
{
	qNetwork->Train(epochs);
}

// Trains the network with the passed input and output array values
void NeuralNetworkLinker::setLearningRate(float lRate)
{
	if (network != NULL) {
		network->learningRate = lRate;
	}
	else if (qNetwork != NULL) {
		qNetwork->SetLearningRate(lRate);
	}
}

// Retrieves the neuron bias at the passed indices
float NeuralNetworkLinker::getAxonBias(int layerIndex, int axonIndex)
{
	return network->getLayerAt(layerIndex)->axons.at(axonIndex)->bias;
}

// Retrieves the dendrite weight at the passed indices
float NeuralNetworkLinker::getDendriteWeight(int layerIndex, int neuronIndex, int dendriteIndex)
{
	return network->getLayerAt(layerIndex)->axons.at(neuronIndex)->dendrites.at(dendriteIndex)->weight;
}

// Clears the current network
void NeuralNetworkLinker::clearNetwork()
{
	if (network != NULL) {
		delete network;
		network = NULL;
	}
	else if (qNetwork != NULL) {
		delete qNetwork;
		qNetwork = NULL;
	}
}

// Retrieves the export array of the current network
float* NeuralNetworkLinker::exportNetwork()
{
	NeuralNetwork* exportingNet = NULL;
	if (network != NULL) {
		exportingNet = network;
	}
	else if (qNetwork != NULL) {
		exportingNet = qNetwork->network;
	}

	if (exportingNet == NULL) {
		return {};
	}

	vector<float> exported;
	exported.push_back(exportingNet->sizeOfLayers());
	for (int i = 0; i < exportingNet->sizeOfLayers(); i++) {
		Layer* layer = exportingNet->getLayerAt(i);
		exported.push_back(layer->axons.size());
		for (int j = 0; j < layer->axons.size(); j++) {
			Axon* axon = layer->axons.at(j);
			exported.push_back(axon->bias);
			exported.push_back(axon->delta);
			exported.push_back(axon->value);
			exported.push_back(axon->dendrites.size());
			for (int k = 0; k < axon->dendrites.size(); k++) {
				Dendrite* dendrite = axon->dendrites.at(k);
				exported.push_back(dendrite->weight);
			}
		}
	}
	
	float* data = new float[exported.size() + 1];
	data[0] = exported.size();
	copy(exported.begin(), exported.end(), data + 1);
	return data;
}

// Imports a network based on the passed array
void NeuralNetworkLinker::importNetwork(vector<float> data)
{
	clearNetwork();

	// TODO: With different types add indicator to type of network
	network = new DenseNetwork();
	int x = 1;
	int layerSize = data.at(x++);
	for (int i = 0; i < layerSize; i++) {
		Layer* newLayer = new Layer();
		int axonSize = data.at(x++);
		for (int j = 0; j < axonSize; j++) {
			Axon* newAxon = new Axon(data.at(x++), data.at(x++), data.at(x++));
			int dendriteSize = data.at(x++);
			for (int k = 0; k < dendriteSize; k++) {
				newAxon->dendrites.push_back(new Dendrite(data.at(x++)));
			}
			newLayer->axons.push_back(newAxon);
		}
		network->layers.push_back(newLayer);
		network->topology.push_back(newLayer->axons.size());
	}
}

// Retrieves the network topology of the current network
int* NeuralNetworkLinker::networkTopology() {
	int* data = new int[network->topology.size() + 1];
	data[0] = network->topology.size();
	copy(network->topology.begin(), network->topology.end(), data + 1);
	return data;
}