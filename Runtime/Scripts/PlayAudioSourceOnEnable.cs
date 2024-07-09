/****************************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: SharedVirtusenseProject
 * Creation Date: 2/13/2024 12:18:11 PM
 * 
 * Description: Plays a sound everytime an object is enabled instead of on awake.
****************************************************************************/
using UnityEngine;

namespace SharedPackage.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayAudioSourceOnEnable : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// The audiosource attached to this object.
        /// </summary>
        private AudioSource audioSource;
        #endregion

        #region Functions
        /// <summary>
        /// Calls for the initialization of components and references.
        /// </summary>
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays a sound every time this object is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
        #endregion
    }
}