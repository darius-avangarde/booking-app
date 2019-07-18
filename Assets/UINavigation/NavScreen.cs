using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UINavigation
{
    public class NavScreen : MonoBehaviour
    {
        private RectTransform rectTransform;
        /// <summary>
        /// The GameObject's RectTransform.
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }

        private List<CanvasGroup> canvasGroups;
        /// <summary>
        /// The GameObject's CanvasGroup. Adds a new one if missing.
        /// </summary>
        public List<CanvasGroup> CanvasGroups
        {
            get
            {
                if(canvasGroups == null)
                {
                    canvasGroups = new List<CanvasGroup>();
                    if(GetComponent<CanvasGroup>() != null)
                        canvasGroups.Add(GetComponent<CanvasGroup>());
                    else
                        canvasGroups.Add(gameObject.AddComponent<CanvasGroup>());

                    foreach (Canvas c in gameObject.GetComponentsInChildren<Canvas>())
                    {
                        canvasGroups.Add(c.gameObject.AddComponent<CanvasGroup>());
                    }
                }

                return canvasGroups;
            }
        }

        /// <summary>
        /// Event invoked when the screen is coming into view and the transition is starting.
        /// </summary>
        public UnityEvent Showing;

        /// <summary>
        /// Event invoked when the screen has come into view and the transition has finished.
        /// </summary>
        public UnityEvent Shown;

        /// <summary>
        /// Event invoked when the screen is leaving view and the transition is starting.
        /// </summary>
        public UnityEvent Hiding;

        /// <summary>
        /// Event invoked when the screen has left view and the transition has finished.
        /// </summary>
        public UnityEvent Hidden;

        /// <summary>
        /// Called at the end of a transition when the screen has come into view.
        /// </summary>
        public void OnShown()
        {
            Shown.Invoke();
        }

        /// <summary>
        /// Called at the end of a transition when the screen has left view.
        /// </summary>
        public void OnHidden()
        {
            Hidden.Invoke();
        }

        /// <summary>
        /// Called at the start of a transition when the screen is coming into view.
        /// </summary>
        public void OnShowing()
        {
            Showing.Invoke();
        }

        /// <summary>
        /// Called at the start of a transition when the screen is leaving view.
        /// </summary>
        public void OnHiding()
        {
            Hiding.Invoke();
        }
    }
}
