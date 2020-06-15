#ifndef DEEPQNETWORK_HPP 
#define DEEPQNETWORK_HPP

#include "neuralnetwork.hpp"
#include "densenetwork.hpp"
#include "temporal.hpp"
#include "memory.hpp"

#include <vector>

using namespace std;

namespace neuralnetwork {
	
	class DeepQNetwork {
	private:
		Temporal* temporal;

		float exploreStart = 1;
		float exploreStop = 0.01;
		float decayRate = 0.00001;
		float gamma = 0.7; //Discounting rate

		int trainingEpisode = 5;
		int sampleSize = 20;

		int actionSize;
		int stateSize;
	public:
		NeuralNetwork* network;

		DeepQNetwork(int actionSize, int stateSize, int memorySize, int sampleSize);
		~DeepQNetwork();
		void SetLearningRate(float learningRate);
		void PreTrainMemory(vector<Memory*> states);
		void AddMemory(Memory* memory);
		int PredictAction(int decayStep, vector<float> state);
		void Train(int epochs);
	};
}


#endif // !DEEPQNETWORK_HPP 
