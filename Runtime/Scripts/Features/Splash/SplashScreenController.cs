/****************************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: SharedVirtusenseProject
 * Creation Date: 7/22/2024 9:17:18 AM
 * 
 * Description: A custom implementation of a splash screen that allows for us to manually
 *                     run code during its exectuion. The purpose of this compared to the traditional
 *                     unity splash is that we can preload AKDK dlls during this process.
****************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SharedPackage.Features.SplashScreen
{
    public class SplashScreenController : MonoBehaviour
    {
        #region Fields
        [Header("Images")]
        [Tooltip("The image that is used to display the imagesToSplash")]
        [SerializeField] private Image splashImage;

        [FormerlySerializedAs("imagesToSplash")]
        [Tooltip("The sprites that will be displayed sequentially in the splash screen")]
        [SerializeField] private Sprite[] spritesToSplash = new Sprite[0];

        /// <summary>
        /// The color of the splash image (note that this is mainly used to modify the alpha value of the splashImage).
        /// </summary>
        private Color splashColor = Color.white;

        [Header("Display Settings")]
        [Range(0.0f, 10.0f)]
        [Tooltip("The total time that the image will be displayed (fade in and out does not affect this time)")]
        [SerializeField] private float totalImageTime = 2.0f;

        [Space(10)]

        [Range(0.0f, 2.0f)]
        [Tooltip("The minimum scale of the image when it is first displayed")]
        [SerializeField] private float minImageScale = 0.5f;

        [Range(0.0f, 2.0f)]
        [Tooltip("The maximum scale of the image when it is fully displayed")]
        [SerializeField] private float maxImageScale = 1.5f;

        [Space(10)]

        [Range(0.0f, 5.0f)]
        [Tooltip("The time it takes for the image to fade in")]
        [SerializeField] private float fadeInTime = 0.25f;

        [Range(0.0f, 5.0f)]
        [Tooltip("The time it takes for the image to fade out")]
        [SerializeField] private float fadeOutTime = 0.25f;
        #endregion

        #region Functions
        /// <summary>
        /// Handles the routine for a splash screen.
        /// </summary>
        /// <returns></returns>
        public IEnumerator SplashScreenRoutine()
        {
            if (splashImage == null)
            {
                Debug.LogError("Splash Image is not set in the inspector.");
                yield break;
            }

            for (int i = 0; i < spritesToSplash.Length; i++)
            {
                if (spritesToSplash[i] == null)
                {
                    Debug.LogError("Image to splash is null at index: " + i);
                    continue;
                }

                // Resetting image transparency
                splashColor.a = 0;
                splashImage.color = splashColor;

                // Resetting image sprite and scale
                splashImage.sprite = spritesToSplash[i];
                splashImage.transform.localScale = new Vector3(minImageScale, minImageScale, 1);
                splashImage.SetNativeSize();

                yield return ImageScaleRoutine();
            }

            Debug.Log("Setting Splash inactive");
            gameObject.SetActive(false);

            yield return new WaitForEndOfFrame(); // Ensures that the splash screen has a frame to become disabled

            #region Debug Loop
            // If you want to test it in a loop you can uncomment this code
            /*
            yield return new WaitForSeconds(1.0f);

            StartCoroutine(SplashScreenRoutine());*/
            #endregion
        }

        /// <summary>
        /// Scales in an image and fades it in and out.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ImageScaleRoutine()
        {
            var time = totalImageTime;

            while (time > 0)
            {
                yield return new WaitForEndOfFrame();

                time -= Time.deltaTime;

                // Scaling image in
                var timeLerp = time / totalImageTime; // 0 to 1
                var scale = Mathf.Lerp(maxImageScale, minImageScale, timeLerp);
                splashImage.transform.localScale = new Vector3(scale, scale, 1);

                #region Update Image Transparency
                if (time <= fadeOutTime) // Fade out
                {
                    var fadeLerp = Mathf.InverseLerp(fadeOutTime, 0.0f, time);
                    splashColor.a = Mathf.Lerp(1.0f, 0.0f, fadeLerp);
                }
                else if (time >= totalImageTime - fadeInTime) // Fade in
                {
                    var fadeLerp = Mathf.InverseLerp(totalImageTime, totalImageTime - fadeInTime, time);
                    splashColor.a = Mathf.Lerp(0.0f, 1.0f, fadeLerp);
                }
                else // Is visible by default
                {
                    splashColor.a = 1;
                }

                splashImage.color = splashColor;
                #endregion
            }
        }
        #endregion
    }
}
