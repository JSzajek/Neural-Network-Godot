#ifndef NEURALNETWORKBASE_HPP
#define NEURALNETWORKBASE_HPP

#include <math.h>
#include <vector>
#include <time.h>

using namespace std;

namespace neuralnetwork {
	
	inline float RandRange(float a, float b) {
		return a + static_cast <float> (rand()) / (static_cast <float> (RAND_MAX / (b - a)));
	}
	
	// Struct representing a drendrite
	struct Dendrite {
	public:
		float weight;
		Dendrite() : weight(RandRange(-0.5, 0.5)) { }
		Dendrite(float val) : weight(val) { }
	};

	// Struct representing an axon with its connection dendrites
	struct Axon {
	public:
		vector<Dendrite*> dendrites;
		float bias;
		float delta;
		float value;
		Axon() : bias(RandRange(-0.5, 0.5)), delta(0), value(0) { }

		// Reverse due to operation ordering
		Axon(float value, float delta, float bias) : bias(bias), delta(delta), value(value) { }
	};

	// Struct representing a layer with its axons
	struct Layer {
	public:
		vector<Axon*> axons;
		Layer() {
			srand(static_cast <unsigned> (time(0))); // Assure randomization between layers
		}
	};

	
}

#endif // !NEURALNETWORKBASE_HPP
