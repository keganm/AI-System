/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// AI probability solver.
/// Returns a randomized probability for decision making
/// </summary>
public static class AIProbabilitySolver {
	static int topResults = 3;
	static float resultThreshold = 0.075f;

	//Holds value and index
	public struct ValIndex
	{
		public float val;
		public int index;

		public ValIndex(float _val, int _index)
		{
			this.val = _val;
			this.index = _index;
		}

		public override string ToString ()
		{
			return (string.Format ("{0},{1}",val,index));
		}
	}
	
	/// <summary>
	/// Returns a randomized result based on initial values.
	/// Values will be normalized to a percentage.
	/// </summary>
	/// <returns><c>true</c>, if result index was gotten, <c>false</c> otherwise.</returns>
	/// <param name="_index">_index.</param>
	/// <param name="_highestValue">_highest value.</param>
	/// <param name="_values">_values.</param>
	/// <param name="_inverse">If set to <c>true</c> _inverse.</param>
	public static bool GetResultIndex(ref int _index, float[] _values)
	{
		float _highestValue = -1f;
		return ResultIndex (ref _index, ref _highestValue, _values, false);
	}
	public static bool GetResultIndex(ref int _index, float[] _values, bool _inverse)
	{
		float _highestValue = -1f;
		return ResultIndex (ref _index, ref _highestValue, _values,_inverse);
	}
	public static bool GetResultIndex(ref int _index, ref float _highestValue, float[] _values)
	{
		return ResultIndex (ref _index, ref _highestValue, _values, false);
	}
	public static bool GetResultIndex(ref int _index, ref float _highestValue, float[] _values, bool _inverse)
	{
		return ResultIndex (ref _index, ref _highestValue, _values,_inverse);
	}

	private static bool ResultIndex(ref int _index, ref float _highestValue, float[] _values, bool _inverse)
	{
		int result = -1;
		
		if (!NormalizeValues (ref _values))
		{
			_index = result;
			return false;
		}

		if (_inverse)
						InvertNormalizedValues (ref _values);
		
		List<ValIndex> _tests = GetTopResults (_values);
		RandomizeResults (ref _tests);
		
		if(_tests.Count == 0){
			_index = result;
			return false;
		}
		
		result = _tests [0].index;
		_highestValue = _tests [0].val;
		_index = result;
		return true;
	}

	private static bool NormalizeValues(ref float[] _val)
	{
		float totalVal = 0f;
		for(int i = 0; i < _val.Length; i++)
		{
			totalVal += _val[i];
		}

		if (totalVal == 0)
						return false;

		float valScale = 1f / totalVal;

		for (int i = 0; i < _val.Length; i++) 
		{
			_val[i] *= valScale;
		}
		return true;
	}

	private static bool InvertNormalizedValues(ref float[] _val)
	{
		for(int i = 0; i < _val.Length; i++)
		{
			_val[i] = 1.0f - _val[i];
		}
		return true;
	}

	private static List<ValIndex> GetTopResults(float[] _values)
	{
		int[] prevIndex = new int[_values.Length];
		for(int i = 0; i < _values.Length; i++)
			prevIndex[i] = i;

		Array.Sort (_values, prevIndex);

		List<ValIndex> returnValues = new List<ValIndex>();

		for (int i = _values.Length - 1; i > _values.Length - 1 - topResults; i--) {
			if(i < 0){
				SortValues (ref returnValues);
				return returnValues;
			}
			if(returnValues.Count >= 1 && _values[i] < resultThreshold){
				SortValues (ref returnValues);
				return returnValues;
			}

			returnValues.Add(new ValIndex(_values[i],prevIndex[i]));
		}

		SortValues (ref returnValues);
		return returnValues;
	}


	private static bool RandomizeResults(ref List<ValIndex> topResults)
	{
		if (topResults.Count <= 0)
						return false;

		for(int i = 0; i < topResults.Count; i++)
			topResults[i] = new ValIndex(UnityEngine.Random.value * topResults[i].val, topResults[i].index);

		SortValues (ref topResults);

		return true;
	}
	
	private static void SortValues(ref List<ValIndex> toSort)
	{
		toSort = toSort.OrderByDescending (tosort => tosort.val).ToList ();
	}
}
