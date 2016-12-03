using UnityEngine;
using System.Collections;

// Attach this to the GameObject that has your Typewriter script
public class AdvanceOnKeypressOrMouseInput : MonoBehaviour {
	public bool useKeyCode;
	public KeyCode keyCode;
	public bool useMouseButton;
	public int mouseButton;

	void Update () {
		if((useKeyCode && Input.GetKeyDown(keyCode)) || (useMouseButton && Input.GetMouseButtonDown(mouseButton))) {
			GetComponent<Typewriter>().Advance();
		}
	}
}
