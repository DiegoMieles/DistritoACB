using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Controla el subpanel que anima la ACBall de inicio de desafio
/// </summary>
public class SubPanelStartChallenge : MonoBehaviour
{
    #region Fields and properties

    [Header("Subpanel Components")]
    [SerializeField] [Tooltip("Texto del t�tulo del subpanel")]
    private Text titleText;
    [SerializeField] [Tooltip("Componente de transformaci�n de la ACBall")]
    private RectTransform acballTransform;

    [Space(5)]
    [Header("Configuration Variables")]
    [SerializeField] [Tooltip("Posici�n inicial de la imagen de la ACBall")]
    private RectTransform startPosition;
    [SerializeField] [Tooltip("Posici�n intermedia de la imagen de la ACBall")]
    private RectTransform middlePosition;
    [SerializeField] [Tooltip("Posici�n final de la imagen de la ACBall")]
    private RectTransform endPosition;
    [SerializeField] [Tooltip("Texto que se debe mostrar en el t�tulo")]
    private string titleString;
    [SerializeField]
    [Tooltip("panel que muestra el logo de la liga actual")]
    private GameObject actualLeaguePanel;
    [SerializeField]
    [Tooltip("panel que muestra el logo de la liga cl�sica")]
    private GameObject clasicLeaguePanel;
    [SerializeField]
    [Tooltip("panel que muestra el logo de la liga actual")]
    private GameObject ACBallPanel;
    private Action onFinishAnimation; //Acci�n que se ejecuta al finaliza la animaci�n de la ACBall

    #endregion

    #region Public Methods

    /// <summary>
    /// Inicia la animaci�n de la ACBall y actualiza el texto del t�tulo
    /// </summary>
    /// <param name="onFinishAnimation">Acci�n que se ejecuta al finaliza la animaci�n de la ACBall</param>
    public void AnimateStartChallenge(Action onFinishAnimation)
    {
        titleText.text = titleString;
        this.onFinishAnimation = onFinishAnimation;
        if(FindObjectOfType<Panels.PanelTablonDesafio>())
        {
            StartCoroutine(ShowLeague(!FindObjectOfType<Panels.PanelTablonDesafio>(true).isClasicLeague));
        }
        if (FindObjectOfType<PanelPavilionField>())
        {
            StartCoroutine(ShowLeague(!FindObjectOfType<PanelPavilionField>(true).isclasicLeague));
        }
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Finaliza la animaci�n de la ACBall
    /// </summary>
    private void EndAnimation()
    {
        StartCoroutine(WaitForAnimationEnd());
    }
    /// <summary>
    /// Corrutina que se encarga de mostrar el logo de la liga antes de empezar la partida
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowLeague(bool isActualLeague)
    {
        if(isActualLeague)
        {
            actualLeaguePanel.SetActive(true);
            actualLeaguePanel.transform.parent.gameObject.SetActive(true);
            actualLeaguePanel.GetComponent<Image>().DOFade(1, 0.2f);
            yield return new WaitForSeconds(1f);
            actualLeaguePanel.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() =>
            {

                ACBallPanel.SetActive(true);
                actualLeaguePanel.SetActive(false);
                actualLeaguePanel.transform.parent.gameObject.SetActive(false);
                acballTransform.anchoredPosition = startPosition.anchoredPosition;
                acballTransform.DOAnchorPos(middlePosition.anchoredPosition, 1f).OnComplete(() => { EndAnimation(); });
            }
            );
        }
        else
        {
            clasicLeaguePanel.SetActive(true);
            clasicLeaguePanel.transform.parent.gameObject.SetActive(true);
            clasicLeaguePanel.GetComponent<Image>().DOFade(1, 0.2f);
            yield return new WaitForSeconds(1f);
            clasicLeaguePanel.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() =>
            {

                ACBallPanel.SetActive(true);
                clasicLeaguePanel.SetActive(false);
                clasicLeaguePanel.transform.parent.gameObject.SetActive(false);
                acballTransform.anchoredPosition = startPosition.anchoredPosition;
                acballTransform.DOAnchorPos(middlePosition.anchoredPosition, 1f).OnComplete(() => { EndAnimation(); });
            }
            );
        }
    }

    /// <summary>
    /// Corrutina que se encarga de la finalizaci�n de la animaci�n de la ACBall
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForAnimationEnd()
    {
        yield return new WaitForSeconds(1.5f);
        acballTransform.anchoredPosition = middlePosition.anchoredPosition;
        acballTransform.DOAnchorPos(endPosition.anchoredPosition, 1f).OnComplete(() => { onFinishAnimation?.Invoke(); });
    }

    #endregion
}
