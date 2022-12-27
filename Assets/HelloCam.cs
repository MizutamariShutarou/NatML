using UnityEngine;
using UnityEngine.UI;
using NatML.Devices;
using NatML.Devices.Outputs;

public class HelloCam : MonoBehaviour
{

    [Header(@"UI")]
    public RawImage rawImage;
    public AspectRatioFitter aspectFitter;

    async void Start()
    {
        // Create a device query for the front camera 
        var filter = MediaDeviceCriteria.FrontCamera;
        var query = new MediaDeviceQuery(filter);
        // Get the camera device
        var device = query.current as CameraDevice;
        // Start the camera preview
        var textureOutput = new TextureOutput(); // stick around for an explainer
        device.StartRunning(textureOutput);
        // Display the preview in our UI
        var previewTexture = await textureOutput;
        rawImage.texture = previewTexture;
        aspectFitter.aspectRatio = (float)previewTexture.width / previewTexture.height;
    }
}