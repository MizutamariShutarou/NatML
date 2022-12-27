using UnityEngine;
using NatML.Devices;
using NatML.Devices.Outputs;
using NatML.Vision;
using NatML.Visualizers;

namespace NatML
{
    [MLModelDataEmbed("@natml/blazepalm-detector"), MLModelDataEmbed("@natml/blazepalm-landmark")]
    public class HandTracking : MonoBehaviour
    {
        /// <summary>
        /// Visualizer
        /// </summary>
        [Header(@"UI"), SerializeField]
        private BlazePalmVisualizer visualizer;

        /// <summary>
        /// Pipeline
        /// </summary>
        private BlazePalmPipeline pipeline;

        /// <summary>
        /// CameraDevice
        /// </summary>
        private CameraDevice cameraDevice;

        /// <summary>
        /// CameraDevice‚ÅÊ‚µ‚½‚à‚Ì‚ğTexture‚É•ÏŠ·‚µ‚½‚à‚Ì
        /// </summary>
        private TextureOutput previewTextureOutput;

        private void OnDisable() => pipeline?.Dispose();

        private async void Start()
        {
            // ƒJƒƒ‰‚ª‹–‰Â‚³‚ê‚Ä‚¢‚é‚©
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();

            // ‹–‰Â‚³‚ê‚Ä‚¢‚È‚¢ê‡‚Íreturn
            if (permissionStatus != PermissionStatus.Authorized) return;

            // ƒJƒƒ‰æ“¾
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;

            // ƒJƒƒ‰‚ÅÊ‚µ‚½‚à‚Ì‚ğTexture‚É•ÏŠ·
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            var previewTexture = await previewTextureOutput;
            visualizer.image = previewTexture;

            // BlazePalm detector‚Æpredictor‚ğNatML Hub‚©‚çæ“¾‚µPipeline‚Ìì¬
            var detectorModelData = await MLModelData.FromHub("@natml/blazepalm-detector");
            var predictorModelData = await MLModelData.FromHub("@natml/blazepalm-landmark");
            pipeline = new BlazePalmPipeline(detectorModelData, predictorModelData);
        }

        private void Update()
        {
            // Pipeline‚ªì¬‚³‚ê‚Ä‚¢‚é‚©
            if (pipeline == null) return;

            // ƒJƒƒ‰‚ÅÊ‚µ‚Ä‚¢‚éî•ñ‚©‚çè‚ğ—\‘ª
            var hands = pipeline.Predict(previewTextureOutput.texture);

            // —\‘ª‚µ‚½î•ñ‚ğ•`‰æ
            visualizer.Render(hands);
        }
    }
}