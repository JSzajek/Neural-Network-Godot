#ifndef MEMORY_HPP
#define MEMORY_HPP

#include <vector>

namespace std {
	/// <summary>
	/// Memory class representing a memory that consist of the current state
	/// the action that brought about the next state and the rewarding outcome
	/// </summary>
	class Memory {
	public:
		vector<float> state;
		int action;
		int reward;
		vector<float> next;
		bool done;
		
		Memory(vector<float> state, int action, int reward, vector<float> next, bool done) :
				state(state), action(action), reward(reward), next(next), done(done){
		}
	};
}


#endif // !MEMORY_HPP
