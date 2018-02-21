using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObjectVisibility : MonoBehaviour {

	public GameObject ObjectToToggle;

	public void ToggleObject()
	{
		ObjectToToggle.SetActive(!ObjectToToggle.activeSelf);
	}
}
