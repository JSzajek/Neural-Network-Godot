#ifndef TEMPORAL_HPP
#define TEMPORAL_HPP

#include "memory.hpp"
#include <vector>
#include <unordered_set>

using namespace neuralnetwork;

namespace std { 
	/// <summary>
	/// Temporal lobe class representing memory storage of past 
	/// states and outcomes
	/// </summary>
	class Temporal {
	private:
		vector<Memory*>* memory;
	public:
		Temporal(int memorySize) {
			memory = new vector<Memory*>();
		}
		void Add(Memory* data) {
			memory->push_back(data);
		}

		const vector<Memory*> Sample(int size) {
			if (size >= memory->size()) {
				return vector<Memory*>(*memory);
			}
			
			vector<Memory*> samples;
			while (samples.size() < size) {
				int randIndex = RandRangeInt(0, memory->size() - 1);
				samples.push_back(memory->at(randIndex));
			}
			return samples;
		}
	};
}

#endif // !TEMPORAL_HPP
