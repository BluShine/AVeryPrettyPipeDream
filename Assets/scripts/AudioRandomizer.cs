using UnityEngine;
using System.Collections;

public class AudioRandomizer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSource a = GetComponent<AudioSource>();
        a.time = Random.Range(0f, a.clip.length);
	}
}
