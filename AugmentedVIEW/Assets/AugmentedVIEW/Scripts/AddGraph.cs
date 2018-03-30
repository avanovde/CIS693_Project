using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour {

	public GameObject Prefab;
	public GameObject GraphPrefab;

	private GameObject _graphPositioner;

	public void AddGraphButtonHandler()
	{
		Vector3 cameraPosition = Camera.main.transform.position;
		Quaternion orientation = Camera.main.transform.rotation;

		// Create a new axis for placing a graph
		_graphPositioner = Instantiate (Prefab) as GameObject;
		_graphPositioner.name = "Positioner";
		_graphPositioner.SetActive (true); // Make sure it is turned on

		// Pass the data provider to the positioner, to give to the graph
		GraphPositioner positionerScript = _graphPositioner.GetComponent<GraphPositioner> ();
		if (positionerScript == null) {
			Debug.Log ("Unable to get the BaseAxisModel script from the positioner");
			return;
		}
		positionerScript.GraphPrefab = GraphPrefab;
	}
}
