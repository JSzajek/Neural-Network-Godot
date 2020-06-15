#include "pch.h"
#include "deepqnetwork.hpp"

using namespace neuralnetwork;
using namespace std;

// Constructor of the deep q network
DeepQNetwork::DeepQNetwork(int stateSize, int actionSize, int memorySize, int sampleSize) 
	: actionSize(actionSize), stateSize(stateSize), sampleSize(sampleSize)
{
	network = new DenseNetwork({ stateSize, stateSize, stateSize, actionSize });
	temporal = new Temporal(memorySize);
}

// Destructor of the deep q network
DeepQNetwork::~DeepQNetwork() 
{
	delete network;
	delete temporal;
}

// Pre trains the deep q learning network with starting memories
void DeepQNetwork::SetLearningRate(float learningRate) {
	network->learningRate = learningRate;
}

void DeepQNetwork::PreTrainMemory(vector<Memory*> states) 
{
	// Add each of the starting memory into the temporal lobe
	for (Memory* memory : states) {
		temporal->Add(memory);
	}
}

// Adds the memory to the deep q learning temporal lobe
void DeepQNetwork::AddMemory(Memory* memory)
{
	temporal->Add(memory);
}

// Predicts the best possible action based on the current state and current network bias
int DeepQNetwork::PredictAction(int decayStep, vector<float> state)
{
	float expTradeOff = RandRange(0, 0.999);
	float exploreProbability = exploreStart + (exploreStart - exploreStop) * expf(-decayRate * decayStep);

	// Exploration
	if (exploreProbability > expTradeOff) {
		return RandRangeInt(0, actionSize - 1);
	}
	//Explotation
	else {
		return ArgMax(network->Predict(state));
	}
}

// Trains the current deep q learning network with the passed amount of iterations
void DeepQNetwork::Train(int epochs) 
{
	vector<Memory*> samples = temporal->Sample(sampleSize);
	vector<vector<float>> targetQBatch;
	vector<vector<float>> states;

	for (Memory* s : samples) {
		states.push_back(s->state);
		vector<float> oldStatesQ = network->Predict(s->state);
		if (s->done) {
			oldStatesQ[s->action] = s->reward;
		}
		else {
			vector<float> newStateQ = network->Predict(s->next);
			oldStatesQ[s->action] = s->reward + (gamma * (*max_element(s->next.begin(), s->next.end())));
		}
		targetQBatch.push_back(oldStatesQ);
	}
	
	// Batch training
	while (epochs > 0) {
		for (int i = 0; i < states.size(); i++) {
			network->Train(states.at(i), targetQBatch.at(i));
		}
		epochs--;
	}
}