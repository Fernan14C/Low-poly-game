using UnityEngine;

public class FlotabilidadXolotlan : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody _rb;
    public XolotlanCPU _scriptAgua;

    [Header("Configuración Física")]
    public float _fuerzaFlotacion = 15f;
    public float _profundidadHundimiento = 1f;
    public float _resistenciaAgua = 1f;

    // Lista de puntos (puedes usar GameObjects vacíos como hijos del barco)
    public Transform[] _puntosFlotacion;

    void FixedUpdate()
    {
        foreach (var _punto in _puntosFlotacion)
        {
            // 1. Obtenemos la altura de la ola en la posición del punto
            float _alturaOla = _scriptAgua.GetWaveHeight(_punto.position);

            // 2. Calculamos la diferencia entre el punto y el agua
            float _diferenciaAltura = _punto.position.y - _alturaOla;

            // 3. Si el punto está bajo el agua, aplicamos fuerza hacia arriba
            if (_diferenciaAltura < 0)
            {
                // Fórmula de flotación simplificada
                float _desplazamiento = Mathf.Abs(_diferenciaAltura) / _profundidadHundimiento;
                float _fuerzaAplicada = Mathf.Clamp01(_desplazamiento) * _fuerzaFlotacion;

                // Aplicamos la fuerza en la posición exacta del punto
                _rb.AddForceAtPosition(Vector3.up * _fuerzaAplicada, _punto.position, ForceMode.Acceleration);

                // Aplicamos resistencia para que no salte como un resorte
                _rb.AddForceAtPosition(-_rb.linearVelocity * _resistenciaAgua, _punto.position, ForceMode.Acceleration);
            }
        }
    }
}