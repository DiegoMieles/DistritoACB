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
        acballTransform.anchoredPosition = startPosition.anchoredPosition;
        acballTransform.DOAnchorPos(middlePosition.anchoredPosition, 1f).OnComplete(() => { EndAnimation(); });
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
