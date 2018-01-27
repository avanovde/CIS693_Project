using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour {

	public GameObject GraphManagerObject;
	public void AddGraphButtonHandler()
	{
		GraphManager graphManager = GraphManagerObject.GetComponent<GraphManager> ();

		if (graphManager == null) {
			Debug.Log ("Unable to retrieve the graph manager from the scene");
			return;
		}

		graphManager.AddNewGraph ();
	}
}
