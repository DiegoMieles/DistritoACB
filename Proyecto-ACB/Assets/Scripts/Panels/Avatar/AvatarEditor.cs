using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Data;
using WebAPI;

/// <summary>
/// Controlador del panel de edición del avatar
/// </summary>
public class AvatarEditor : Panel
{
    [Header("Cameras")]
    [SerializeField] [Tooltip("Cámara con vista grande del avatar")]
    private Camera cameraBig;
    [SerializeField] [Tooltip("Cámara con vista pequeña del avatar")]
    private Camera cameraSmall;
    
    [Header("Panels References")]
    [SerializeField] [Tooltip("Panel de vista principal del avatar")]
    private PanelAvatar panelAvatar;
    [SerializeField] [Tooltip("Vista del avatar")]
    private AvatarView avatarView;

    [Space(5)]
    [Header("Panel components")]
    [SerializeField] [Tooltip("Lita de Categorías/Atributos del avatar del jugador")]
    private ChangeCategory[] changeCategories;
    [SerializeField] [Tooltip("Selección de categoria")]
    private CategorySelection categorySelection;
    [SerializeField] [Tooltip("Botones de tipo de skin")]
    private GameObject topButtons;
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Botón de guardado del avatar")]
    private Button saveButton;
    [SerializeField] [Tooltip("Botón para aleatorizar el avatar")]
    private Button shuffleButton;

    private bool avatarHasChanges; //Determina si el avatar tiene cambios registrados
    private string avatarsmallImageString; //Nombre de la imagen del avatar pequeña
    private string avatarbigImageString; //Nombre de la imagen del avatar grande
    [SerializeField] [Tooltip("Datos del avatar")]
    private AvatarEditorData avatarMenuData;

    public bool AvatarHasChanges { get => avatarHasChanges; set => avatarHasChanges = value; }
    public AvatarView AvatarView => avatarView;
    
    #region Public Methods

    /// <summary>
    /// Abre editor de avatar y lo configura
    /// </summary>
    /// <param name="panel"></param>
    public void OpenEditor(PanelAvatar panel)
    {
        panelAvatar = panel;
        ACBSingleton.Instance.ActivateMainSpinner(true);
        avatarView.gameObject.SetActive(false);
        WebProcedure.Instance.GetAvatarInfo(OnSuccessLoadingAvatarData, OnFailedLoadingAvatarData);
    }

    /// <summary>
    /// Reasigna los objeto al seleccionar una categoría
    /// </summary>
    /// <param name="typeCategory"></param>
    public void OnSelectCategory(TypeCategory typeCategory)
    {
        if(typeCategory != TypeCategory.Look)
        {
            bool showAlert = true;

            if(ACBSingleton.Instance.AccountData.avatarData.avatarItems != null)
            {
                for(int i = 0; i < ACBSingleton.Instance.AccountData.avatarData.avatarItems.Count; i++)
                {
                    ItemData itemData = ACBSingleton.Instance.AccountData.avatarData.avatarItems[i];
                    if ((itemData.itemType == ItemType.BODYACCESORY || itemData.itemType == ItemType.HEADACCESORY || itemData.itemType == ItemType.ARMACCESORY || 
                        itemData.itemType == ItemType.EYEACCESORY) && typeCategory == TypeCategory.Clothes)
                    {
                        showAlert = false;
                        break;
                    }
                    else if((itemData.itemType == ItemType.BACKGROUNDACCESORY || itemData.itemType == ItemType.FOREGROUNDACCESORY) && typeCategory == TypeCategory.BackGround)
                    {
                        showAlert = false;
                        break;
                    }
                }
            }

            if(showAlert)
            {
                ACBSingleton.Instance.AlertPanel.SetupPanel("Aún no tienes skins", "", false, null);

                for (int i = 0; i < changeCategories.Length; i++)
                    changeCategories[i].OnDesactiveCategory();

                changeCategories[0].OnChangeCategory();
                return;
            }
        }

        for(int i = 0; i < changeCategories.Length; i++)
        {
            if(changeCategories[i].TypeCategory != typeCategory)
                changeCategories[i].OnDesactiveCategory();
            else
            {
                categorySelection.InitCategory(typeCategory);
            }
        }
    }

    /// <summary>
    /// Método que se ejecuta cuando un botón es seleccionado
    /// </summary>
    /// <param name="typeButton">Tipo de botón</param>
    /// <param name="layoutItems">Datos del avatar</param>
    /// <param name="useNullButton">Determina si tiene botón nulo</param>
    /// <param name="loadFromGameData">Determina si se cargan los datos del alamacenamiento</param>
    public void OnSelectItems(ItemType typeButton, AvatarLayoutItems layoutItems, bool useNullButton, bool loadFromGameData)
    {
        List<ItemData> items = new List<ItemData>();
        List<string> itemName = (typeButton == ItemType.BODYFORM) ? new List<string>() : null;

        List<ItemData> listToCheck = loadFromGameData ? avatarMenuData.avatarMenuData.itemsFree : ACBSingleton.Instance.AccountData.avatarData.avatarItems;

        foreach (ItemData gameDataItem in listToCheck)
        {
            if (gameDataItem.itemType == typeButton)
            {
                items.Add(gameDataItem);
                
                if(typeButton == ItemType.BODYFORM)
                {
                    itemName.Add(LoadUndershirtSizeName(gameDataItem.id));
                }
            }
        }

        if(items.Count > 0)
        {
            layoutItems.Init(this);
            bool isColorButton = typeButton == ItemType.HAIRCOLOR || typeButton == ItemType.SKINCOLOR;
            layoutItems.ActiveButtons(items, useNullButton, isColorButton, loadFromGameData, itemName);
        }
        else
        {
            layoutItems.DesactiveButtons();
            SetAlertText(typeButton);
        }
    }

    /// <summary>
    /// Determina si esconde los botones del editor
    /// </summary>
    /// <param name="hide">Determina el valor de activación de los botones</param>
    public void HideButtons(bool hide)
    {
        topButtons.SetActive(!hide);
        closeButton.gameObject.SetActive(!hide);
        saveButton.gameObject.SetActive(!hide);
    }

    /// <summary>
    /// Abre panel de guardar avatar
    /// </summary>
    public void OpenSaveAvatarPanel()
    {
        StartCoroutine(TakeScreenShot());
    }

    /// <summary>
    /// Método que se ejecuta al guardar el avatar
    /// </summary>
    public void OnSaveAvatar()
    {
        ACBSingleton.Instance.SetCanvasRenderModeAsCamera(true);
        avatarView.SaveAvatar(() => { panelAvatar.UpdateAccountData(Close); }, avatarsmallImageString, avatarbigImageString);
        transform.SetAsFirstSibling();
        panelAvatar.CloseButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Cierra el panel
    /// </summary>
    public void OnCloseEditor()
    {
        avatarView.ResetCachedData();
        if (avatarHasChanges)
            ACBSingleton.Instance.AlertPanel.SetupPanel("¿Deseas salir del editor? No se guardarán tus cambios", "", true, () => { avatarView.ResetCachedData(); Close(); });
        else
            Close();
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que se ejecuta cuando se han podido cargar los datos de avarar desde backend
    /// </summary>
    /// <param name="obj">Datos de carga del avatar</param>
    private void OnSuccessLoadingAvatarData(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        avatarMenuData = new AvatarEditorData();
        JsonConvert.PopulateObject(obj.RawJson, avatarMenuData);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData, new JsonSerializerSettings( ){ObjectCreationHandling = ObjectCreationHandling.Replace});

        avatarView.UpdateView();
        categorySelection.InitColors(ACBSingleton.Instance.AvatarExtraData.skinColor);
        categorySelection.InitHairColors(ACBSingleton.Instance.AvatarExtraData.hairColors);
        shuffleButton.onClick.AddListener(SetRandomAvatarView);

        closeButton.onClick.AddListener(OnCloseEditor);
        saveButton.onClick.AddListener(OpenSaveAvatarPanel);

        for (int i = 0; i < changeCategories.Length; i++)
        {
            changeCategories[i].Init(this);
        }

        changeCategories[0].OnChangeCategory();
        ACBSingleton.Instance.ActivateMainSpinner(false);
        avatarView.gameObject.SetActive(true);
    }

    /// <summary>
    /// Método que se ejecuta cuando falla el cargado de datos del avatar 
    /// </summary>
    /// <param name="error">Clase con los datos de error de avatar</param>
    private void OnFailedLoadingAvatarData(WebError error)
    {
        ACBSingleton.Instance.ActivateMainSpinner(false);
        avatarView.gameObject.SetActive(true);
        ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, Close);
    }

    /// <summary>
    /// Determina el estado de activación de los botones principales del panel
    /// </summary>
    /// <param name="state">Estdo de activaciónde los botones</param>
    private void SetButtonsActiveState(bool state)
    {
        closeButton.gameObject.SetActive(state);
        saveButton.gameObject.SetActive(state);
        shuffleButton.gameObject.SetActive(state);
    }

    /// <summary>
    /// Toma pantallazo del avatar, que permite cargar todas la imagenes de avatar
    /// </summary>
    /// <param name="isBigImage">Determina si la imagen es grande o no</param>
    /// <returns></returns>
    private string TakeAvatarScreenShot(bool isBigImage)
    {
        var image = RTImage(isBigImage);
        var bytes = image.EncodeToPNG();
        Debug.Log(GetSavePath("source", isBigImage));
        File.WriteAllBytes(GetSavePath("source", isBigImage), bytes);
        var i64 = Convert.ToBase64String(bytes); ;
        return i64;
    }

    /// <summary>
    /// Corrutina de toma del pantallazo
    /// </summary>
    /// <returns></returns>
    private IEnumerator TakeScreenShot()
    {
        SetButtonsActiveState(false);
        ACBSingleton.Instance.SetCanvasRenderModeAsCamera(false);
        avatarsmallImageString = string.Empty;
        avatarbigImageString = string.Empty;
        avatarbigImageString = TakeAvatarScreenShot(true);
        yield return new WaitUntil(() => !string.IsNullOrEmpty(avatarbigImageString));
        avatarsmallImageString = TakeAvatarScreenShot(false);
        yield return new WaitUntil(() => !string.IsNullOrEmpty(avatarsmallImageString));
        ACBSingleton.Instance.AlertPanel.SetupPanel("El avatar ha sido actualizado con éxito", "", false, OnSaveAvatar);
    }

    /// <summary>
    /// Devuelve el pantallazo como una tetura
    /// </summary>
    /// <param name="isBigImage">Determina si la imagen es grande o no</param>
    /// <returns></returns>
    private Texture2D RTImage(bool isBigImage)
    {
        Camera cam = isBigImage ? cameraBig : cameraSmall;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;
        
        Destroy(image);

        return image;
    }

    /// <summary>
    /// Devuelve la ruta de guardado del archivo
    /// </summary>
    /// <param name="folderName">Nombre de la carpeta donde se va a guardar el archivo</param>
    /// <param name="isBigImage">Determina si la imagen es grande o no</param>
    /// <returns></returns>
    private string GetSavePath(string folderName, bool isBigImage)
    {
        string directoryPath = Application.persistentDataPath + "/" + folderName + "/";
        string fileName = isBigImage ? "bigImage.png": "smallImage.png";

        if (!Directory.Exists(directoryPath))
        {
            //Create if it doesn't exist yet
            Directory.CreateDirectory(directoryPath);
            return directoryPath + fileName;
        }

        return directoryPath + fileName;
    }

    /// <summary>
    /// Aleatoriza la apariencia del avatar cambiando atributos físicos
    /// </summary>
    private void SetRandomAvatarView()
    {
        shuffleButton.interactable = false;
        List<ItemData> backendItem = avatarMenuData.avatarMenuData.itemsFree;
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(22, 41)]); //Random mouth
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(42, 51)]); //Random faceform
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(52, 71)]); //Random eyebrows
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(72, 109)]); //Random eyes
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(110, 129)]); //Random nose
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(130, 135)]); //Random ear
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(136, 141)]); //Random bodyform
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(148, 170)]); //Random hair
        avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(195, 202)]); //Random facial hair

        float randomBackHairProbability = UnityEngine.Random.Range(0f, 1f);

        if (randomBackHairProbability > 0.5f)
            avatarView.SetIconInAvatar(backendItem[UnityEngine.Random.Range(171, 173)]); //Random backHair
        else
        {
            ItemData nullHairItemData = new ItemData() { id = -1, itemType = ItemType.HAIRSTYLEBACK };
            avatarView.SetIconInAvatar(nullHairItemData);
        }
        shuffleButton.interactable = true;
    }

    /// <summary>
    /// Muestra alerta cuando el jugador no posee determinados items
    /// </summary>
    /// <param name="typeButton">Tipo de item</param>
    private void SetAlertText(ItemType typeButton)
    {
        string alertText = "";

        switch (typeButton)
        {
            case ItemType.BODYACCESORY:
                alertText = "Aún no tienes camisetas/abrigos";
                break;

            case ItemType.HEADACCESORY:
                alertText = "Aún no tienes accesorios de cabeza";
                break;

            case ItemType.ARMACCESORY:
                alertText = "Aún no tienes cinta de brazo";
                break;

            case ItemType.EYEACCESORY:
                alertText = "Aún no tienes gafas de sol";
                break;

            case ItemType.BACKGROUNDACCESORY:
                alertText = "Aún no tienes fondos";
                break;

            case ItemType.FOREGROUNDACCESORY:
                alertText = "Aún no tienes complementos fondos";
                break;

            case ItemType.FACIALHAIR:
                alertText = "Aún no tienes vello facial";
                break;
        }
        
        ACBSingleton.Instance.AlertPanel.SetupPanel(alertText, "", false, null);
    }

    /// <summary>
    /// Devuelve el tamaño de la camisilla de acuerdo a la complexión del cuerpo del avatar
    /// </summary>
    /// <param name="bodyFormId">Id de la forma del cuerpo</param>
    /// <returns></returns>
    private string LoadUndershirtSizeName(int bodyFormId)
    {
        List<UnderShirtReferenceData> underShirtReference = ACBSingleton.Instance.AvatarExtraData.underShirts;

        for (int i = 0; i < underShirtReference.Count; i++)
        {
            UnderShirtReferenceData actualUnderShirtData = underShirtReference[i];
            for (int j = 0; j < actualUnderShirtData.bodyReferences.Count; j++)
            {
                if (actualUnderShirtData.bodyReferences[j] == bodyFormId)
                {
                    return actualUnderShirtData.undershirtSizeName;
                }
            }
        }

        return string.Empty;
    }

#endregion
}
