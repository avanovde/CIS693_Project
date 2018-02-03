using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour {

	public GameObject GraphPositioner;

	public void AddGraphButtonHandler()
	{
		Vector3 cameraPosition = Camera.main.transform.position;
		Quaternion orientation = Camera.main.transform.rotation;
		// Create a new axis for placing a graph
		Instantiate (GraphPositioner, cameraPosition, orientation);

		//GameObject positioner = GraphPositioner.GetComponent<ResizeBox>() as ResizeBox;

		IDataProvider dataProvider = GetComponent<DataProvider> () as DataProvider;

		if (dataProvider == null) {
			Debug.Log ("Unable to get the data provider");
			return;
		}

		IList<ITraceDescriptor> availableTraces = dataProvider.AvailableTraces;

		//Hard coded registering for 3 time traces
		//dataProvider.RegisterForData (availableTraces[0], dataProcessor);
		//dataProvider.RegisterForData (availableTraces[2], dataProcessor);
		//dataProvider.RegisterForData (availableTraces[4], dataProcessor);

	}
}
