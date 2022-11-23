using UnityEngine;
using Data;


/// <summary>
/// Controla el ciclo circadiano de juego (Ciclo d�a/noche)
/// </summary>
public class LightsCircadianCycle : MonoBehaviour
{
    [Header("Global Lightning configuration")]
    [SerializeField] [Tooltip("Luz global del d�a/noche")]
    private UnityEngine.Rendering.Universal.Light2D globalDayLight;
    [SerializeField] [Tooltip("Color de luz del d�a en el juego")]
    private Color dayLight;
    [SerializeField] [Tooltip("Color de luz de la noche en el juego")]
    private Color nightLight;

    [Space(5)]
    [Header("Map Lightning configuration")]
    [SerializeField] [Tooltip("Objeto que contiene todas las luces del juego")]
    private GameObject nightLights;
    [SerializeField] [Tooltip("Listado de puntos de luces del juego")]
    private UnityEngine.Rendering.Universal.Light2D[] mapLightPoints;

    private float[] lightBaseInnerRadius; //Listado de radios de iluminaci�n interno de los puntos de luz del juego
    private float[] lightBaseOuterRadius; //Listado de radios de iluminaci�n externo de los puntos de luz del juego

    private TransitionTime dateTime; //Enumerador del tiempo del d�a (dia o noche)
    private bool isFirstTimeScalingLight = true; //Detecta si las luces se deben escalar por primera vez

    #region Public Methods

    /// <summary>
    /// Configura el sistema de iluminaci�n general
    /// </summary>
    /// <param name="newScale">Escala inicial de las luces del juego</param>
    /// <param name="actualDateTime">Tiempo del d�a traido desde backend</param>
    public void StartLightSetup(float newScale, TransitionTime actualDateTime)
    {
        dateTime = actualDateTime;

        nightLights.SetActive(dateTime == TransitionTime.NIGHT);
        globalDayLight.color = dayLight;

        if(dateTime == TransitionTime.NIGHT)
        {
            SaveBaseLightsIntensity();

            if(isFirstTimeScalingLight)
                ScaleLight(newScale);
            
            globalDayLight.color = nightLight;
            isFirstTimeScalingLight = false;
        }
    }

    /// <summary>
    /// Escala las luces del juego
    /// </summary>
    /// <param name="newScale">Valor nuevo de la escala de luz</param>
    public void ScaleLight(float newScale)
    {
        if (dateTime == TransitionTime.DAY)
            return;

        for (int i = 0; i < mapLightPoints.Length; i++)
        {
            mapLightPoints[i].pointLightInnerRadius = lightBaseInnerRadius[i] * newScale;
            mapLightPoints[i].pointLightOuterRadius = lightBaseOuterRadius[i] * newScale;
        }
    }
    #endregion

    #region Inner Methods

    /// <summary>
    /// Guarda el valor inicial de intensidad de iluminaci�n de las luces
    /// </summary>
    private void SaveBaseLightsIntensity()
    {
        lightBaseInnerRadius = new float[mapLightPoints.Length];
        lightBaseOuterRadius = new float[mapLightPoints.Length];

        for (int i = 0; i < mapLightPoints.Length; i++)
        {
            lightBaseInnerRadius[i] = mapLightPoints[i].pointLightInnerRadius;
            lightBaseOuterRadius[i] = mapLightPoints[i].pointLightOuterRadius;
        }
    }

    #endregion
}
