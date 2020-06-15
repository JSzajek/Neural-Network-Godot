#ifndef DENSENETWORK_HPP
#define DENSENETWORK_HPP

#include "neuralnetwork.hpp"

namespace neuralnetwork {
	class DenseNetwork : public NeuralNetwork {
	public:
		DenseNetwork();
		DenseNetwork(vector<int> topology);
		~DenseNetwork();

		bool Run(vector<float> input);
		const vector<float> Output();
		const vector<float> Train(vector<float> input, vector<float> output);

		const vector<float> Predict(vector<float> input);

		const string toString();
	};
}

#endif // !DENSENETWORK_HPP
