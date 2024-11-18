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
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SharedPackage.Features.Timeout
{
    public class TimeoutBehaviour : MonoBehaviour
    {
        #region Fields
        #region Timeout Durations
        /// <summary>
        /// The total time before the user is timed out in minutes.
        /// </summary>
        private float timeBeforeTimeout = 10.0f;

        /// <summary>
        /// The total time before the user is timed out in seconds.
        /// </summary>
        private float TimeBeforeTimeoutSeconds
        {
            get
            {
                return timeBeforeTimeout * 60.0f;
            }
        }

        /// <summary>
        /// The total time the prompt will be displayed in minutes. This is subtracted from the TimeBeforeTimeout to determine when the prompt will be displayed. It should be smaller than the TimeBeforeTimeout.
        /// </summary>
        private const float TIMEOUT_PROMPT_DURATION = 1.0f;

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

        private float timeOfLastUserActivity = 0.0f;

        /// <summary>
        /// Holds the number of seconds since the user has made any inputs or any conditions that would reset the prompt timer.
        /// </summary>
        private float TimeSinceLastUserActivity
        {
            get
            {
                return Time.unscaledTime - timeOfLastUserActivity;
            }
        }

        /// <summary>
        /// Holds true if the timeout prompt is currently set to being open.
        /// </summary>
        public static bool TimeoutPromptOpen = false;

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
        // private Image promptTimerBar;

        /// <summary>
        /// The text that tells the user how much time is left before the timeout.
        /// </summary>
        private TextMeshProUGUI promptTimerText;
        #endregion

        #region Functions
        /// <summary>
        /// We get these so that we don't have non-assigned warnings from the package.
        /// </summary>
        private void Awake()
        {
            timeoutPrompt = transform.GetChild(0).gameObject;

            var timeBarTextParent = timeoutPrompt.transform.GetChild(0).GetChild(0);
            promptTimerText = timeBarTextParent.GetChild(1).GetComponent<TextMeshProUGUI>();
            // promptTimerBar = timeBarTextParent.GetChild(1).GetComponent<Image>(); // We decided not to use the timer bar but leaving functionality commented out
        }

        /// <summary>
        /// Records what the starting time is when this object is initialized.
        /// </summary>
        private void Start()
        {
            timeOfLastUserActivity = Time.unscaledTime;
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

        /// <summary>
        /// Sets prompt to be inactive as there was a bug where the scene instance was not getting reset when stopping play mode.
        /// </summary>
        private void OnApplicationQuit()
        {
            SetPromptActive(false);
        }

        /// <summary>
        /// This will set a new timeout time (currently the user will be timed out after 10 minutes of inactivity).
        /// </summary>
        /// <param name="newTimeBeforeTimeout">The new time, don't make this less than 1 minute.</param>
        public void OverrideTimeBeforeTimeout(float newTimeBeforeTimeout)
        {
            timeBeforeTimeout = newTimeBeforeTimeout;
        }

        /// <summary>
        /// This will set a new panel to prompt the user when they are about to be timed out. It should have the same structure as the default timeout prompt.
        /// </summary>
        /// <param name="newTimeoutPromptMenu">The new object that we want to use as the timeout prompt menu.</param>
        public void OverrideTimeoutPromptObject(GameObject newTimeoutPromptMenu)
        {
            timeoutPrompt = newTimeoutPromptMenu;

            try
            {
                var timeBarTextParent = timeoutPrompt.transform.GetChild(0).GetChild(0);
                promptTimerText = timeBarTextParent.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            catch(Exception e)
            {
                Debug.LogError("The new timeout prompt object does not have the correct structure. Please make sure it has the same structure as the default timeout prompt.");
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
            timeOfLastUserActivity = Time.unscaledTime;
            SetPromptActive(false);
        }

        /// <summary>
        /// Updates the timeout progress and displays the prompt if the user has been inactive for too long.
        /// </summary>
        private void UpdateTimeoutProgress()
        {
            /*
            if (hasPassedFirstFrame) // We do this check because unscaledDelta time throws a bad value on the first frame
            {
                TimeSinceLastUserActivity += Time.unscaledDeltaTime; // Using unscaleedDeltaTime because normal deltaTime is affected by time scale and will show as 0 when we have the application paused
            }
            else
            {
                hasPassedFirstFrame = true;
            }*/

            if (TimeSinceLastUserActivity >= TimeBeforeTimeoutSeconds)
            {
                OnTimeout.Invoke();
            }
            else if (TimeSinceLastUserActivity >= PromptStartingTimeSeconds)
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
            //promptTimerBar.fillAmount = Mathf.InverseLerp(TimeBeforeTimeoutSeconds, PromptStartingTimeSeconds, timeSinceLastUserActivity);
            var timeLeft = Mathf.CeilToInt(TimeBeforeTimeoutSeconds - TimeSinceLastUserActivity).ToString();
            promptTimerText.text = $"Your session will end in <b>{ timeLeft }</b> seconds.";
        }
        #endregion
        #endregion
    }
}