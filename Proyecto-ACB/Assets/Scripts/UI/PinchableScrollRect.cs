using UnityEngine;
using UnityEngine.UI;
using Data;

/// <summary>
/// Clase que extiende las funciones básicas del componente de zona de arrastre (Scroll) y que controla el movimiento
/// y el zoom del mapa
/// </summary>
public class PinchableScrollRect : ScrollRect
{
    #region Fields and Properties

    private float _minZoom = 0.4f; //Zoom mínimo en el mapa
    private float _maxZoom = 1f; //Zoom máximo en el mapa
    private float _zoomLerpSpeed = 10f; //Velocidad de interpolación al detectar un zoom
    private float _currentZoom = 0.4f; //Zoom actual del mapa
    private bool _isPinching = false; //Determina si los dedos están en forma de pinza (haciendo zoom)
    private float _startPinchDist; //Distancia inicial entre los dedos al hacer zoom
    private float _startPinchZoom; //Zoom inicial al poner los dedos en forma de pinza
    private Vector2 _startPinchCenterPosition; //Vector de posición central entre los dedos al formar la pinza
    private Vector2 _startPinchScreenPosition; //Vector de posición del mapa al poner los dedos en forma de pinza
    private float _mouseWheelSensitivity = 1; //Sensibilidad de movimiento en el zoom
    private bool blockPan = false; //Determina si se bloquea el zoom o movimiento en el scroll
    private LightsCircadianCycle lightsController; //Controlador de luces de ciclo dia/noche

    #endregion

    #region Unity Methods

    /// <summary>
    /// Habilita el multitouch para permitir hacer zoom en dispositivos móviles
    /// </summary>
    protected override void Awake()
    {
        Input.multiTouchEnabled = true;
    }
 
    /// <summary>
    /// Detecta de forma constante la ubicación de los dedos para mover el mapa o hacer zoom
    /// </summary>
    private void Update()
    {
        if (Input.touchCount == 2)
        {
            if (!_isPinching)
            {
                _isPinching = true;
                OnPinchStart();
            }
            OnPinch();
        }
        else
        {
            _isPinching = false;
            if (Input.touchCount == 0)
            {
                blockPan = false;
            }
        }
        //pc input
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollWheelInput) > float.Epsilon)
        {
            _currentZoom *= 1 + scrollWheelInput * _mouseWheelSensitivity;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

            if (lightsController != null)
                lightsController.ScaleLight(_currentZoom);

            _startPinchScreenPosition = (Vector2)Input.mousePosition;
             
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, Camera.main, out _startPinchCenterPosition);
            Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
            Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;
            SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
        }
        //pc input end
 
        if (Mathf.Abs(content.localScale.x - _currentZoom) > 0.001f)
            content.localScale = Vector3.Lerp(content.localScale, Vector3.one * _currentZoom, _zoomLerpSpeed * Time.deltaTime);
    }

    #endregion

    #region ScrollRect inheritance

    /// <summary>
    /// Ancla el contenido del mapa a los extremos (lo escala)
    /// </summary>
    /// <param name="position"></param>
    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        if (_isPinching || blockPan) return;
        base.SetContentAnchoredPosition(position);
    }

    #endregion

    #region Public Methods
    
    /// <summary>
    /// Configura el nivel a nivel visual de acuerdo al momento del día
    /// </summary>
    /// <param name="actualTime">Momento del día</param>
    /// <param name="lightsController">Controlador general de las luces del juego</param>
    public void SetupWorldView(TransitionTime actualTime, LightsCircadianCycle lightsController)
    {
        this.lightsController = lightsController;
        lightsController.StartLightSetup(_minZoom, actualTime);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que se encarga de iniciar el estado de los dedos para hacer zoom
    /// </summary>
    private void OnPinchStart()
    {
        Vector2 pos1 = Input.touches[0].position;
        Vector2 pos2 = Input.touches[1].position;
 
        _startPinchDist = Distance(pos1, pos2) * content.localScale.x;
        _startPinchZoom = _currentZoom;
        _startPinchScreenPosition = (pos1 + pos2) / 2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition,  Camera.main, out _startPinchCenterPosition);
 
        Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
        Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;
 
        SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
        blockPan = true;
    }
 
    /// <summary>
    /// Hace el zoom del mapa
    /// </summary>
    private void OnPinch()
    {
        float currentPinchDist = Distance(Input.touches[0].position, Input.touches[1].position) * content.localScale.x;
        _currentZoom = (currentPinchDist / _startPinchDist) * _startPinchZoom;
        _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

        if(lightsController != null)
            lightsController.ScaleLight(_currentZoom);
    }
 
    /// <summary>
    /// Determina la distancia de los dedos a nivel local del mapa
    /// </summary>
    /// <param name="pos1">Posición del primer dedo</param>
    /// <param name="pos2">Posición del segundo dedo</param>
    /// <returns>Distancia entre los dedos</returns>
    private float Distance(Vector2 pos1, Vector2 pos2)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos1,  Camera.main, out pos1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos2,  Camera.main, out pos2);
        return Vector2.Distance(pos1, pos2);
    }
 
    /// <summary>
    /// Establece punto central al hacer zoom
    /// </summary>
    /// <param name="rectTransform">Componente de transformación del mapa</param>
    /// <param name="pivot">Punto medio al hacer zoom</param>
    static void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;
 
        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y) * rectTransform.localScale.x;
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    #endregion
}