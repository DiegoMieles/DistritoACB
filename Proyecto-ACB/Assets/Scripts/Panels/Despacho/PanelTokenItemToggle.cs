using System;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el objeto seleccionable arrastrable de la carta
/// </summary>
public class PanelTokenItemToggle : MonoBehaviour
{
    #region Fields and Properties

    [Header("Panel components")] [Tooltip("Objeto arrastrable de la carta")]
    public Toggle toggle;
    [SerializeField] [Tooltip("Imagen miniatura de la carta")]
    private Image imageThumbnail;
    [SerializeField]
    [Tooltip("Imagen borde de la carta")]
    private Image imageBorder;
    [SerializeField]
    [Tooltip("Imagen borde de la liga clásica")]
    private Sprite clasicBorderSprite;
    [SerializeField]
    [Tooltip("Imagen borde de la liga actual")]
    private Sprite actualBorderSprite;
    [SerializeField]
    [Tooltip("Imagen miniatura de la carta si es highlight")]
    private Image imageThumbnailHighlight;
    [SerializeField] [Tooltip("Texto con el nombre del objeto")]
    private Text textName;
    [SerializeField] [Tooltip("Texto con el valor hash de la carta")]
    private Text textHash;
    [SerializeField] [Tooltip("Imagen de la carta en el equipo competitivo")]
    private GameObject team;
    [SerializeField] [Tooltip("Imagen de potenciador activo en la carta")]
    private GameObject booster;
    [SerializeField] [Tooltip("Imagen de carta con lesión")]
    private GameObject injured;
    [SerializeField]
    [Tooltip("Datos de la carta")]
    public TokenItemData currentToken;
    [SerializeField] [Tooltip("Datos del highlight")]
    public  HighLightData.HigthlightItems currentHighLight;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos es fallida")]
    private UnityEvent onFailed;

    [Space(5)]
    [Header("Panel to open components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Muestra el panel de la carta")]
    private GameObject cardViewPrefab;

    [Space(5)]
    [Header("Dragable reference")]
    [SerializeField] [Tooltip("Objeto arrastrable")]
    private DragableObject dragable;

    [SerializeField] [Tooltip("Nombre del spinner de carga")]
    private string spinner_name;

    public TokenItemData CurrentToken => currentToken;
    public DragableObject Dragable => dragable;

    private Action onBoosterSet; //Acción que se ejecuta cuando a la carta se le aplica un potenciador

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra los datos de la carta en el objeto arrastrable
    /// </summary>
    /// <param name="hightlightdata">Datos de highlight</param>
    /// <param name="objectivePosition">Posición objetivo del objeto arrastrable</param>
    /// <param name="onDraggedPosition">Acción a ejecutar cuando el objeto llega a su posición final</param>
    /// <param name="onBoosterSet">Acción que se ejecuta cuando a la carta se le aplica un potenciador</param>
    public void ShowInfo(HighLightData.HigthlightItems hightlightdata, GameObject objectivePosition = null, Action onDraggedPosition = null, Action onBoosterSet = null, UnityAction<bool> onToggleClicked = null)
    {
        currentHighLight = hightlightdata;
        textHash.text = currentHighLight.token;
        textName.text = currentHighLight.title;
        this.onBoosterSet = onBoosterSet;
        booster.transform.parent.gameObject.SetActive(false);
        imageThumbnail.transform.parent.gameObject.SetActive(false);
        imageThumbnailHighlight.transform.parent.gameObject.SetActive(true);

        if (objectivePosition != null)
            dragable.SetupDragable(objectivePosition, onDraggedPosition);
        if (onToggleClicked != null)
        {
            GetComponent<Toggle>().onValueChanged.AddListener(onToggleClicked);
        }

        if (!string.IsNullOrEmpty(currentHighLight.pathImgThumbnail))
        {
            WebProcedure.Instance.GetSprite(currentHighLight.pathImgThumbnail, sprite =>
            {
                imageThumbnailHighlight.sprite = sprite;
                imageThumbnailHighlight.gameObject.SetActive(true);
                
                GameObject spinner = GameObject.Find(spinner_name);
                
                if (spinner)
                {
                    for(int i=0; i<spinner.transform.childCount; i++)
                    {
                        spinner.transform.GetChild(i).gameObject.SetActive(false);
                    }   
                }
            }, error =>
            {
                Debug.LogError("fail");
                onFailed.Invoke();
            } );
        }
       
    }

    /// <summary>
    /// Muestra los datos de la carta en el objeto arrastrable
    /// </summary>
    /// <param name="tokendata">Datos de la carta</param>
    /// <param name="objectivePosition">Posición objetivo del objeto arrastrable</param>
    /// <param name="onDraggedPosition">Acción a ejecutar cuando el objeto llega a su posición final</param>
    /// <param name="onBoosterSet">Acción que se ejecuta cuando a la carta se le aplica un potenciador</param>
    public void ShowInfo(TokenItemData tokendata, GameObject objectivePosition = null, Action onDraggedPosition = null, Action onBoosterSet = null, UnityAction<bool> onToggleClicked = null)
    {
        bool isActualLeague = false;
        PanelTeamCompetitivo addTeamPanel = FindObjectOfType<PanelTeamCompetitivo>(true);
        if (addTeamPanel) isActualLeague = addTeamPanel.isActualLeague;
        else isActualLeague = tokendata.current;
        currentToken = tokendata;
        this.onBoosterSet = onBoosterSet;
        team.SetActive(currentToken.isTeam);
        booster.SetActive(currentToken.isBooster);
        injured.SetActive(currentToken.isInjured);
        textName.text = tokendata.name;
        textHash.text = tokendata.token;
        if (imageBorder)imageBorder.sprite =isActualLeague ? actualBorderSprite:clasicBorderSprite ;
        if (onToggleClicked != null)
        {
            GetComponent<Toggle>().onValueChanged.AddListener(onToggleClicked);
        }
        if (objectivePosition != null)
            dragable.SetupDragable(objectivePosition, onDraggedPosition);
        string pathThumbnail = !string.IsNullOrEmpty(currentToken.card.pathThumbnail) ? currentToken.card.pathThumbnail : currentToken.pathThumbnail;
        if (!string.IsNullOrEmpty(pathThumbnail))
        {
            WebProcedure.Instance.GetSprite(pathThumbnail, sprite =>
            {
                imageThumbnail.sprite = sprite;
                imageThumbnail.gameObject.SetActive(true);
                GameObject spinner = GameObject.Find(spinner_name);
                if (spinner)
                {
                    for(int i=0; i<spinner.transform.childCount; i++)
                    {
                        spinner.transform.GetChild(i).gameObject.SetActive(false);
                    }   
                }

            }, error =>
            {
                Debug.LogError("fail");
                onFailed.Invoke();
            } );
        }
       
    }

    /// <summary>
    /// Muestra panel de la carta
    /// </summary>
    public void ShowCard()
    {
        panelOpener.popupPrefab = cardViewPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelCardCompetitiveTeam>().SetCardData(currentToken, ()=> {
            GameObject.FindObjectOfType<PanelTeamCompetitivo>().CallInfoActualLeague();
            PanelTeamCompetitivo.OnClose?.Invoke();
        }, false, onBoosterSet);
    }

    #endregion

}