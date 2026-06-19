using UnityEngine;
using UnityEngine.InputSystem; // Importamos el sistema moderno de Unity 6

public class DetectorMisionesBarco : MonoBehaviour
{
    [Header("Configuración de Rangos")]
    [SerializeField] private float distanciaInteraccion = 6f;

    private Transform transBasura;
    private Transform transDeposito;
    private Transform transRetorno;
    private bool enRangoDeInteraccion = false;

    void Start()
    {
        // Buscamos las posiciones usando los nombres exactos de la jerarquía
        GameObject objBasura = GameObject.Find("Punto_Basura");
        GameObject objDeposito = GameObject.Find("Punto_Deposito");
        GameObject objRetorno = GameObject.Find("Punto_Retorno");

        if (objBasura != null) transBasura = objBasura.transform;
        if (objDeposito != null) transDeposito = objDeposito.transform;
        if (objRetorno != null) transRetorno = objRetorno.transform;
    }

    void Update()
    {
        if (GestorMision.Instancia == null) return;

        int estado = GestorMision.Instancia.GetEstadoActual();
        enRangoDeInteraccion = false; // Reseteamos cada cuadro

        if (estado == 0 && transBasura != null) // Distancia a la basura
        {
            float dist = Vector3.Distance(transform.position, transBasura.position);
            enRangoDeInteraccion = (dist <= distanciaInteraccion);
            GestorMision.Instancia.SetJugadorCerca(enRangoDeInteraccion);
        }
        else if (estado == 1 && transDeposito != null) // Distancia al depósito
        {
            float dist = Vector3.Distance(transform.position, transDeposito.position);
            enRangoDeInteraccion = (dist <= distanciaInteraccion);
            GestorMision.Instancia.SetJugadorCerca(enRangoDeInteraccion);
        }
        else if (estado == 2 && transRetorno != null) // Llegada al muelle
        {
            float dist = Vector3.Distance(transform.position, transRetorno.position);
            if (dist <= distanciaInteraccion - 2f)
            {
                GestorMision.Instancia.FinalizarJuego();
            }
        }

        // SOLUCIÓN COMPATIBLE CON EL NUEVO SISTEMA:
        // Revisamos de manera directa si la tecla E fue presionada en este frame
        if (enRangoDeInteraccion && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            GestorMision.Instancia.Interactuar();
        }
    }
}