using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Enumerador de los estados de arrastre de un objeto
public enum DragState { NONE, BLOCKED }

/// <summary>
/// Clase que controla el arrastre de un item arrastrable
/// </summary>
public class DragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Tooltip("Panel de arrastre de la carta con información")]
    public PlayerCard panelCardDragInfo;
    [Tooltip("Grupo contenedor de la carta arrastrable")]
    public GridLayoutGroup gridLayoutGroup;
    [Tooltip("Posición final de la carta")]
    public Transform targetPosition;

    [SerializeField] [Tooltip("Distancia de error mínimo con respecto al objeto de destino")]
    private float minDistanceToParent = 3;
    [SerializeField] [Tooltip("Mostrar registros en la consola de distancia del objeto arrastrable con respecto a su destino")]
    private bool showDebugDistance;
    [SerializeField] [Tooltip("Estado actual de arrastre del objeto")]
    private DragState dragState = DragState.NONE;

    protected bool isDraggin; //Determina si el objeto está siendo arrastrado

    /// <summary>
    /// Se ejecuta al iniciar el arrastre del objeto
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDrag();
    }

    /// <summary>
    /// Método que se llama al arrastrar un objeto
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (dragState == DragState.BLOCKED)
            return;
        CheckDrag();
    }

    /// <summary>
    /// Método que se llama al finalizar el arrastre de un objeto
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragState == DragState.BLOCKED)
            return;
        CheckEndDrag();
    }

    /// <summary>
    /// Verifica el arrastre del objeto
    /// </summary>
    private void CheckDrag()
    {
        if (dragState != DragState.BLOCKED)
        {
            isDraggin = true;
            var screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f;
            transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

            if (targetPosition)
            {
                float distance = Vector3.Distance(transform.position, targetPosition.position);  
                if (showDebugDistance)
                {
                    Debug.Log(distance);
                }
            }
        }
    }

    /// <summary>
    /// Determina si al final de un arrastre el objeto llego a su posición objetivo o no
    /// </summary>
    private void CheckEndDrag()
    {
        isDraggin = false;
        DestroyImmediate(this.gameObject.GetComponent<Canvas>());
        if (targetPosition)
        {
            float distance = Vector3.Distance(transform.position, targetPosition.position);     
            if (distance <= minDistanceToParent)
            {
                OnDragComplete();
            }
            else if (distance > minDistanceToParent)
            {
                OnDragFailed();
            }
        }
    }

    /// <summary>
    /// Regresa el objeto a su posición inicial
    /// </summary>
    private void ResetToInitialPosition() => gridLayoutGroup.CalculateLayoutInputHorizontal();

    /// <summary>
    /// Asigna un estado de arrastre al objeto
    /// </summary>
    /// <param name="dragstate">Nuevo estado de arrastre del objeto</param>
    private void ChangeDragState(DragState dragstate) => dragState = dragstate;

    /// <summary>
    /// Desbloquea el arrastre del objeto
    /// </summary>
    public void UnlockDrag() => ChangeDragState(DragState.NONE);
    
    /// <summary>
    /// Bloquea arrastre del objeto
    /// </summary>
    public void LockDrag() => ChangeDragState(DragState.BLOCKED);

    /// <summary>
    /// Método que se ejecuta al completar el arrastre del objeto
    /// </summary>
    protected virtual void OnDragComplete()
    {
        if (dragState != DragState.BLOCKED)
        {
            Debug.Log("Complete");
            ResetToInitialPosition();
        }
    }

    /// <summary>
    /// Reinicia posición del objeto al fallar el arrastre
    /// </summary>
    protected virtual void OnDragFailed()
    {
        if (dragState != DragState.BLOCKED)
        {
            Debug.Log("Failed");
            ResetToInitialPosition();
        }
    }

    /// <summary>
    /// Método que se ejecuta al iniciar el arrastre del objeto
    /// </summary>
    protected virtual void OnBeginDrag()
    {
        if (dragState != DragState.BLOCKED)
        {
            var sorting = this.gameObject.AddComponent<Canvas>();
            sorting.overrideSorting = true;
        }
    }
}
