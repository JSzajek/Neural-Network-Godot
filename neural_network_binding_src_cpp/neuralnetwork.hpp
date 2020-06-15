#ifndef NEURALNETWORK_HPP
#define NEURALNETWORK_HPP

#include "neuralnetworkbase.hpp"
#include <vector>
#include <string>

using namespace std;

namespace neuralnetwork {

	class NeuralNetwork {
	public:
		vector<int> topology;
		vector<Layer*> layers;
		float learningRate;

		vector<int> getTopology() { return topology; }
		int sizeOfLayers() { return layers.size(); }
		Layer* getLayerAt(int index) { return layers.at(index); }

		virtual bool Run(vector<float> input) { return false; }

		virtual const vector<float> Output() { return {}; }

		virtual const vector<float> Predict(vector<float> input) { return {}; }

		virtual const vector<float> Train(vector<float> input, vector<float> output) { return {}; }

		virtual const string toString() { return "Base Neural Network"; }

		inline float sigmoid(float x) {
			return (1.0f) / (1.0f + exp(-x));
		}
	};

}

#endif // !NEURALNETWORK_HPP
