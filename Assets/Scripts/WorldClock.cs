using UnityEngine;

public class WorldClock : MonoBehaviour {

    [SerializeField] int dayLength = 10000;
    [SerializeField] int nightThreshold = 6666;
    [SerializeField] float lightOffset = 1f;
    [SerializeField] float lightAmplitude = 0.5f;
    [SerializeField] float lightLimit = 1f;
    [SerializeField] Light sunLight;

    public int time { get; private set; }

    private void Awake()
    {
        GameManager.Instance.WorldClock = this;
    }

    private void Start() {
        time = 0;
    }

    private void Update() {
        //Debug.Log(Time.deltaTime);    // todo - incorporate Time.deltaTime into clock advancement. somehow
        time++;
        float intensity = lightOffset + (Mathf.Sin(time * 6.33f / dayLength) * lightAmplitude);
        sunLight.intensity = Mathf.Min(intensity, lightLimit);
        // mod hue
        if (time % dayLength == 0) Debug.Log("cockadoodledoo");
    }

    public bool IsDaytime()
    {
        return time % dayLength < nightThreshold;
    }
}
