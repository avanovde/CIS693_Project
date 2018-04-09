using UnityEngine;
using System.Collections;

public class ClearGraphs : MonoBehaviour
{
	public void ClearAllGraphs()
	{
		var allGraphs = GameObject.FindGameObjectsWithTag ("Graph");
		foreach (var graph in allGraphs) {
			Destroy (graph);
		}
	}
}

