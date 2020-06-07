#pragma once

#ifndef NEURALNETWORKLINKER_HPP
#define NEURALNETWORKLINKER_HPP

#include "densenetwork.hpp"
#include <vector>

using namespace std;

namespace neuralnetwork 
{
	class NeuralNetworkLinker
	{
	private:
		static NeuralNetwork* network;
	public:
		~NeuralNetworkLinker();
		static void setupDenseNetwork(vector<int> topology);

		static void setLearningRate(float lRate);
		static float* trainNetwork(vector<float> input, vector<float> output);

		static float getAxonBias(int layerIndex, int axonIndex);
		static float getDendriteWeight(int layerIndex, int axonIndex, int dendriteIndex);

		static void clearNetwork();
		static float* exportNetwork();
		static void importNetwork(vector<float> data);
		static int* networkTopology();
	};
}

#endif // !NEURALNETWORKLINKER_HPP
