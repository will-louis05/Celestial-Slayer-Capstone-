using UnityEngine;
using System.IO;

public class ScreenshotTaker : MonoBehaviour
{
    void Update()
    {
        // Press F12 to take a screenshot
        if (Input.GetKeyDown(KeyCode.F12))
        {
            string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

            // Saves to the project root in the Editor, or persistentDataPath in builds
            string path = filename;


            // Optional: Pass a second int parameter (e.g., 2 or 4) to upscale the resolution
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Saved screenshot to: " + path);
        }
    }
}
