using TMPro;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class VersionDisplayer : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Format")]
        [SerializeField] private string prefix = "Version";
        [SerializeField] private bool showBuildNumber = true;

        private void Awake()
        {
            UpdateVersionText();
        }

        private void UpdateVersionText()
        {
            if (versionText == null)
            {
                Debug.LogWarning("VersionDisplayer: TextMeshProUGUI is not assigned.");
                return;
            }

            string version = Application.version;
            string build = Application.buildGUID;

            if (showBuildNumber)
            {
                // Shorten build GUID for readability
                build = build.Substring(0, 8);
                versionText.text = $"{prefix} {version} ({build})";
            }
            else
            {
                versionText.text = $"{prefix} {version}";
            }
        }
    }
}