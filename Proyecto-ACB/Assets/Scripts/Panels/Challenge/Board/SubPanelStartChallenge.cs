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
        acballTransform.anchoredPosition = startPosition.anchoredPosition;
        acballTransform.DOAnchorPos(middlePosition.anchoredPosition, 1f).OnComplete(() => { EndAnimation(); });
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
