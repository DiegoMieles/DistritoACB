using UnityEngine;
using UnityEngine.Events;

public class InsideRectEvent : MonoBehaviour
{
    #region Fields and Properties

    [Header("Actions")]
    [SerializeField] private UnityEvent onInsideAction = null;
    [SerializeField] private UnityEvent onOutiseAction = null;
    [Header("Conditionals")]
    [SerializeField] private RectTransform overlaping = null;
    [SerializeField] private RectTransform overlaped = null;
    [SerializeField] string overlapedName = "";
    internal bool isInside { get; private set; } = true;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (overlaping == null)
        {
            overlaping = GetComponent<RectTransform>();
        }
        if (overlaped == null)
        {
            overlaped = GameObject.Find(overlapedName).GetComponent<RectTransform>();
        }
    }
 
    private void Update()
    {
        if (!isInside)
        {
            if (overlaping.WorldSpaceOverlaps(overlaped))
            {
                isInside = true;
                onInsideAction?.Invoke();
                Debug.Log("Inside " + gameObject.name);
            }
        }
        else
        {
            if (!overlaping.WorldSpaceOverlaps(overlaped))
            {
                isInside = false;
                onOutiseAction?.Invoke();
                Debug.Log("Outside "+ gameObject.name);
            }
        }
 
    }

    #endregion
}
