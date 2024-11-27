using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace FloorGame.Model.SensorInput;

public class ColorImage
{
    KinectSensor _kinectSensor;
    private WriteableBitmap colorBitmap;
    private byte[] colorPixels;

    public ImageSource Image => colorBitmap;

    public ColorImage(KinectSensor kinectSensor)
    {
        _kinectSensor = kinectSensor!;
        _kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

        colorPixels = new byte[_kinectSensor.ColorStream.FramePixelDataLength];
        colorBitmap = new WriteableBitmap(
            _kinectSensor.ColorStream.FrameWidth,
            _kinectSensor.ColorStream.FrameHeight,
            96.0, 96.0, PixelFormats.Bgr32, null
        );

        _kinectSensor.ColorFrameReady += KinectSensor_ColorFrameReady;
    }

    ~ColorImage() => _kinectSensor.ColorFrameReady -= KinectSensor_ColorFrameReady;

    private void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
    {
        using ColorImageFrame colorFrame = e.OpenColorImageFrame();
        if (colorFrame != null)
        {
            colorFrame.CopyPixelDataTo(colorPixels);

            colorBitmap.WritePixels(
                new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight),
                colorPixels,
                colorBitmap.PixelWidth * sizeof(int),
                0
            );
        }
    }
}
