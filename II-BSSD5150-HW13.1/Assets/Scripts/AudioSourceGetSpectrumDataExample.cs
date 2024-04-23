using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Formulas and original code for analysis taken from:
//https://discussions.unity.com/t/getoutputdata-and-getspectrumdata-what-represent-the-values-returned/27063/2

[RequireComponent(typeof(AudioSource))]
public class AudioSourceGetSpectrumDataExample : MonoBehaviour
{
    [SerializeField]
    private GameObject square1;

    [SerializeField]
    private GameObject projectilePrefab; // Reference to the projectile prefab

    int qSamples = 1024; // array size
    float refValue = 0.1f; // RMS value for 8 dB
    float threshold = 0.02f;
    float rmsValue;
    float dbValue;
    float pitchValue; // sound pitch - Hz
    private float[] samples; // audio samples 
    private float[] spectrum; // audio spectrum 
    private float fSample;

    void Start()
    {
        samples = new float[qSamples];
        spectrum = new float[qSamples];
        fSample = AudioSettings.outputSampleRate;
    }

    void AnalyzeSound()
    {
        AudioListener.GetOutputData(samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < qSamples; i++)
            sum += samples[i] * samples[i]; // sum squared samples
        rmsValue = Mathf.Sqrt(sum / qSamples); // rms square root of average.
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min

        // get sound spectrum
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        int maxN = 0;
        for (i = 0; i < qSamples; i++)
        {
            // find max
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
        }

        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < qSamples - 1)
        {
            // interpolate index using neighbours
            float dL = spectrum[maxN - 1];
            float dR = spectrum[maxN + 1];
            freqN += 0.5f * (dR - dL) / (2 * maxV - dR - dL);
            pitchValue = freqN * (fSample / 2) / qSamples; // convert index to frequency
        }
    }

    void Update()
    {
        AnalyzeSound();
        float xpos = square1.GetComponent<Rigidbody2D>().position.x;
        square1.GetComponent<Rigidbody2D>().position = new Vector2(xpos, dbValue);
        if (pitchValue % 3 == 0)
        {
            Instantiate(projectilePrefab, square1.transform.position, Quaternion.identity);
        }
    }
}