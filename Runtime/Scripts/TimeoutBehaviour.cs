/****************************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: SharedVirtusenseProject
 * Creation Date: 7/8/2024 10:53:34 AM
 * 
 * Description: Handles the base timeout functionality. Extra hooks are needed for closing
 *                     the application by hooking into the OnTimeout event. Extra functionality
 *                     for resetting the timeout timer is may also be needed. The ResetTime
 *                     function is public so that it can be called from other scripts for this purpose.
****************************************************************************/
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SharedPackage
{
    public class TimeoutBehaviour : MonoBehaviour
    {
        #region Fields
        #region Timeout Durations
        /// <summary>
        /// The total time before the user is timed out in minutes.
        /// </summary>
        private const float TIME_BEFORE_TIMEOUT = 0.25f;

        /// <summary>
        /// The total time before the user is timed out in seconds.
        /// </summary>
        private float TimeBeforeTimeoutSeconds
        {
            get
            {
                return TIME_BEFORE_TIMEOUT * 60.0f;
            }
        }

        /// <summary>
        /// The total time the prompt will be displayed in minutes. This is subtracted from the TimeBeforeTimeout to determine when the prompt will be displayed.
        /// </summary>
        private const float TIMEOUT_PROMPT_DURATION = 0.2f;

        /// <summary>
        /// The total time before the prompt is displayed in seconds. This is subtracted from the TimeBeforeTimeout to determine when the prompt will be displayed.
        /// </summary>
        private float TimeoutPromptDurationSeconds
        {
            get
            {
                return TIMEOUT_PROMPT_DURATION * 60.0f;
            }
        }

        /// <summary>
        /// The time that the prompt should be open at.
        /// </summary>
        private float PromptStartingTimeSeconds
        {
            get
            {
                return TimeBeforeTimeoutSeconds - TimeoutPromptDurationSeconds;
            }
        }
        #endregion

        /// <summary>
        /// Holds the number of seconds since the user has made any inputs or any conditions that would reset the prompt timer.
        /// </summary>
        private float timeSinceLastUserActivity = 0.0f;

        /// <summary>
        /// Holds true if the timeout prompt is currently set to being open.
        /// </summary>
        public static bool TimeoutPromptOpen = false;

        /// <summary>
        /// Holds true if we have processed the first frame yet.
        /// </summary>
        private bool hasPassedFirstFrame = false;

        /// <summary>
        /// An event that is invoked when the user has been inactive for too long.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnTimeout = new UnityEvent();

        /// <summary>
        /// The prompt that will be displayed and updated.
        /// </summary>
        private GameObject timeoutPrompt;

        /// <summary>
        /// The image that will control the fill amount of the timeout prompt.
        /// </summary>
        private Image promptTimerBar;

        /// <summary>
        /// The text that tells the user how much time is left before the timeout.
        /// </summary>
        private TextMeshProUGUI promptTimerText;
        #endregion

        #region Functions
        private void Awake()
        {
            timeoutPrompt = transform.GetChild(0).gameObject;
            var timeBarTextParent = timeoutPrompt.transform.GetChild(0).GetChild(0);
            promptTimerText = timeBarTextParent.GetChild(0).GetComponent<TextMeshProUGUI>();
            promptTimerBar = timeBarTextParent.GetChild(1).GetComponent<Image>();
        }

        /// <summary>
        /// Checks if the user has made any inputs.
        /// </summary>
        private void Update()
        {
            if (UserPressedKey() || UserMovedMouse()) // If other conditions are needed hook them in on a per project basis (unless sharing mode code for that is possible)
            {
                ResetTime();
            }
            else
            {
                UpdateTimeoutProgress();
            }
        }

        #region Reset Conditions
        /// <summary>
        /// Returns true if the user has pressed any key.
        /// </summary>
        /// <returns></returns>
        private bool UserPressedKey()
        {
            return Input.anyKey;
        }

        /// <summary>
        /// Returns true if the user has moved their mouse or trackpad.
        /// </summary>
        /// <returns></returns>
        private bool UserMovedMouse()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            return mouseX != 0 || mouseY != 0;
        }
        #endregion

        #region Timeout Functionality
        /// <summary>
        /// Resets the timeout timer and hides the prompt.
        /// </summary>
        public void ResetTime()
        {
            timeSinceLastUserActivity = 0;
            SetPromptActive(false);
        }

        /// <summary>
        /// Updates the timeout progress and displays the prompt if the user has been inactive for too long.
        /// </summary>
        private void UpdateTimeoutProgress()
        {
            if (hasPassedFirstFrame) // We do this check because unscaledDelta time throws a bad value on the first frame
            {
                timeSinceLastUserActivity += Time.unscaledDeltaTime; // Using unscaleedDeltaTime because normal deltaTime is affected by time scale and will show as 0 when we have the application paused
            }
            else
            {
                hasPassedFirstFrame = true;
            }

            if (timeSinceLastUserActivity >= TimeBeforeTimeoutSeconds)
            {
                OnTimeout.Invoke();
            }
            else if (timeSinceLastUserActivity >= PromptStartingTimeSeconds)
            {
                SetPromptActive(true);
                UpdatePromptTimer();
            }
            else
            {
                SetPromptActive(false);
            }
        }

        /// <summary>
        /// Changes the active state of the timeout prompt. (doing this in a function in case we want to add animations)
        /// </summary>
        /// <param name="shouldBeActive">Holds true if the prompt should be active.</param>
        private async void SetPromptActive(bool shouldBeActive)
        {
            await Task.Delay(1); // We wait a frame because I want to delay closing it after user inputs (this fixes an issue where remote inputs were getting used when pressing a key on the remote while the prompt was active)

            if (timeoutPrompt != null)
            {
                timeoutPrompt.SetActive(shouldBeActive);
                TimeoutPromptOpen = shouldBeActive;
            }
            else
            {
                TimeoutPromptOpen = false;
            }
        }

        /// <summary>
        /// Updates the timer that is displayed on the prompt.
        /// </summary>
        private void UpdatePromptTimer()
        {
            promptTimerBar.fillAmount = Mathf.InverseLerp(TimeBeforeTimeoutSeconds, PromptStartingTimeSeconds, timeSinceLastUserActivity);
            promptTimerText.text = Mathf.CeilToInt(TimeBeforeTimeoutSeconds - timeSinceLastUserActivity).ToString();
        }
        #endregion
        #endregion
    }

}