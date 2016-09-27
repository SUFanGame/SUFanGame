using UnityEngine;
using UnityEngine.UI;
using StevenUniverse.FanGame.Extensions;

namespace StevenUniverse.FanGame.UI
{
    public class ProgressBarUIController : MonoBehaviour
    {
        private Image image;
        private Text title;
        private Text info;

        void Start()
        {
        }

        public float Progress
        {
            get
            {
                if (image == null)
                {
                    CacheImage();
                }
                return image.fillAmount;
            }
            set
            {
                if (image == null)
                {
                    CacheImage();
                }
                image.fillAmount = value;
            }
        }

        public string Title
        {
            get
            {
                if (title == null)
                {
                    CacheTitle();
                }
                return title.text;
            }
            set
            {
                if (title == null)
                {
                    CacheTitle();
                }
                title.text = value;
            }
        }

        public string Info
        {
            get
            {
                if (info == null)
                {
                    CacheInfo();
                }
                return info.text;
            }
            set
            {
                if (info == null)
                {
                    CacheInfo();
                }
                info.text = value;
            }
        }

        private void CacheImage()
        {
            image = gameObject.FindChildWithName("Image").GetComponent<Image>();
        }

        private void CacheTitle()
        {
            title = gameObject.FindChildWithName("Title").GetComponent<Text>();
        }

        private void CacheInfo()
        {
            info = gameObject.FindChildWithName("Info").GetComponent<Text>();
        }
    }
}