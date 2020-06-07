using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using File = System.IO.File;

/// <summary>
/// NeuralNetworkLinker dll class representing an 
/// library for NeuralNetwork functionality.
/// </summary>
public class NeuralNetworkLinker : Dll {
	
	#region Delegates

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void setupDenseNetwork([In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I4)] int[] points, int d1);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void setLearningRate(float learningRate);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate IntPtr trainNetwork([In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[] input, int id1, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[] output, int od1);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate float getAxonBias(int layerIndex, int axonIndex);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate float getDendriteWeight(int layerIndex, int axonIndex, int dendriteIndex);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void clearNetwork();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr exportNetwork();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate IntPtr importNetwork([In][MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.R4)] float[] input, int d1);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr networkTopology();


	#endregion Delegates

	#region  Private Parameters

	private setupDenseNetwork _setupDenseNetwork;
	private setLearningRate _setLearningRate;
	private trainNetwork _trainNetwork;
	private getAxonBias _getAxonBias;
	private getDendriteWeight _getDendriteWeight;
	private clearNetwork _clearNetwork;
	private exportNetwork _exportNetwork;
	private importNetwork _importNetwork;
	private networkTopology _networkTopology;

	#endregion Private Parameters

	/// <summary>
	/// Constructor initializing delegate methods of the library
	/// </summary>
	/// <param name="filePath">The file path to the dll</param>
	/// <returns></returns>
	public NeuralNetworkLinker(string filePath) : base(filePath)
	{
		_setupDenseNetwork = pDll.GetFuncDelegate<setupDenseNetwork>("setupDenseNetwork");
		_setLearningRate = pDll.GetFuncDelegate<setLearningRate>("setLearningRate");
		_trainNetwork = pDll.GetFuncDelegate<trainNetwork>("trainNetwork");
		_getAxonBias = pDll.GetFuncDelegate<getAxonBias>("getAxonBias");
		_getDendriteWeight = pDll.GetFuncDelegate<getDendriteWeight>("getDendriteWeight");
		_clearNetwork = pDll.GetFuncDelegate<clearNetwork>("clearNetwork");
		_exportNetwork = pDll.GetFuncDelegate<exportNetwork>("exportNetwork");
		_importNetwork = pDll.GetFuncDelegate<importNetwork>("importNetwork");
		_networkTopology = pDll.GetFuncDelegate<networkTopology>("networkTopology");
	}

	/// <summary>
	/// Sets up a dense neural network with the passed topology.
	/// </summary>
	/// <param name="topology">The topology of the network</param>
	public void SetupDenseNetwork(List<int> topology) {
		_setupDenseNetwork(topology.ToArray(), topology.Count);
	}

	/// <summary>
	/// Sets the learning rate of the current network.
	/// </summary>
	/// <param name="learningRate">The new learning rate of the network</param>
	public void SetLearningRate(float learningRate) {
		_setLearningRate(learningRate);
	}

	/// <summary>
	/// Trains the current network with the passed input and desired output values.
	/// </summary>
	/// <param name="input">The input values</param>
	/// <param name="output">The desired output values</param>
	/// <returns>The current prediction of the network</returns>
	public float[] TrainNetwork(List<float> input, List<float> output) {
		IntPtr resultPtr = _trainNetwork(input.ToArray(), input.Count, output.ToArray(), output.Count);
		
		float[] sizeArray = new float[1];
		Marshal.Copy(resultPtr, sizeArray, 0, 1);
		int size = (int)sizeArray[0];

		if (size == 1) {
			return new float[0]; // Should never happen
		}

		float[] outputVals = new float[size + 1];
		Marshal.Copy(resultPtr, outputVals, 0, size + 1);
		return outputVals.Skip(1).Take(size).ToArray();
	}

	/// <summary>
	/// Retieves the axon bias at the passed indices (for visualization)
	/// </summary>
	/// <param name="layerIndex">The layers index</param>
	/// <param name="axonIndex">The axons index</param>
	/// <returns></returns>
	public float GetAxonBias(int layerIndex, int axonIndex) {
		return _getAxonBias(layerIndex, axonIndex);
	}

	/// <summary>
	/// Retrieves the dendrite weight at the passed indices (for visualization)
	/// </summary>
	/// <param name="layerIndex">The layers index</param>
	/// <param name="axonIndex">The axons index</param>
	/// <param name="dendriteIndex">The dendrites index</param>
	/// <returns>The dendrite weight</returns>
	public float GetDendriteWeight(int layerIndex, int axonIndex, int dendriteIndex) {
		return _getDendriteWeight(layerIndex, axonIndex, dendriteIndex);
	}

	/// <summary>
	/// Clears the current network
	/// </summary>
	public void ClearNetwork() {
		_clearNetwork();
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

	/// <summary>
	/// Retrieves the current network topology
	/// </summary>
	/// <returns>The current network topology</returns>
	public List<int> NetworkTopology() {
		IntPtr resultPtr = _networkTopology();
		
		int[] sizeArray = new int[1];
		Marshal.Copy(resultPtr, sizeArray, 0, 1);
		int size = (int)sizeArray[0];

		if (size == 1) {
			return null;
		}

		int[] topology = new int[size + 1];
		Marshal.Copy(resultPtr, topology, 0, size + 1);
		return topology.Skip(1).Take(size).ToList();
	}
}