using Data;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Clase personalizada con los datos espec�ficos de una ACBall
/// </summary>
public class ACBallButton : MonoBehaviour
{
    [Header("Buttons components")]
    [SerializeField] [Tooltip("Imagen de la ACBall espec�fica")]
    private Image acballImage;
    [SerializeField] [Tooltip("Titulo que contiene el nombre de la ACBall")]
    private Text acballTitleText;
    [SerializeField] [Tooltip("Texto que contiene la descripci�n de la ACBall")]
    private Text acballDescriptionText;
    [SerializeField] [Tooltip("Bot�n que se encarga de llevar al usuario al panel donde se acepta si una ACBall se abre o no se abre")]
    private Button openConfirmationPanelButton;
    [SerializeField] [Tooltip("Burbuja de la zona superior que se muestra cuando una ACBall no se ha visto/abierto")]
    private GameObject bubble;

    [Space(5)]
    [Header("Panel references")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de confirmaci�n de si se abre o no una ACBall")]
    private GameObject confirmationPanelPrefab;

    private Action onFinishedOpeningACBall; //Acci�n que se llama una vez una ACBall es abierta
    private Action onGoBack; //Acci�n que se llama al retroceder en la opci�n de selecionar si se abre o no una ACBall
    public AcbBallContainer.AcbBallsData.AcBallsItems acballItemData { get; private set; } //Clase con los datos de los items que se encuentran dentro de la ACBall

    #region Public Methods

    /// <summary>
    /// M�todo que configura el bot�n con sus respectivas acciones al abrir, no abrir y recibir un item de una ACBall
    /// </summary>
    /// <param name="acballItemData">Datos de los items dentro de la ACBall</param>
    /// <param name="onFinishedOpeningACBall">Acci�n que se llama al abrir una ACBall</param>
    /// <param name="onGoBack">Acci�n que se llama al no abrir una ACBall</param>
    /// <param name="onClickedButton">Acci�n que se ejecuta al seleccionar el bot�n de la ACBall</param>
    public void SetupButton(AcbBallContainer.AcbBallsData.AcBallsItems acballItemData, Action onFinishedOpeningACBall, Action onGoBack, Action onClickedButton)
    {
        this.onGoBack = onGoBack;
        this.acballItemData = acballItemData;
        this.onFinishedOpeningACBall = onFinishedOpeningACBall;
        onClickedButton?.Invoke();
        WebProcedure.Instance.GetSprite(acballItemData.path_img, OnSuccess, OnFailed);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// M�todo que se llama al cargar la imagen de la ACBall satisfactoriamente de backend y que se
    /// Encarga de mostrar los datos de la ACBall junto con su imagen
    /// </summary>
    /// <param name="obj">Imagen de la ACBall traida desde backend</param>
    private void OnSuccess(Sprite obj)
    {
        bubble?.SetActive(!acballItemData.show);
        openConfirmationPanelButton.onClick.AddListener(OpenConfirmationPanel);
        acballImage.sprite = obj;
        acballTitleText.text = acballItemData.name;
        acballDescriptionText.text = acballItemData.description+ " \n " +"Id: "+acballItemData.id;
        GameObject spinner = GameObject.Find("Spinner_ACBall");
        for(int i=0; i<spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// M�todo que se llama si la imagen de la ACBall no se pudo cargar desde backend
    /// </summary>
    /// <param name="obj">Clase con los datos de error devueltos desde backend</param>
    private void OnFailed(WebError obj)
    {
        Debug.Log(obj.Message);
        Destroy(gameObject);
    }

    /// <summary>
    /// M�todo que se encarga de abrir el panel donde se decide si una ACBall se abre o no
    /// </summary>
    private void OpenConfirmationPanel()
    {
        panelOpener.popupPrefab = confirmationPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelACBallOpenConfirmation>().SetupPanel(acballItemData, onFinishedOpeningACBall, onGoBack);

        AcbBallContainer.ACBallUpdateShowBody body = new AcbBallContainer.ACBallUpdateShowBody() { acball_id = acballItemData.id };

        WebProcedure.Instance.PostSetShowACBall(JsonConvert.SerializeObject(body), (obj) => { Debug.Log(obj.RawJson); }, (error) => { });
    }

    #endregion
}
