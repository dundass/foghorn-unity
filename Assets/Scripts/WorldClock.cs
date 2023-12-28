using UnityEngine;
using System.Collections;

public class WorldClock : MonoBehaviour {

    public int time { get; private set; }

    private int dayLength = 10000;

    public GameObject sun;
    private Light sunLight;

    // Use this for initialization
    void Start() {
        time = 0;
        sunLight = sun.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update() {
        time++;
        sunLight.intensity = 1 + Mathf.Sin(time * 6.33f / dayLength) / 2;
        // mod hue
        if (time % dayLength == 0) Debug.Log("cockadoodledoo");
    }
}
