using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage = null;

    public void SetFill(float amount)
    {
        fillImage.fillAmount = amount;
    }
}