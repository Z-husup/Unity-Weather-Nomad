using UnityEngine;

public class FloatingGroup : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float baseAmplitude = 0.1f;
    [SerializeField] private float baseFrequency = 1f;

    [Header("Random Ranges")]
    [SerializeField] private float amplitudeRandom = 0.05f;
    [SerializeField] private float frequencyRandom = 0.5f;
    [SerializeField] private float offsetRandom = 5f;

    private Transform[] children;
    private Vector3[] startPositions;

    private float[] amplitudes;
    private float[] frequencies;
    private float[] offsets;

    private void Awake()
    {
        int count = transform.childCount;

        children = new Transform[count];
        startPositions = new Vector3[count];

        amplitudes = new float[count];
        frequencies = new float[count];
        offsets = new float[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            children[i] = child;
            startPositions[i] = child.localPosition;

            // RANDOM FOR EACH OBJECT
            amplitudes[i] = baseAmplitude + Random.Range(-amplitudeRandom, amplitudeRandom);
            frequencies[i] = baseFrequency + Random.Range(-frequencyRandom, frequencyRandom);
            offsets[i] = Random.Range(0f, offsetRandom);
        }
    }

    private void Update()
    {
        float time = Time.time;

        for (int i = 0; i < children.Length; i++)
        {
            float wave = Mathf.Sin((time + offsets[i]) * frequencies[i]) * amplitudes[i];

            Vector3 pos = startPositions[i];
            pos.y += wave;

            children[i].localPosition = pos;
        }
    }
}