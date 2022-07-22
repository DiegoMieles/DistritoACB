using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    /// <summary>
    /// Objeto que contiene los datos de una transacci�n espec�fica del jugador
    /// </summary>
   public class PanelBankTransaction : MonoBehaviour
   {
        [SerializeField] [Tooltip("Texto con la fecha de transacci�n")]
        private Text textDate;
        [SerializeField] [Tooltip("Texto con el concepto/descripci�n de la transacci�n")]
        private Text textConcept;
        [SerializeField] [Tooltip("Texto con el valor de la transacci�n")]
        private Text textSpend;
        [SerializeField] [Tooltip("Imagen de la moneda")]
        private Image imageSped;
        [SerializeField] [Tooltip("Texto con las ACBCoins restantes de la transacci�n")]
        private Text textBalance;
        [SerializeField] [Tooltip("M�ximo valor a mostrar de monedas en el texto")]
        private float limit;
        [SerializeField] [Tooltip("Color de la transacci�n cuando ha sido gasto de ACBCoins")]
        private Color32 colorLess;
        [SerializeField] [Tooltip("Color de la transacci�n cuando ha sido ganancia de ACBCoins")]
        private Color32 colorMore;
        [SerializeField] [Tooltip("Imagen representativa de la transacci�n cuando ha sido ganancia de ACBCoins")]
        private Sprite spriteMore;
        [SerializeField] [Tooltip("Imagen representativa de la transacci�n cuando ha sido gasto de ACBCoins")]
        private Sprite spriteLess;

        /// <summary>
        /// Configura y muestra la informaci�n de la transacci�n
        /// </summary>
        /// <param name="transactionData">Datos de la transacci�n</param>
        public void ShowInfo(TransactionData.TransactionItemData transactionData)
        {
            textDate.text = transactionData.date;
            textConcept.text = transactionData.concept;
            textBalance.text = Mathf.Clamp(transactionData.balance, 0, limit).ToString();
            textSpend.text = transactionData.spend.ToString();
            textSpend.color = transactionData.type == TransactionType.EGRESO ? colorLess : colorMore;
            imageSped.sprite = transactionData.type == TransactionType.EGRESO  ? spriteLess  : spriteMore;
            GameObject spinner = GameObject.Find("Spinner_bank");
            for(int i=0; i<spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
   
   }
}
