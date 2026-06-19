using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Requerido para poder cargar el menú
using UnityEngine.UI;              // Requerido para manejar componentes de Botones

public class GestorMision : MonoBehaviour
{
    public static GestorMision Instancia;

    [Header("UI Textos Nativos")]
    [SerializeField] private TextMeshProUGUI textoMisionIzquierda;
    [SerializeField] private GameObject contenedorPromptE;
    [SerializeField] private TextMeshProUGUI textoCentroDesarrollo;

    // NUEVA VARIABLE: Para encender el botón al final
    [SerializeField] private Button botonVolverMenu;

    [Header("Referencias de Objetos (Faros/Hitos)")]
    [SerializeField] private GameObject puntoBasuraFarol;
    [SerializeField] private GameObject puntoDepositoFarol;
    [SerializeField] private GameObject puntoRetornoFarol;

    [Header("Modelos 3D de la Basura")]
    [SerializeField] private GameObject modeloBasuraLago;
    [SerializeField] private GameObject modeloBasuraDeposito;

    private int estadoActual = 0;
    private bool jugadorCerca = false;

    void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        // Aseguramos que el tiempo corra normal al iniciar (por si acaso)
        Time.timeScale = 1f;

        textoMisionIzquierda.text = "Dirígete hacia el faro rojo.";
        contenedorPromptE.SetActive(false);
        textoCentroDesarrollo.gameObject.SetActive(false);

        // El botón inicia apagado
        if (botonVolverMenu != null) botonVolverMenu.gameObject.SetActive(false);

        puntoBasuraFarol.SetActive(true);
        modeloBasuraLago.SetActive(true);
        puntoDepositoFarol.SetActive(false);
        puntoRetornoFarol.SetActive(false);
        modeloBasuraDeposito.SetActive(false);
    }

    public void SetJugadorCerca(bool cerca)
    {
        jugadorCerca = cerca;
        contenedorPromptE.SetActive(cerca);
    }

    public void Interactuar()
    {
        if (!jugadorCerca) return;

        if (estadoActual == 0)
        {
            estadoActual = 1;
            SetJugadorCerca(false);
            puntoBasuraFarol.SetActive(false);
            modeloBasuraLago.SetActive(false);
            puntoDepositoFarol.SetActive(true);
            textoMisionIzquierda.text = "Lleva los desechos al siguiente faro.";
        }
        else if (estadoActual == 1)
        {
            estadoActual = 2;
            SetJugadorCerca(false);
            puntoDepositoFarol.SetActive(false);
            modeloBasuraDeposito.SetActive(true);
            puntoRetornoFarol.SetActive(true);
            textoMisionIzquierda.text = "¡Excelente! Regresa al muelle de partida.";
        }
    }

    public void FinalizarJuego()
    {
        if (estadoActual != 2) return;
        estadoActual = 3;

        puntoRetornoFarol.SetActive(false);
        textoMisionIzquierda.text = "";
        contenedorPromptE.SetActive(false);

        textoCentroDesarrollo.gameObject.SetActive(true);
        textoCentroDesarrollo.text = "¡Gracias por jugar!\n\nEl videojuego se encuentra actualmente en desarrollo.";

        // MAGIA: Encendemos el botón para que el usuario pueda hacer clic
        if (botonVolverMenu != null)
        {
            botonVolverMenu.gameObject.SetActive(true);
        }

        // Congelar las físicas y el movimiento del barco al ganar
        Time.timeScale = 0f;
    }

    // NUEVA FUNCIÓN PÚBLICA: Para recargar la escena del menú
    public void CargarMenuPrincipal()
    {
        // IMPORTANTE: Reseteamos el tiempo a la normalidad antes de cambiar de escena
        Time.timeScale = 1f;

        // Reemplaza "MenuPrincipal" por el nombre exacto de la escena de tu menú
        SceneManager.LoadScene("Menu");
    }

    public int GetEstadoActual() { return estadoActual; }
}