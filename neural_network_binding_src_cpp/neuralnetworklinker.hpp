#pragma once

#ifndef NEURALNETWORKLINKER_HPP
#define NEURALNETWORKLINKER_HPP

#include "densenetwork.hpp"
#include "deepqnetwork.hpp"
#include <vector>

using namespace std;

namespace neuralnetwork 
{
	class NeuralNetworkLinker
	{
	private:
		static NeuralNetwork* network;
		static DeepQNetwork* qNetwork;
	public:
		~NeuralNetworkLinker();
		static void setupDenseNetwork(vector<int> topology);
		static void setupDeepQNetwork(int stateSize, int actionSize, int memorySize, int sampleSize);

		static void setLearningRate(float lRate);
		static float* trainNetwork(vector<float> input, vector<float> output);
		static void preTrainQLearningNetwork(vector<Memory*> data);
		static void addMemory(Memory* memory);
		static int predictQLearning(int decayStep, vector<float> state);
		static void trainQLearningNetwork(int epochs);

		static float getAxonBias(int layerIndex, int axonIndex);
		static float getDendriteWeight(int layerIndex, int axonIndex, int dendriteIndex);

		static void clearNetwork();
		static float* exportNetwork();
		static void importNetwork(vector<float> data);
		static int* networkTopology();
	};
}

#endif // !NEURALNETWORKLINKER_HPP
