using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase base del panel que se encarga del control de las acciones y propiedades más básicas
/// </summary>
public class Panel : MonoBehaviour
{
    [SerializeField] [Tooltip("Color del fondo del panel")]
    private Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

    private GameObject m_background; //Imagen del fondo del panel

    /// <summary>
    /// Abre panel y añade background
    /// </summary>
    public void Open()
    {
        AddBackground();
    }

    /// <summary>
    /// Cierra panel
    /// </summary>
    public void Close()
    {
        var animator = GetComponent<Animator>();

        if (animator)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                animator.Play("Close");
        }

        RemoveBackground();
        StartCoroutine(RunPopupDestroy());
    }

    /// <summary>
    /// Corrutina que se encarga de cerrar el panel después de cierto tiempo
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunPopupDestroy()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Destroy(m_background);
        Destroy(gameObject);
    }

    /// <summary>
    /// Añade el backgound al panel
    /// </summary>
    private void AddBackground()
    {
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, backgroundColor);
        bgTex.Apply();

        m_background = new GameObject("PopupBackground");

        var image = m_background.AddComponent<Image>();
        var rect = new Rect(0, 0, bgTex.width, bgTex.height);
        var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        var newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(1.0f, 0.4f, false);

        var canvas = GameObject.Find("Canvas");
        m_background.transform.localScale = new Vector3(1, 1, 1);
        m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        m_background.transform.SetParent(canvas.transform, false);
        m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    /// <summary>
    /// Elimina el background del panel
    /// </summary>
    private void RemoveBackground()
    {
        if (m_background)
        {
            var image = m_background.GetComponent<Image>();
            if (image != null)
                image.CrossFadeAlpha(0.0f, 0.2f, false);
        }
    }
}