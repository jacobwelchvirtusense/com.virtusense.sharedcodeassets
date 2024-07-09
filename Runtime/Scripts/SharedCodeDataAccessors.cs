/****************************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: SharedVirtusenseProject
 * Creation Date: 7/8/2024 10:53:34 AM
 * 
 * Description: Acts as a data accessor for shared code. This helps with issues later
 *                     if we want to remove or modify a script but we want to make sure other
 *                     code bases have data to point to.
****************************************************************************/
using SharedPackage.Features.Timeout;

namespace SharedPackage
{
    public static class SharedCodeDataAccessors
    {
        #region Fields
        /// <summary>
        /// Returns true if the timeout prompt is open.
        /// </summary>
        public static bool TimeoutPromptOpen
        {
            get
            {
                return TimeoutBehaviour.TimeoutPromptOpen;
            }
        }
        #endregion
    }
}
