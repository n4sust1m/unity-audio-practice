using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauges: MonoBehaviour {
    private List<GameObject> _gauges;
    private List<float> _levels;
    // audio api
    // ref: https://qiita.com/niusounds/items/b8858a2b043676185a54
    private AudioSource _audio;
    const int SAMPLE_RATE = 64;
    private int GAUGE_COUNT;
    void Awake () {
        GAUGE_COUNT = SAMPLE_RATE - 20; // とりあえずのハイパス

        _gauges = new List<GameObject>() {};
        _levels = new List<float>() {};
        var gaugeObj = GameObject.Find("Gauge");

        for (int i = 0; i < GAUGE_COUNT; i++) {
            float posX = (i - GAUGE_COUNT / 2.0f) / (GAUGE_COUNT / 2.0f) * 5.0f;
            GameObject g = Instantiate(gaugeObj, new Vector3(posX, 0, 0), new Quaternion(.0f, .0f, .0f, .0f));
            _gauges.Add(g);
        }
        for (int i = 0; i < SAMPLE_RATE; i++) {
            _levels.Add(.0f);
        }
    }
    // Start is called before the first frame update
    void Start () {
        Application.targetFrameRate = 30;

        _audio = GetComponent<AudioSource>();
        _audio.clip = Microphone.Start(null, true, 10, 44100);
        _audio.loop = true;
        //_audio.mute = true;
        while (!(Microphone.GetPosition("") > 0)){}
        _audio.Play();
    }

    // Update is called once per frame
    void Update () {

        float[] spectrum = new float[SAMPLE_RATE]; // minsize=64
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        Debug.Log("Sample: " + spectrum[SAMPLE_RATE / 2]);

        for (int i = SAMPLE_RATE - GAUGE_COUNT; i < SAMPLE_RATE; i++) {
            if (_levels[i] * 0.96f - spectrum[i] > 0) {
                _levels[i] *= 0.96f;
                _gauges[i - (SAMPLE_RATE - GAUGE_COUNT)].transform.localScale = new Vector3(0.2f, _levels[i] * 2000.0f, 1);
            } else {
                _gauges[i - (SAMPLE_RATE - GAUGE_COUNT)].transform.localScale = new Vector3(0.2f, spectrum[i] * 2000.0f, 1);
                _levels[i] = spectrum[i];
            }
        }
    }
}
