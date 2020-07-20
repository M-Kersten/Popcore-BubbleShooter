using UnityEngine;
using UnityEngine.UI;

namespace Bubbleshooter.UI
{
    /// <summary>
    /// Cycles through a list of colors and applies it to the image component of the gameobject
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ColorChangeBackground : MonoBehaviour
    {
        /// <summary>
        /// Duration on eacht transition between colors
        /// </summary>
        [SerializeField]
        private int fadeDuration;

        [SerializeField]
        private Color[] backgroundColors;

        private Image img;
        private int index;

        private void Awake() => img = GetComponent<Image>();

        private void Start()
        {
            index = 0;
            img.color = backgroundColors[0];
            if (fadeDuration < .001f)
                return;

            LerpColors();
        }

        /// <summary>
        /// Continously lerp the colors of the background in a loop through all the backgroundcolors
        /// </summary>
        private void LerpColors()
        {
            LeanTween.value(img.gameObject, backgroundColors[index], backgroundColors[index + 1 >= backgroundColors.Length ? 0 : index + 1], fadeDuration).setOnUpdate((value) => img.color = value).setOnComplete(() => LerpColors());
            index++;
            if (index >= backgroundColors.Length)
                index = 0;
        }

    }
}