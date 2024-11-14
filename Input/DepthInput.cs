using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace floorgame
{
    public class DepthInput
    {
        private KinectSensor kinectSensor;
        private WriteableBitmap depthBitmap;
        private byte[] depthPixels;

        public ImageSource DepthImageSource => depthBitmap;

        public DepthInput(KinectSensor sensor)
        {
            kinectSensor = sensor;
            InitializeDepthStream();
        }

        private void InitializeDepthStream()
        {
            if (kinectSensor != null)
            {
                // Enable the depth stream
                kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                // Allocate space for the depth data (each pixel is 16 bits in the depth stream)
                depthPixels = new byte[kinectSensor.DepthStream.FramePixelDataLength * sizeof(int)];

                // Create the bitmap for the depth stream
                depthBitmap = new WriteableBitmap(
                    kinectSensor.DepthStream.FrameWidth,
                    kinectSensor.DepthStream.FrameHeight,
                    96.0, 96.0, PixelFormats.Bgr32, null);

                // Add an event handler for the depth frame
                kinectSensor.DepthFrameReady += KinectSensor_DepthFrameReady;
            }
            else
            {
                MessageBox.Show("Kinect sensor is not initialized.");
            }
        }

        private void KinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    // Convert depth data to grayscale pixel data
                    ConvertDepthFrameToBitmap(depthFrame);
                    depthBitmap.WritePixels(
                        new Int32Rect(0, 0, depthBitmap.PixelWidth, depthBitmap.PixelHeight),
                        depthPixels,
                        depthBitmap.PixelWidth * sizeof(int),
                        0);
                }
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
                depthPixels[i * 4] = intensity;       // Blue
                depthPixels[i * 4 + 1] = intensity;   // Green
                depthPixels[i * 4 + 2] = intensity;   // Red
                depthPixels[i * 4 + 3] = 255;         // Alpha
            }
        }

        public void Stop()
        {
            // Unsubscribe from the event handler to avoid memory leaks
            if (kinectSensor != null)
            {
                kinectSensor.DepthFrameReady -= KinectSensor_DepthFrameReady;
            }
        }
    }
}
