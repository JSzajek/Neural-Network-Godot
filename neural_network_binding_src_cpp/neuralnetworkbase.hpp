#ifndef NEURALNETWORKBASE_HPP
#define NEURALNETWORKBASE_HPP

#include <math.h>
#include <vector>
#include <time.h>
#include <algorithm>

using namespace std;

namespace neuralnetwork {
	
	inline float RandRange(float a, float b) {
		return a + static_cast <float> (rand()) / (static_cast <float> (RAND_MAX / (b - a)));
	}

	inline int RandRangeInt(int min, int max) {
		return min + (rand() % static_cast<int>(max - min + 1));
	}

	inline int ArgMax(vector<float> data) {
		return distance(data.begin(), max_element(data.begin(), data.end()));
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
