using Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    /// <summary>
    /// Controla los datos del botón de desafios del tablón
    /// </summary>
    public class PanelTabloDesafioData : MonoBehaviour
    {
        #region Fields and properties

        [Header("Prefab components")]
        [SerializeField] [Tooltip("Botón de aceptar desafioo")]
        public Button button;
        [SerializeField] [Tooltip("Nombre del rival")]
        private Text textname;
        [SerializeField] [Tooltip("Fecha de publicación del desafio")]
        private Text textfecha;
        [SerializeField] [Tooltip("Vista del rival")]
        private AvatarImageView playerView;
        [SerializeField] [Tooltip("Datos del desafio")]
        private ChallengesTablon.ChallengesTablonItem currentChallenge;

        [Space(5)]
        [Header("Panel opener data")]
        [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
        private PanelOpener panelOpener;
        [SerializeField] [Tooltip("Prefab del panel de equipo competitivo incompleto")]
        private GameObject challengeTeamImcompleteAlertPrefab;
        [SerializeField] [Tooltip("Panel de vista del avatar antes del desafio")]
        private GameObject avatarBoardViewPanel;

        private bool teamIsFull; //Determina si el equipo competitivo está completo
        private bool accepChallenge; //Determina si se puede aceptar el desafio
        private bool freeChallange; //Determina si el desafio es gratuito
        private string messagefreechallenge; //Mensaje del desafio gratuito
        private string noCanAcceptMessage; //Mensaje de desafio que no se puede aceptar por falta de cartas en el equipo competitivo
        private Action onFinishedChallenge; //Acción que se ejecuta al finalizar el desafio

        #endregion

        #region Public Methods

        /// <summary>
        /// Muestra los datos del desafio
        /// </summary>
        /// <param name="challengeData">Datos del desafio</param>
        /// <param name="teamIsFull">Determina si el equipo competitivo está completo</param>
        /// <param name="onFinishedChallenge">Acción que se ejecuta al finalizar el desafio</param>
        /// <param name="accepChallenge">Determina si se puede aceptar el desafio</param>
        /// <param name="freeChallange">Determina si el desafio es gratuito</param>
        /// <param name="messagefreechallenge">Mensaje del desafio gratuito</param>
        /// <param name="nocanacceptmessage">Mensaje de desafio que no se puede aceptar por falta de cartas en el equipo competitivo</param>
        public void ShowInfo(ChallengesTablon.ChallengesTablonItem challengeData, bool teamIsFull, Action onFinishedChallenge, bool accepChallenge, bool freeChallange, string messagefreechallenge, string nocanacceptmessage)
        {
            this.onFinishedChallenge = onFinishedChallenge;
            playerView.UpdateView(challengeData, false);
            currentChallenge = challengeData;
            this.teamIsFull = teamIsFull;
            this.accepChallenge = accepChallenge;
            this.freeChallange = freeChallange;
            this.messagefreechallenge = messagefreechallenge;
            this.noCanAcceptMessage = nocanacceptmessage;
            
            textname.text = currentChallenge.nickName;
            textfecha.text = currentChallenge.created.ToString();
            button.gameObject.SetActive(challengeData.show);
            button.onClick.AddListener(OpenChallenge);
            CloseSpinner();
        }

        #endregion

        #region Inner Methods
        
        /// <summary>
        /// Abre panel de desafio
        /// </summary>
        private void OpenChallenge()
        {
            if (teamIsFull)
            {
                if (accepChallenge)
                {
                    panelOpener.popupPrefab = avatarBoardViewPanel;
                    panelOpener.OpenPopup();
                    panelOpener.popup.GetComponent<PanelTablonAvatar>().SetAvatarData(currentChallenge, onFinishedChallenge, true,freeChallange,  messagefreechallenge);
                }
                else
                {
                    panelOpener.popupPrefab = challengeTeamImcompleteAlertPrefab;
                    panelOpener.OpenPopup();
                    panelOpener.popup.GetComponent<ChallengeIncompleteTeamPanel>().OpenAlertNoCanAcceptChallenge(noCanAcceptMessage);  
                }
            }
            else
            {
                panelOpener.popupPrefab = challengeTeamImcompleteAlertPrefab;
                panelOpener.OpenPopup();
                panelOpener.popup.GetComponent<ChallengeIncompleteTeamPanel>().OpenAlertNotTeam();  
            }

        }

        /// <summary>
        /// Oculta el spinner de carga
        /// </summary>
        private void CloseSpinner()
        {
            GameObject spinner = GameObject.Find("Spinner_TablonDesafio");
            for(int i=0; i<spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        #endregion
    }
}

