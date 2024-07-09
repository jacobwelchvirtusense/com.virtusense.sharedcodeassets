/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/28/2023 1:41:30 PM
 * 
 * Description: Sets this objects audiosource to ignore the 
 *              audiolisteners pause state.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedPackage.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class IgnoreAudioListenerPause : MonoBehaviour
    {
        #region Functions
        /// <summary>
        /// Sets this objects audiosource to ignore the audiolisteners pause state.
        /// </summary>
        private void Awake()
        {
            var audioSource = GetComponent<AudioSource>();

            if (audioSource)
            {
                audioSource.ignoreListenerPause = true;
            }
        }
        #endregion
    }
}
