using UnityEngine;
using UnityEngine.UI;

public class FTUESign : InteractableObjectBehaviour
{
    [Header("FTUE")]
    [SerializeField]
    private Texture m_tutoTexture;
    public Texture TutoTexture => m_tutoTexture;
}
