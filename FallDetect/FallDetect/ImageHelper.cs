using Emgu.CV;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FallDetect
{
    public static class ImageHelper
    {


        const int skeletonCount = 6;

        const float MaxDepthDistance = 4000; // max value returned
        const float MinDepthDistance = 850; // min value returned
        const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;
       


        public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                // from System.Media.BitmapImage to System.Drawing.Bitmap
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
                return bitmap;
            }
        }

        public static BitmapSource GenerateColoredBytes(this DepthImageFrame depthFrame, DepthImagePoint pointL)
        {

            //get the raw data from kinect with the depth for every pixel
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            //use depthFrame to create the image to display on-screen
            //depthFrame contains color information for all pixels in image
            //Height x Width x 4 (Red, Green, Blue, empty byte)
            Byte[] pixels = new byte[depthFrame.Height * depthFrame.Width * 4];

            //Bgr32  - Blue, Green, Red, empty byte
            //Bgra32 - Blue, Green, Red, transparency 
            //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

            //hardcoded locations to Blue, Green, Red (BGR) index positions       
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;

            for (int depthIndex = 0, colorIndex = 0;
                            depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                            depthIndex++, colorIndex += 4)
            {

                // Calculate the distance represented by the two depth bytes
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // Map the distance to an intesity that can be represented in RGB
                var intensity = CalculateIntensityFromDepth(depth);


                if (depth > 850 && depth < pointL.Depth + 40)
                {
                    // Apply the intensity to the color channels
                    pixels[colorIndex + BlueIndex] = intensity;//blue
                    pixels[colorIndex + GreenIndex] = intensity; //green
                    pixels[colorIndex + RedIndex] = intensity; //red                    
                }
            }
            return BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, depthFrame.Width * 4);
        }



        public static BitmapSource GenerateColoredBytes(this DepthImageFrame depthFrame)
        {

            //get the raw data from kinect with the depth for every pixel
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            //use depthFrame to create the image to display on-screen
            //depthFrame contains color information for all pixels in image
            //Height x Width x 4 (Red, Green, Blue, empty byte)
            Byte[] pixels = new byte[depthFrame.Height * depthFrame.Width * 4];

            //Bgr32  - Blue, Green, Red, empty byte
            //Bgra32 - Blue, Green, Red, transparency 
            //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

            //hardcoded locations to Blue, Green, Red (BGR) index positions       
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;

            for (int depthIndex = 0, colorIndex = 0;
                            depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                            depthIndex++, colorIndex += 4)
            {

                // Calculate the distance represented by the two depth bytes
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // Map the distance to an intesity that can be represented in RGB
                var intensity = CalculateIntensityFromDepth(depth);
                pixels[colorIndex + BlueIndex] = intensity;//blue
                pixels[colorIndex + GreenIndex] = intensity; //green
                pixels[colorIndex + RedIndex] = intensity; //red     
            }
            return BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, depthFrame.Width * 4);
        }

        public static BitmapSource GenerateColoredBytes(this ColorImageFrame colorframe)
        {

            byte[] colorPixels=new byte[colorframe.PixelDataLength];
            colorframe.CopyPixelDataTo(colorPixels);
           
            return BitmapSource.Create(colorframe.Width, colorframe.Height, 96, 96, PixelFormats.Bgr32, null, colorPixels, colorframe.Width * 4);
        }




        public static byte CalculateIntensityFromDepth(int distance)
        {
            //formula for calculating monochrome intensity for histogram
            return (byte)(255 - (255 * Math.Max(distance - MinDepthDistance, 0)
                / (MaxDepthDistanceOffset)));
        }


        public static Bitmap ToBitmap(System.Drawing.Point[] sequence)
        {
            if (sequence.Length == 0)
                return null;

            int xmax = (int)sequence.Max(x => x.X);
            int xmin = (int)sequence.Min(x => x.X);

            int ymax = (int)sequence.Max(x => x.Y);
            int ymin = (int)sequence.Min(x => x.Y);

            int width = xmax - xmin;
            int height = ymax - ymin;


            Bitmap bmp = new Bitmap(width + 16, height + 16);

            Graphics g = Graphics.FromImage(bmp);


            for (int i = 1; i < sequence.Length; i++)
            {
                int x = (int)sequence[i].X - xmin;
                int y = (int)sequence[i].Y - ymin;
                int p = (int)Accord.Math.Tools.Scale(0, sequence.Length, 0, 255, i);

                int prevX = (int)sequence[i - 1].X - xmin;
                int prevY = (int)sequence[i - 1].Y - ymin;

                using (System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(255 - p, 0, p)))
                using (System.Drawing.Pen pen = new System.Drawing.Pen(brush, 16))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, prevX, prevY, x, y);
                }
            }

            return bmp;
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(48, 48));

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        /*   public static BitmapSource ToBitmapSource(IImage image)
           {

               System.Drawing.Bitmap source = image.Bitmap;
               IntPtr ptr = source.GetHbitmap();
               BitmapSource bmp = null;
               try
               {
                  bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        ptr,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(48, 48));

               }
               finally
               {
                   DeleteObject(ptr);
               }
               return bmp;


           }

       */


        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }


    }



}

