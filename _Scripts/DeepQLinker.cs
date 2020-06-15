using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using File = System.IO.File;

/// <summary>
/// Deep-Q-Learning dll class representing an 
/// library for NeuralNetwork functionality.
/// </summary>
public class DeepQLinker : Dll {
	
	#region Delegates

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void setupDeepQLearning(int statesSize, int actionSize, int memorySize, int sampleSize);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void setLearningRate(float learningRate);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void preTrainNetwork([In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[,] input, int d1, int d2);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int addMemory([In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[] input, int d1, int stateSize);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int predictAction(int decayStep, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[] input, int d1);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate IntPtr trainQNetwork(int epochs);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void clearNetwork();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr exportNetwork();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate IntPtr importNetwork([In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[] input, int d1);

	#endregion Delegates

	#region  Private Parameters

	private setupDeepQLearning _setupDeepQLearning;
	private setLearningRate _setLearningRate;
	private predictAction _predictAction;
	private preTrainNetwork _preTrainNetwork;
	private addMemory _addMemory;
	private trainQNetwork _trainQNetwork;
	private exportNetwork _exportNetwork;
	private importNetwork _importNetwork;

	#endregion Private Parameters

	/// <summary>
	/// Constructor initializing delegate methods of the library
	/// </summary>
	/// <param name="filePath">The file path to the dll</param>
	/// <returns></returns>
	public DeepQLinker(string filePath) : base(filePath)
	{
		_setupDeepQLearning = pDll.GetFuncDelegate<setupDeepQLearning>("setupDeepQLearning");
		_setLearningRate = pDll.GetFuncDelegate<setLearningRate>("setLearningRate");
		_preTrainNetwork = pDll.GetFuncDelegate<preTrainNetwork>("preTrainNetwork");
		_addMemory = pDll.GetFuncDelegate<addMemory>("addMemory");
		_predictAction = pDll.GetFuncDelegate<predictAction>("predictAction");
		_trainQNetwork = pDll.GetFuncDelegate<trainQNetwork>("trainQLearningNetwork");

		_exportNetwork = pDll.GetFuncDelegate<exportNetwork>("exportNetwork");
		_importNetwork = pDll.GetFuncDelegate<importNetwork>("importNetwork");
	}

	/// <summary>
	/// Sets up a dense neural network with the passed topology.
	/// </summary>
	/// <param name="topology">The topology of the network</param>
	public void setupDeepQ(int stateSize, int actionSize, int memorySize, int sampleSize) {
		_setupDeepQLearning(stateSize, actionSize, memorySize, sampleSize);
	}

	/// <summary>
	/// Sets the learning rate of the current network.
	/// </summary>
	/// <param name="learningRate">The new learning rate of the network</param>
	public void SetLearningRate(float learningRate) {
		_setLearningRate(learningRate);
	}

	/// <summary>
	/// Predicts the best possible action given the current state
	/// </summary>
	/// <param name="decayStep">The amount of exploration decay</param>
	/// <param name="state">The current state of the world</param>
	/// <returns>The best possible action index</returns>
	public int PredictAction(int decayStep, List<float> state) 
	{
		return _predictAction(decayStep, state.ToArray(), state.Count);
	}

	/// <summary>
	/// Pre-trains the deep Q network with some initial memories
	/// </summary>
	/// <param name="memories">The initial memories to kickstart the network</param>
	public void PreTrainMemory(List<List<float>> memories) {
		// Convert to rectangular array
		var arr = new float[memories.Count, (int)memories.FirstOrDefault()?.Count];
		for (int i = 0; i < arr.GetLength(0); i++) {
			for (int j = 0; j < arr.GetLength(1); j++) {
				arr[i,j] = memories[i][j];
			}
		}
		_preTrainNetwork(arr, arr.GetLength(0), arr.GetLength(1));
	}

	public void AddMemory(List<float> memory, int stateSize) {
		_addMemory(memory.ToArray(), memory.Count, stateSize);
	}

	/// <summary>
	/// Trains the current network with the passed input and desired output values.
	/// </summary>
	/// <param name="epochs">number of training epochs</param>
	public void TrainNetwork(int epochs) {
		_trainQNetwork(epochs);
	}

	/// <summary>
	/// Exports the current network to the binary file
	/// saved at the passed file path
	/// </summary>
	/// <param name="filepath">filepath of the binary file to save to</param>
	public void ExportNetwork(string filepath) {
		IntPtr resultPtr = _exportNetwork();
		
		float[] sizeArray = new float[1];
		Marshal.Copy(resultPtr, sizeArray, 0, 1);
		int size = (int)sizeArray[0];

		if (size == 1) {
			return;
		}

		float[] exportVals = new float[size + 1];
		Marshal.Copy(resultPtr, exportVals, 0, size + 1);

		using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
		using (var writer = new BinaryWriter(stream))
		{
			foreach (var item in exportVals)
			{
				writer.Write(item);
			}
			writer.Close();
			stream.Close();
		}
	}

	/// <summary>
	/// Imports the network stored within the passed binary
	/// file at the file path location.
	/// </summary>
	/// <param name="filepath">The binary file path location</param>
	public void ImportNetwork(string filepath) {
		List<float> importValues = new List<float>();
        using (var filestream = File.Open(filepath, FileMode.Open))
        using (var binaryStream = new BinaryReader(filestream))
        {
			importValues.Add(binaryStream.ReadSingle());
			for(int i = 0; i < importValues.First(); i++) {
                importValues.Add(binaryStream.ReadSingle());
			}
			binaryStream.Close();
			filestream.Close();
        }
		// Assure there is atleast a layer index
		if (importValues.Count > 2) {
			_importNetwork(importValues.ToArray(), importValues.Count());
		}
	}
}