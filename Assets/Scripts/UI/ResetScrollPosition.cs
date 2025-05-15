using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResetScrollPosition : MonoBehaviour
{
    public ScrollRect scrollRect;

IEnumerator Start()
{
    yield return null; // attendre un frame
    scrollRect.verticalNormalizedPosition = 1f;
}

}
