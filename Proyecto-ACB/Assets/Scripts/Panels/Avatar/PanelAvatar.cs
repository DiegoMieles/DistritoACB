using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel de visualización de avatar
/// </summary>
public class PanelAvatar : Panel
{
    [Header("Panel Components")]
    [Space(5)]
    [SerializeField] [Tooltip("Texto con el apodo del jugador")]
    private Text nicknameText;
    [SerializeField] [Tooltip("Texto con la versión del juego")]
    private Text textVersion;
    [SerializeField] [Tooltip("Botón de cerrado de panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Botón de editar avatar")]
    private List<Button> editAvatarButtons;
    [SerializeField] [Tooltip("Botón de salir de la partida")]
    private Button logOutButton;
    [SerializeField] [Tooltip("Botón de salir de la partida")]
    private Button editAccountButton;
    [SerializeField] [Tooltip("Prefab de la vista del avatar")]
    private GameObject avatarViewPrefab;
    [SerializeField] [Tooltip("Prefab del panel de cambio de nombre del jugador")]
    private GameObject changeNamePanelPrefab;
    [SerializeField] [Tooltip("Prefab del panel de edición de la cuentar")]
    private GameObject editAccountPanelPrefab;
    [SerializeField] [Tooltip("Imagen del avatar dek jugador")]
    private AvatarImageView avatarView;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpenerAvatarEditor;
    [SerializeField] [Tooltip("Grupo de canvas donde se controla la visibilidad del panel")]
    private CanvasGroup canvasGroup;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;

    private Action onClosePanel; //Acción que se ejecuta cuando el panel es cerrado

    public AvatarImageView AvatarView => avatarView;
    public Button CloseButton => closeButton;

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena cargando el avatar del 
    /// jugador y sus datos
    /// </summary>
    private void Start()
    {
        closeButton.gameObject.SetActive(!ACBSingleton.Instance.AccountData.avatarData.isFirstTime);
        closeButton.onClick.AddListener(() => { CloseAvatarPanel(); onClosePanel?.Invoke(); });
        logOutButton.onClick.AddListener(() => {
            StartCoroutine(CloseSpinner());
            WebProcedure.Instance.PostacbiSignOut(OnLogOutSuccess, OnLogOutFailed );});

        editAvatarButtons.ForEach(button => { button.onClick.AddListener(OpenAvatarEditor); });

        if (ACBSingleton.Instance.AccountData.avatarData.isFirstTime)
            OpenAvatarEditor();

        textVersion.text = "v" + Application.version;
    }

    #endregion

    /// <summary>
    /// Corrutina que desactiva el spinner de carga
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseSpinner()
    {
        spinner.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        spinner.SetActive(false);
    }

    #region Public Methods

    /// <summary>
    /// Activa el panel y actualiza los datos del jugador
    /// </summary>
    /// <param name="onClosePanel">Acción que se ejecuta al cerrar el panel</param>
    public void ActivePanel(Action onClosePanel = null)
    {
        this.onClosePanel = onClosePanel;
        canvasGroup.interactable = true;
        UpdateAccountData();
    }

    /// <summary>
    /// Actualiza los datos del jugador
    /// </summary>
    /// <param name="onUpdateData">Acción que se ejecuta una vez actualiza avatar</param>
    public void UpdateAccountData(Action onUpdateData = null)
    { 
        closeButton.gameObject.SetActive(!ACBSingleton.Instance.AccountData.avatarData.isFirstTime);
        nicknameText.text = ACBSingleton.Instance.AccountData.avatarData.nickName;
        avatarView.UpdateView();
        onUpdateData?.Invoke();
    }

    /// <summary>
    /// Abre panel de editor de avatar
    /// </summary>
    public void OpenAvatarEditor()
    {    
        panelOpenerAvatarEditor.popupPrefab = avatarViewPrefab;
        panelOpenerAvatarEditor.OpenPopup();
        AvatarEditor editor = panelOpenerAvatarEditor.popup.GetComponent<AvatarEditor>();
        editor.OpenEditor(this);
    }

    /// <summary>
    /// Abre panel de cambio de nombre del avatar
    /// </summary>
    public void OpenChangeNamePanel()
    {
        panelOpenerAvatarEditor.popupPrefab = changeNamePanelPrefab;
        panelOpenerAvatarEditor.OpenPopup();
        panelOpenerAvatarEditor.popup.GetComponent<PanelChangeName>().OpenChangeNameView(() => { UpdateAccountData(); });
    }

    /// <summary>
    /// Abre panel de edición de la cuenta
    /// </summary>
    public void OpenEditAccountPanel()
    {
        panelOpenerAvatarEditor.popupPrefab = editAccountPanelPrefab;
        panelOpenerAvatarEditor.OpenPopup();
    }
    
    /// <summary>
    /// Cierra el panel
    /// </summary>
    public void CloseAvatarPanel()
    {
        canvasGroup.interactable = false;
        Close();
    }

    /// <summary>
    /// Error devuelto por backend cuando el jugador no puede cerrar su sesión satisfactoriamente
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnLogOutFailed(WebError obj)
    {
        Debug.Log("LogOut");
    }

    /// <summary>
    /// Método que se ejecuta cuando el jugador sale del juego de forma satisfactoria
    /// </summary>
    /// <param name="obj">Datos de cierre de sesión</param>
    private void OnLogOutSuccess(DataSnapshot obj)
    {
        WebProcedure.Instance.accessData = new UserData();
        Close();
        ACBSingleton.Instance.LogOut();
    }

    #endregion
}
