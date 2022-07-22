using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    /// <summary>
    /// Objeto que contiene los datos de una transacción específica del jugador
    /// </summary>
   public class PanelBankTransaction : MonoBehaviour
   {
        [SerializeField] [Tooltip("Texto con la fecha de transacción")]
        private Text textDate;
        [SerializeField] [Tooltip("Texto con el concepto/descripción de la transacción")]
        private Text textConcept;
        [SerializeField] [Tooltip("Texto con el valor de la transacción")]
        private Text textSpend;
        [SerializeField] [Tooltip("Imagen de la moneda")]
        private Image imageSped;
        [SerializeField] [Tooltip("Texto con las ACBCoins restantes de la transacción")]
        private Text textBalance;
        [SerializeField] [Tooltip("Máximo valor a mostrar de monedas en el texto")]
        private float limit;
        [SerializeField] [Tooltip("Color de la transacción cuando ha sido gasto de ACBCoins")]
        private Color32 colorLess;
        [SerializeField] [Tooltip("Color de la transacción cuando ha sido ganancia de ACBCoins")]
        private Color32 colorMore;
        [SerializeField] [Tooltip("Imagen representativa de la transacción cuando ha sido ganancia de ACBCoins")]
        private Sprite spriteMore;
        [SerializeField] [Tooltip("Imagen representativa de la transacción cuando ha sido gasto de ACBCoins")]
        private Sprite spriteLess;

        /// <summary>
        /// Configura y muestra la información de la transacción
        /// </summary>
        /// <param name="transactionData">Datos de la transacción</param>
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
