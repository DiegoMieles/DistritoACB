using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Controla todos los objetos que puedan hacer drag and drop (o se puedan arrastrarse), actualmente la figura de
/// los jugadores
/// </summary>
public class DragableObject : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Object objective position")]
    [SerializeField] [Tooltip("Imagen del token de jugador que se puede arrastrar")]
    private Image playerView;

    private GameObject objectivePostion; //Posición a la que se espera que el objeto sea arrastrado

    private bool _interactable = false; //Determina si el objeto puede ser arrastrado o no
    public bool Interactable { get => _interactable; set => _interactable = value; }

    private Vector2 _originPosition; //Posición inicial del objeto
    private Canvas _mainCanvasRef; //Referencia al canvas principal del juego
    private RectTransform _handleableTransform; //Referencia del objeto arrastrable a nivel posicional

    private Action onObjectPosition = null; //Acción que se ejecuta una vez el objeto llega a su posición final

    #region Public Methods

    /// <summary>
    /// Configura el objeto arrastrable asignando la posición final
    /// </summary>
    /// <param name="objectivePostion">Posición a la que se espera que llegue el objeto arrastrable</param>
    /// <param name="onObjectPosition">Acción que se ejecuta una vez el objeto arrastrable llega a la posición final</param>
    public void SetupDragable(GameObject objectivePostion, Action onObjectPosition)
    {
        this.onObjectPosition = onObjectPosition;
        this.objectivePostion = objectivePostion;
        _interactable = true;
        gameObject.SetActive(true);
        _mainCanvasRef = FindObjectOfType<Canvas>();
        playerView.color = new Color(1, 1, 1, 1);
        _handleableTransform = GetComponent<RectTransform>();
        _originPosition = _handleableTransform.anchoredPosition;
    }

    /// <summary>
    /// Resetea el objeto arrastrable a su posición original y habilita su interaccióñ
    /// </summary>
    public void ResetDragable()
    {
        _interactable = true;
        gameObject.SetActive(true);
        playerView.color = new Color(1, 1, 1, 1);
        _handleableTransform.anchoredPosition = _originPosition;
    }

    #endregion

    #region IDragHandler Implementation

    /// <summary>
    /// Método que se ejecuta cuando el objeto se intenta arrastrar verificando si es interactuable
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (Interactable)
            _handleableTransform.anchoredPosition += eventData.delta / _mainCanvasRef.scaleFactor;
    }

    #endregion

    #region IEndDragHandler Implementation

    /// <summary>
    /// Método que se ejecuta al finalizar el arrastre del objeto y detecta si el objeto ya llego a su posición objetivo
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, 0.5f, Vector3.zero);
        if (hit.Length > 0)
        {
            //Check hit objects
            for (int i = 0; i < hit.Length; i++)
            {
                GameObject hitGameObject = hit[i].collider.gameObject;

                if (hitGameObject == objectivePostion)
                {
                    Interactable = false;
                    onObjectPosition?.Invoke();
                    playerView.color = new Color(1, 1, 1, 0.5f);
                    _handleableTransform.anchoredPosition = _originPosition;
                    return;
                }
            }
            _handleableTransform.anchoredPosition = _originPosition;
        }
        else
        {
            if(_handleableTransform)
                _handleableTransform.anchoredPosition = _originPosition;
        }
    }

    #endregion
}