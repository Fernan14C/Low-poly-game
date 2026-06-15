using UnityEngine;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public Material waterMaterial;

    // MATCHING SHADER GRAPH (Simple Sine Waves)
    // Formula en SG: y = amp * sin(pos / len + offset)

    [Header("Onda 1 (Principal, eje X)")]
    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    [HideInInspector] public float offset = 0f; // Gestionado por tiempo

    [Header("Onda 2 (Detalle, eje Z)")]
    public float amplitude2 = 0.4f;
    public float length2 = 5.5f;
    public float speed2 = 1.4f;
    [HideInInspector] public float offset2 = 0f; // Gestionado por tiempo

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    private void Update()
    {
        // Calculamos los tiempos por separado según su velocidad
        offset += Time.deltaTime * speed;
        offset2 += Time.deltaTime * speed2;

        // Enviamos los datos al Shader usando tus nombres de Referencia
        if (waterMaterial != null)
        {
            // Onda 1 (References from image_1cbe19/image_1c553c)
            waterMaterial.SetFloat("_amplitude", amplitude);
            waterMaterial.SetFloat("_length", length);
            waterMaterial.SetFloat("_offset", offset);

            // Onda 2
            waterMaterial.SetFloat("_amplitude2", amplitude2);
            waterMaterial.SetFloat("_length2", length2);
            waterMaterial.SetFloat("_offset2", offset2);
        }
    }

    // FÍSICA SINCRONIZADA: Simple Sine Wave Sum
    public float GetWaveHeight(Vector3 _position)
    {
        // Ola 1: Solo usa X
        float wave1 = amplitude * Mathf.Sin(_position.x / length + offset);

        // Ola 2: Solo usa Z
        float wave2 = amplitude2 * Mathf.Sin(_position.z / length2 + offset2);

        return wave1 + wave2;
    }
}