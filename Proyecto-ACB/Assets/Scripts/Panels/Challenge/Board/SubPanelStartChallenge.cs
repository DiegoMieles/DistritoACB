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
    [SerializeField] [Tooltip("Texto del título del subpanel")]
    private Text titleText;
    [SerializeField] [Tooltip("Componente de transformación de la ACBall")]
    private RectTransform acballTransform;

    [Space(5)]
    [Header("Configuration Variables")]
    [SerializeField] [Tooltip("Posición inicial de la imagen de la ACBall")]
    private RectTransform startPosition;
    [SerializeField] [Tooltip("Posición intermedia de la imagen de la ACBall")]
    private RectTransform middlePosition;
    [SerializeField] [Tooltip("Posición final de la imagen de la ACBall")]
    private RectTransform endPosition;
    [SerializeField] [Tooltip("Texto que se debe mostrar en el título")]
    private string titleString;
    [SerializeField]
    [Tooltip("panel que muestra el logo de la liga actual")]
    private GameObject actualLeaguePanel;
    [SerializeField]
    [Tooltip("panel que muestra el logo de la liga clásica")]
    private GameObject clasicLeaguePanel;
    [SerializeField]
    [Tooltip("panel que muestra el logo de la liga actual")]
    private GameObject ACBallPanel;
    private Action onFinishAnimation; //Acción que se ejecuta al finaliza la animación de la ACBall

    #endregion

    #region Public Methods

    /// <summary>
    /// Inicia la animación de la ACBall y actualiza el texto del título
    /// </summary>
    /// <param name="onFinishAnimation">Acción que se ejecuta al finaliza la animación de la ACBall</param>
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
    /// Finaliza la animación de la ACBall
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
    /// Corrutina que se encarga de la finalización de la animación de la ACBall
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
