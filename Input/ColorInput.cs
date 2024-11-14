using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace floorgame
{
    public class ColorInput
    {
        private KinectSensor kinectSensor;
        private WriteableBitmap colorBitmap;
        private byte[] colorPixels;

        public ImageSource ColorImageSource => colorBitmap;

        public ColorInput(KinectSensor sensor)
        {
            kinectSensor = sensor;
            InitializeColorStream();
        }

        private void InitializeColorStream()
        {
            if (kinectSensor != null)
            {
                // Enable the color stream
                kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space for the color data
                colorPixels = new byte[kinectSensor.ColorStream.FramePixelDataLength];

                // Create the bitmap for the color stream
                colorBitmap = new WriteableBitmap(
                    kinectSensor.ColorStream.FrameWidth,
                    kinectSensor.ColorStream.FrameHeight,
                    96.0, 96.0, PixelFormats.Bgr32, null);

                // Add an event handler for the color frame
                kinectSensor.ColorFrameReady += KinectSensor_ColorFrameReady;
            }
            else
            {
                MessageBox.Show("Kinect sensor is not initialized.");
            }
        }

        private void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(colorPixels);

                    colorBitmap.WritePixels(
                        new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight),
                        colorPixels,
                        colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        public void Stop()
        {
            if (kinectSensor != null)
            {
                kinectSensor.ColorFrameReady -= KinectSensor_ColorFrameReady;
            }
        }
    }
}
