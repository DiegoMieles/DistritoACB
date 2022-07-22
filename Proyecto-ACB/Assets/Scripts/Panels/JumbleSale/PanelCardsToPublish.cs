using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel de añadir carta al equipo
/// </summary>
public class PanelCardsToPublish : Panel
{
    #region Fields and Properties
    //Evento que se dispara al completar la publicación en el mercadillo
    public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
    [SerializeField] [Tooltip("Determina si es colección")]
    private bool isColeccion;
    [Header("Panel Components")]
    [SerializeField] [Tooltip("Texto del título del panel")]
    private Text textTitle;
    [SerializeField] [Tooltip("Contenedor de las cartas")]
    private GridLayoutGroup gridLayoutGroupCards;
    [SerializeField] [Tooltip("Panel de la carta")]
    private PanelCardItem panelCardItem;
    [SerializeField] [Tooltip("Acción que se ejecuta cuando backend no puede traer datos")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Datos de la carta")]
    private CardDataToPublish cardContainer = new CardDataToPublish();
    [SerializeField]
    [Tooltip("Contenedor de cartas de subcolección de highlights ")]
    private SubCollectionHiglightDataToPublish subcolectionHiglightDataContainer = new SubCollectionHiglightDataToPublish();
    [SerializeField] [Tooltip("Datos del contenedor de las cartas")]
    private HigthlightContainer higthlightContainer = new HigthlightContainer();
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;
    [SerializeField, TextArea] [Tooltip("Texto que se muestra cuando no se puede traer datos de backend")]
    private string textFail;
    [SerializeField] [Tooltip("Texto que se muestra cuando no hay cartas")]
    private Text textNoPotenciadores;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Suscribe acción de cerrar panel del equipo competitivo
    /// </summary>
    private void OnEnable()
    {
        PanelTeamCompetitivo.OnClose += Close;
    }

    /// <summary>
    /// Desuscribe el evento de cerrar panel al destruirse el objeto
    /// </summary>
    private void OnDestroy()
    {
        PanelTeamCompetitivo.OnClose -= Close;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Trae la información de subcolección de carta
    /// </summary>
    /// <param name="subcollectionid">ID de la subcolección</param>
    /// <param name="title">Título de la subcolección</param>
    public void CallInfo(int subcollectionid, string title)
    {
        textTitle.text = title;
        var cardbody = JsonConvert.SerializeObject(new BodySubcollection(){ collection_id = subcollectionid});

        if (isColeccion)
        {
            WebProcedure.Instance.GetCardsToSell(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, cardContainer);
                Debug.Log(snapshot.RawJson);
            
                if (cardContainer.items != null && cardContainer.items.Count > 0)
                {
                    foreach (var transactionData in cardContainer.items)
                    {
                        if (gridLayoutGroupCards)
                        {
                            var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                            prefab.ShowInfo(transactionData, null,true);
                            prefab.OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
                            textNoPotenciadores.text = string.Empty;
                        }
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }

                ClosedSpinner();
            
            }, error =>
            {
                onFailed.Invoke();

                ClosedSpinner();
            });
        }
        else
        {
            WebProcedure.Instance.GetSubCollectionHighlightsToSell(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, subcolectionHiglightDataContainer);
                Debug.Log(snapshot.RawJson);
            
                if (subcolectionHiglightDataContainer.items != null && subcolectionHiglightDataContainer.items.Count > 0)
                {
                    foreach (var transactionData in subcolectionHiglightDataContainer.items)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                        prefab.ShowInfo(transactionData, null,true);
                        prefab.OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
                        textNoPotenciadores.text = string.Empty;
                    }
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }

                ClosedSpinner();

            }, error =>
            {
                onFailed.Invoke();
                textNoPotenciadores.text = textFail;
                ClosedSpinner();
            });
        }
    }

    /// <summary>
    /// Trae la información de colección de carta
    /// </summary>
    /// <param name="collectionid">Id de la colección</param>
    /// <param name="title">Título de la colección</param>
    public void CallInfoC(int collectionid, string title)
    {   
        textTitle.text = title;
        var cardbody = JsonConvert.SerializeObject(new BodySubcollection(){ collection_id = collectionid});

        if (isColeccion)
        {
            WebProcedure.Instance.GetSubCollectionsToSell(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, cardContainer);
                Debug.Log(snapshot.RawJson);
            
                if (cardContainer.items != null && cardContainer.items.Count > 0)
                {
                    foreach (var transactionData in cardContainer.items)
                    {
                        if (gridLayoutGroupCards)
                        {
                            var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                            prefab.ShowInfo(transactionData, null,true);
                            textNoPotenciadores.text = string.Empty;
                        }
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }
                
                ClosedSpinner();
            
            }, error =>
            {
                onFailed.Invoke();
                textNoPotenciadores.text = textFail;
                ClosedSpinner();
            }); 
        }
        else
        {
            WebProcedure.Instance.GetUserSubCollectionTC(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, cardContainer);
                Debug.Log(snapshot.RawJson);
            
                if (cardContainer.items != null && cardContainer.items.Count > 0)
                {
                    foreach (var transactionData in cardContainer.items)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                        prefab.ShowInfo(transactionData, null,true);
                        textNoPotenciadores.text = string.Empty;
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }
                
                ClosedSpinner();
            
            }, error =>
            {
                onFailed.Invoke();
                ClosedSpinner();
                textNoPotenciadores.text = textFail;
            });  
        }
    }

    /// <summary>
    /// Trae la información de colección de highlights
    /// </summary>
    /// <param name="collectionid">Id de la colección</param>
    /// <param name="title">Título de la colección</param>
    public void CallInfoCHighLight(int collectionid, string title)
    {   
        textTitle.text = title;
        var cardbody = JsonConvert.SerializeObject(new BodySubcollection(){ collection_id = collectionid});

        if (isColeccion)
        {
            WebProcedure.Instance.GetSubCollectionCL(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, higthlightContainer);
                Debug.Log(snapshot.RawJson);
            
                if (higthlightContainer.higthlightData.higthlightItems != null && higthlightContainer.higthlightData.higthlightItems.Count > 0)
                {
                    foreach (var transactionData in higthlightContainer.higthlightData.higthlightItems)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                        prefab.ShowInfo(transactionData, null);
                        textNoPotenciadores.text = string.Empty;
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }
                
                ClosedSpinner();

            }, error =>
            {
                onFailed.Invoke();
                textNoPotenciadores.text = textFail;
                ClosedSpinner();
            }); 
        }
        else
        {
            WebProcedure.Instance.GetUserSubCollectionTC(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, higthlightContainer);
                Debug.Log(snapshot.RawJson);
            
                if (higthlightContainer.higthlightData.higthlightItems != null && higthlightContainer.higthlightData.higthlightItems.Count > 0)
                {
                    foreach (var transactionData in higthlightContainer.higthlightData.higthlightItems)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                        prefab.ShowInfo(transactionData, null);
                        textNoPotenciadores.text = string.Empty;
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }
                
                ClosedSpinner();
            
            }, error =>
            {
                onFailed.Invoke();
                textNoPotenciadores.text = textFail;
                ClosedSpinner();
            });  
        }
 
    }
    
    /// <summary>
    /// Trae la información de subcolección de highlights
    /// </summary>
    /// <param name="subcollectionid">ID de la subcolección</param>
    /// <param name="title">Título de la subcolección</param>
    public void CallInfoCHighLightSub(int subcollectionid, string title)
    {   
        textTitle.text = title;
        var cardbody = JsonConvert.SerializeObject(new BodyCard(){ subcollection_id = subcollectionid}) ;

        if (isColeccion)
        {
            WebProcedure.Instance.GetCardCL(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, higthlightContainer);
                Debug.Log(snapshot.RawJson);
            
                if (higthlightContainer.higthlightData.higthlightItems != null && higthlightContainer.higthlightData.higthlightItems.Count > 0)
                {
                    foreach (var transactionData in higthlightContainer.higthlightData.higthlightItems)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                        prefab.ShowInfo(transactionData, null);
                        textNoPotenciadores.text = string.Empty;
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }
                
                ClosedSpinner();

            }, error =>
            {
                onFailed.Invoke();
                textNoPotenciadores.text = textFail;
                ClosedSpinner();
            }); 
        }
        else
        {
            WebProcedure.Instance.GetUserCardTC(cardbody,snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, higthlightContainer);
                Debug.Log(snapshot.RawJson);
            
                if (higthlightContainer.higthlightData.higthlightItems != null && higthlightContainer.higthlightData.higthlightItems.Count > 0)
                {
                    foreach (var transactionData in higthlightContainer.higthlightData.higthlightItems)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);
                        prefab.ShowInfo(transactionData, null);
                        textNoPotenciadores.text = string.Empty;
                    } 
                }
                else
                {
                    textNoPotenciadores.text = textFail;
                }
                
                ClosedSpinner();
            
            }, error =>
            {
                onFailed.Invoke();
                textNoPotenciadores.text = textFail;
                ClosedSpinner();
            });  
        }
    }

    /// <summary>
    /// Desactiva el spinner de carga
    /// </summary>
    private void ClosedSpinner()
    {
        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    #endregion

}
