using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour {

	public GameObject Prefab;
	public GameObject GraphPrefab;

	private GameObject _graphPositioner;

	public void AddGraphButtonHandler()
	{
		// First try get the data provider, before doing any more work
		GameObject providerObject = GameObject.Find("DataProvider");
		if (providerObject == null) {
			Debug.Log ("Unable to find the game object named data provider");
			return;
		}

		IDataProvider dataProvider = providerObject.GetComponent<DataProvider> ();

		if (dataProvider == null) {
			Debug.Log ("Unable to get the data provider");
			return;
		}

		Vector3 cameraPosition = Camera.main.transform.position;
		Quaternion orientation = Camera.main.transform.rotation;

		// Create a new axis for placing a graph
		_graphPositioner = Instantiate (Prefab) as GameObject;
		_graphPositioner.name = "GraphPositioner";
		_graphPositioner.SetActive (true); // Make sure it is turned on

		// Pass the data provider to the positioner, to give to the graph
		BaseAxisModel positionerScript = _graphPositioner.GetComponent<BaseAxisModel> ();
		if (positionerScript == null) {
			Debug.Log ("Unable to get the BaseAxisModel script from the positioner");
			return;
		}
		positionerScript.SetGraphType (GraphPrefab);

		IList<ITraceDescriptor> availableTraces = dataProvider.AvailableTraces;
	}
}
