#pragma once

#ifdef NATIVENEURALNETWORK_H
#define NATIVENEURALNETWORK_H __declspec(dllimport)
#else
#define NATIVENEURALNETWORK_H __declspec(dllimport)

// Initialize the native class.
extern "C" NATIVENEURALNETWORK_H void native_lib_init();

// Destructor for the native class.
extern "C" NATIVENEURALNETWORK_H void native_lib_destroy();

// Sets up the relevent parameters involved in dense network.
extern "C" NATIVENEURALNETWORK_H void setupDenseNetwork(int* layers, int d1);

// Sets the learning rate of the current network
extern "C" NATIVENEURALNETWORK_H void setLearningRate(float learningRate);

// Trains the network with the passed input and output array values
extern "C" NATIVENEURALNETWORK_H float* trainNetwork(float* input, int id1, float* output, int od1);

// Retrieves the neuron bias at the passed indices
extern "C" NATIVENEURALNETWORK_H float getAxonBias(int layerIndex, int axonIndex);

// Retrieves the dendrite weight at the passed indices
extern "C" NATIVENEURALNETWORK_H float getDendriteWeight(int layerIndex, int axonIndex, int dendriteIndex);

// Clears the current network
extern "C" NATIVENEURALNETWORK_H void clearNetwork();

// Retrieves the export array of the current network
extern "C" NATIVENEURALNETWORK_H float* exportNetwork();

// Imports a network based on the passed array
extern "C" NATIVENEURALNETWORK_H void importNetwork(float* data, int d1);

// Retrieves the network topology of the current network
extern "C" NATIVENEURALNETWORK_H int* networkTopology();

// Sets up the relevent parameters involved in deep q learning
extern "C" NATIVENEURALNETWORK_H void setupDeepQLearning(int stateSize, int actionSize, int memorySize, int sampleSize);

// Pretrains the deep q learning network
extern "C" NATIVENEURALNETWORK_H void preTrainNetwork(float* data, int d1, int d2);

// Add memory to the deep q learning network
extern "C" NATIVENEURALNETWORK_H void addMemory(float* data, int d1, int stateSize);

// Predicts the best next action based on the q learning network
extern "C" NATIVENEURALNETWORK_H int predictAction(int decayStep, float* state, int d1);

// Trains the current q learning network based on its current memories
extern "C" NATIVENEURALNETWORK_H void trainQLearningNetwork(int epochs);

#endif // NATIVENEURALNETWORK_H
