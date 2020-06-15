#include "pch.h"
#include "densenetwork.hpp"
#include <iostream>

using namespace neuralnetwork;
using namespace std;

// Blank constructor
DenseNetwork::DenseNetwork() {
}

// Constructor initializing the dense network based on the passed topology
DenseNetwork::DenseNetwork(vector<int> topology)
{
	this->topology = topology;

	if (topology.size() < 2) {
		return;
	}

	for (int i = 0; i < topology.size(); i++) {
		int layerValue = topology.at(i);
		Layer* layer = new Layer();
		for (int j = 0; j < layerValue; j++) {
			Axon* axon = new Axon();
			if (i == 0) {
				axon->bias = 0;
			}
			else {
				for (int k = 0; k < topology.at(i - 1); k++) {
					axon->dendrites.push_back(new Dendrite());
				}
			}
			layer->axons.push_back(axon);
		}
		layers.push_back(layer);
	}
}

// Destructor
DenseNetwork::~DenseNetwork() {
	for (int i = 0; i < layers.size(); i++) {
		Layer *l = layers.at(i);
		for (int j = 0; j < l->axons.size(); j++) {
			Axon* a = l->axons.at(j);
			for (int k = 0; k < a->dendrites.size(); k++) {
				delete a->dendrites.at(k);
			}
			delete a;
		}
		delete l;
	}
}

// Runs the network and returns whether run was successful
bool DenseNetwork::Run(vector<float> input) {
	
	if (input.size() != (layers.at(0)->axons.size())) {
		return false;
	}

	for (int i = 0; i < layers.size(); i++) {
		Layer* layer = layers.at(i);
		for (int j = 0; j < layer->axons.size(); j++) {
			Axon* axon = layer->axons.at(j);
			if (i == 0) {
				axon->value = input.at(j);
			}
			else {
				axon->value = 0;
				for (int k = 0; k < layers.at(i - 1)->axons.size(); k++) {
					axon->value = (axon->value + layers.at(i - 1)->axons.at(k)->value * axon->dendrites.at(k)->weight);
				}
				axon->value = sigmoid(axon->value + axon->bias);
			}
		}
	}
	return true;
}

// Retrieves the output or prediction of the network given the input
const vector<float> DenseNetwork::Output() {
	vector<float> result;
	Layer* last = layers.back();
	for (int i = 0; i < last->axons.size(); i++) {
		result.push_back(last->axons.at(i)->value);
	}
	return result;
}

const vector<float> DenseNetwork::Predict(vector<float> input) {
	if (!Run(input)) { return {}; }
	return Output();
}

// Trains the network with backwards propogation with the given input and desired output
const vector<float> DenseNetwork::Train(vector<float> input, vector<float> output) {

	if (input.size() != layers.at(0)->axons.size() ||
		output.size() != layers.back()->axons.size()) {
		return {}; // Inconsistent Input or Output
	}

	if (!Run(input)) { return {}; }

	// Loop over all output axons
	for (int i = 0; i < layers.back()->axons.size(); i++) {
		Axon* axon = layers.back()->axons.at(i);
		axon->delta = axon->value * (1 - axon->value) * (output.at(i) - axon->value);
	}

	// Loop over all layers in reverse
	for (int i = layers.size() - 1; i > 0; i--) {
		Layer* actualLayer = layers.at(i);
		Layer* nextLayer = layers.at(i - 1);
		
		for (int j = 0; j < actualLayer->axons.size(); j++) {
			Axon* axon = actualLayer->axons.at(j);
			for (int k = 0; k < axon->dendrites.size(); k++) {
				Axon* toChangeDelta = nextLayer->axons.at(k);
				toChangeDelta->delta = (toChangeDelta->delta + (toChangeDelta->value
										* (1 - toChangeDelta->value) * axon->dendrites.at(k)->weight 
											* axon->delta));
			}
		}
	}

	for (int i = layers.size() - 1; i > 0; i--) {
		Layer* layer = layers.at(i);
		for (int j = 0; j < layer->axons.size(); j++) {
			Axon* axon = layer->axons.at(j);
			axon->bias = (axon->bias + (learningRate * axon->delta));
			for (int k = 0; k < axon->dendrites.size(); k++) {
				Dendrite* dendrite = axon->dendrites.at(k);
				dendrite->weight = (dendrite->weight + (learningRate * layers.at(i - 1)->axons.at(k)->value * axon->delta));
			}
		}
	}
	return Output();
}

// To String method used for debugging
const string DenseNetwork::toString() {
	string returnString = "Dense Network:\n";
	for (int i = 0; i < sizeOfLayers(); i++)
	{
		Layer l = *getLayerAt(i);
		returnString += "Layer: " + to_string(i) + "\n";
		returnString += "Axon count: " + to_string(l.axons.size()) + "\n";
		for (int j = 0; j < l.axons.size(); j++)
		{
			Axon axon = *l.axons.at(j);
			returnString += "Axon: " + to_string(j) + "\n";
			returnString += "Bias " + to_string(axon.bias);
			returnString += " Value " + to_string(axon.value);
			returnString += " Delta " + to_string(axon.delta) + "\n";
			for (int k = 0; k < axon.dendrites.size(); k++)
			{
				Dendrite dendrite = *axon.dendrites.at(k);
				returnString += "Dendrite: " + to_string(k) + " Weight " + to_string(dendrite.weight) + "\n";
			}
		}
		returnString += "\n";
	}
	return returnString;
}







