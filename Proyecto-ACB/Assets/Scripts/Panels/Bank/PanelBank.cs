using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

namespace Panels
{
    /// <summary>
    /// Controla el panel de banco y los datos econ�micos del jugador dentro del juego
    /// </summary>
    public class PanelBank : Panel
    {
        #region Fields and Properties

        [SerializeField] [Tooltip("Espacio arrastrable donde se van a mostrar las transacciones del jugador")]
        private ScrollRect scrollRectTransactions;
        [SerializeField] [Tooltip("Texto que muestra la cantidad actual de monedas")]
        private Text textCoins;
        [SerializeField] [Tooltip("Valor m�ximo a mostrar en el texto")]
        private float limit;
        [SerializeField] [Tooltip("Prefab del objeto que contiene una transacci�n espec�fica del jugador")]
        private GameObject panelBankTransaction;
        [SerializeField] [Tooltip("Objeto que contiene los datos principales de la cuenta del jugador")]
        private ScriptableAccount scriptableAccount;
        [SerializeField] [Tooltip("Evento que se ejecuta al fallar la carga de los datos de transacci�n")]
        private UnityEvent onFailed;
        [SerializeField] [Tooltip("Datos de transacci�n del jugador")]
        private TransactionContainer transactionDataContainer = new TransactionContainer();
        [SerializeField] [Tooltip("Bot�n que se encarga del cerrado del panel")]
        private Button closeButton;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Se ejecuta cuando se muestra el panel, cargando la informaci�n de las transacciones y configurando el bot�n
        /// de salir del panel
        /// </summary>
        private void OnEnable()
        {
            closeButton.onClick.AddListener(() => { ACBSingleton.Instance.UpdateGameData(); Close(); });
            CallInfo();
        }

        #endregion

        #region Inner Methods

        /// <summary>
        /// Carga los datos de transacci�n del jugador y los muestra en el espacio arrastrable
        /// </summary>
        private void CallInfo()
        {
            WebProcedure.Instance.GetBankTransaction(snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, transactionDataContainer);
                Debug.Log(snapshot.RawJson);
                textCoins.text = Mathf.Clamp(scriptableAccount.statsData.coinsBalance, 0, limit).ToString();
                
                if (transactionDataContainer.transactionData.transactionItems != null && transactionDataContainer.transactionData.transactionItems.Count > 0)
                {
                    scrollRectTransactions.content.sizeDelta = new Vector2(scrollRectTransactions.content.sizeDelta.x, (transactionDataContainer.transactionData.transactionItems.Count + 1) * panelBankTransaction.GetComponent<LayoutElement>().preferredHeight);

                    foreach (var transactionData in transactionDataContainer.transactionData.transactionItems)
                    {
                        var prefab= Instantiate(panelBankTransaction, scrollRectTransactions.content);
                        prefab.GetComponent<PanelBankTransaction>().ShowInfo(transactionData);
                    } 
                }
                else
                {
                    SetBankSpiner();
                }
            }, error =>
            {
                onFailed.Invoke();
                SetBankSpiner();
            });
        }

        /// <summary>
        /// Actualmente desactiva el spinner de carga en el banco
        /// </summary>
        private void SetBankSpiner()
        {
            GameObject spinner = GameObject.Find("Spinner_bank");
            for (int i = 0; i < spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
