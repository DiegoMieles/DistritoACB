using UnityEngine;

/// <summary>
/// Clase que se encarga de la apertura de paneles
/// </summary>
public class PanelOpener : MonoBehaviour
{
    [Tooltip("Prefab del panel a abrir")]
    public GameObject popupPrefab;
    [Tooltip("Referencia del canvas donde va a aparecer el panel")]
    protected Canvas m_canvas;
    [Tooltip("Determina si el panel a crear se debe crear como hijo del canvas")]
    public bool canParentCanvas = true;
    [HideInInspector] [Tooltip("Referencia que se crea del prefab del panel")]
    public GameObject popup;

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena, detectando cual es el canvas principal
    /// </summary>
    protected void Awake()
    {
        m_canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// <summary>
    /// Abre el panel
    /// </summary>
    public virtual void OpenPopup()
    {
        popup = Instantiate(popupPrefab) as GameObject;
        popup.SetActive(true);
        if (m_canvas  && canParentCanvas)
        {
            popup.transform.SetParent(m_canvas.transform, false);
        }
 
        popup.GetComponent<Panel>().Open();
    }

    /// <summary>
    /// Cierra el panel
    /// </summary>
    public virtual void ClosePopup()
    {
        if (popup)
        {
            popup.GetComponent<Panel>().Close();
        }
    }
}
