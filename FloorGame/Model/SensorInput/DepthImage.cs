using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FloorGame.Model.SensorInput;

public class DepthImage
{
    private KinectSensor _kinectSensor;
    private WriteableBitmap _depthBitmap;
    private byte[] _depthPixels;

    public ImageSource Image => _depthBitmap;

    public DepthImage(KinectSensor kinectSensor)
    {
        _kinectSensor = kinectSensor;
        _kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

        _depthPixels = new byte[_kinectSensor.DepthStream.FramePixelDataLength * sizeof(int)];
        _depthBitmap = new WriteableBitmap(
            _kinectSensor.DepthStream.FrameWidth,
            _kinectSensor.DepthStream.FrameHeight,
            96.0, 96.0, PixelFormats.Bgr32, null
        );

        _kinectSensor.DepthFrameReady += KinectSensor_DepthFrameReady;
    }
    ~DepthImage() => _kinectSensor.DepthFrameReady -= KinectSensor_DepthFrameReady;

    private void KinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
    {
        using DepthImageFrame depthFrame = e.OpenDepthImageFrame();
        if (depthFrame != null)
        {
            ConvertDepthFrameToBitmap(depthFrame);
            _depthBitmap.WritePixels(
                new Int32Rect(0, 0, _depthBitmap.PixelWidth, _depthBitmap.PixelHeight),
                _depthPixels,
                _depthBitmap.PixelWidth * sizeof(int),
                0
            );
        }
    }

    private void ConvertDepthFrameToBitmap(DepthImageFrame depthFrame)
    {
        short[] rawDepthData = new short[depthFrame.PixelDataLength];
        depthFrame.CopyPixelDataTo(rawDepthData);

        for (int i = 0; i < rawDepthData.Length; ++i)
        {
            // Get the depth for this pixel
            int depth = rawDepthData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

            // Convert depth to grayscale intensity (0-255)
            byte intensity = (byte)(depth >= depthFrame.MinDepth && depth <= depthFrame.MaxDepth ? depth / 8 : 0);

            // Each pixel is represented by four bytes in the byte array (ARGB format)
            _depthPixels[i * 4] = intensity;       // Blue
            _depthPixels[i * 4 + 1] = intensity;   // Green
            _depthPixels[i * 4 + 2] = intensity;   // Red
            _depthPixels[i * 4 + 3] = 255;         // Alpha
        }
    }
}
