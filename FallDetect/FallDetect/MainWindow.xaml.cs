using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using Emgu.Util;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Emgu.CV.BgSegm;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf.Controls;
using System.IO;
using Emgu.CV.Util;

namespace FallDetect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        public double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        public double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        bool isFall=false;
        private List<VectorOfPoint> contoursArray;
        CircleF circle;
        RotatedRect r;
        int j = 0;
        Image<Bgr, byte> mask = null;
        Image<Bgr, Byte> referenceImage = null;
        //BackgroundSubtractorMOG mog = new BackgroundSubtractorMOG();
        BackgroundSubtractorMOG2 mog2 = new BackgroundSubtractorMOG2(10000,3000,false);
        int i = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        private void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldSensor = (KinectSensor)e.OldValue;
            //stop the old sensor
            if (oldSensor != null)
            {
                oldSensor.Stop();
                oldSensor.AudioSource.Stop();
            }
            
            //get the new sensor
            var newSensor = (KinectSensor)e.NewValue;
            if (newSensor == null)
            {
                return;
            }
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            newSensor.SkeletonStream.Enable(parameters);
            newSensor.ColorStream.Enable();
            newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);
            newSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            try
            {
                newSensor.Start();

            }
            catch (System.IO.IOException)
            {
                //this happens if another app is using the Kinect
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        private void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

            /*
            Skeleton first = GetFirstSkeleton(e);
            if (first == null)
            {
                return;
            }
            using (SkeletonFrame skeletonframe = e.OpenSkeletonFrame())
            {
               
       
                    Joint leftHandJoint = Coding4Fun.Kinect.Wpf.SkeletalExtensions.ScaleTo(first.Joints[JointType.HandLeft], Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    Joint spineJoint = Coding4Fun.Kinect.Wpf.SkeletalExtensions.ScaleTo(first.Joints[JointType.Spine], Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    Joint leftfootJoint = Coding4Fun.Kinect.Wpf.SkeletalExtensions.ScaleTo(first.Joints[JointType.FootLeft], Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    Joint rightfootJoint = Coding4Fun.Kinect.Wpf.SkeletalExtensions.ScaleTo(first.Joints[JointType.FootRight], Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    Joint HeadJoint = Coding4Fun.Kinect.Wpf.SkeletalExtensions.ScaleTo(first.Joints[JointType.Head], Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    isFall = detectFall(spineJoint, leftfootJoint, rightfootJoint, HeadJoint,e);
                    points.Content = isFall == true ? "Fall" : "Standing";
                
            }
            */

            /*
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                Image<Bgr, byte> image = new Image<Bgr, byte>(colorFrame.GenerateColoredBytes().ToBitmap());
                makeMaskOnce(image.Width, image.Height);
                mog2.Apply(image, mask);
                CvInvoke.Imshow("Mask", mask);
                CvInvoke.Imshow("ABC", image);


            }
        }
         */   
           

            
 
            using(DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
             
                if (depthFrame == null)
                {
                    return;
                }

                Image<Bgr, byte> image = new Image<Bgr, byte>(depthFrame.GenerateColoredBytes().ToBitmap());
                Image<Bgr, byte> image2 = image.Copy();
                Image<Bgr, byte> image3 = image.Copy();
                makeMaskOnce(image.Width, image.Height);
                mog2.Apply(image2, mask);
                CvInvoke.Imshow("Mask", mask);

                VectorOfVectorOfPoint contoursDetected = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(mask, contoursDetected, null, Emgu.CV.CvEnum.RetrType.Tree, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                contoursArray = new List<VectorOfPoint>();
                int count = contoursDetected.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint currContour = contoursDetected[i])
                    {
                        contoursArray.Add(currContour);
                    }
                }

                VectorOfPoint maxCont = contoursArray[0];
                for (int i = 0; i < contoursArray.Count; i++)
                {
                    if (CvInvoke.ContourArea(contoursArray[i]) > 400)
                    {
                        if (CvInvoke.ContourArea(maxCont) < CvInvoke.ContourArea(contoursArray[i]))
                        {
                            maxCont = contoursArray[i];
                            Console.WriteLine(CvInvoke.ContourArea(maxCont));
                        }
                    }
                }
                System.Drawing.Rectangle rect = CvInvoke.BoundingRectangle(maxCont);
                double areaRect = rect.Height * rect.Width;
                double contourArea = CvInvoke.ContourArea(maxCont);
                if (areaRect> 800 && areaRect<10000)
                {
                    
                    CvInvoke.Rectangle(image3, rect, new MCvScalar(255, 255, 0),2);
                    float ratio = rect.Height / rect.Width;
                    double theta = Math.Atan(ratio);
                    //Console.WriteLine("Theta="+theta);

                    if (theta < 0.6100)
                    {
                        points.Content = "Fall "+theta+" "+CvInvoke.ContourArea(maxCont);
                    }
                    else
                    {
                        points.Content = "Not a fall";
                    }
                }
                CvInvoke.Imshow("Image", image3);
                
            }
            
        }

        public void saveimageOnce(Image<Bgr,byte> image)
        {
            if (j == 0)
            {
                referenceImage = image.Copy();
                j++;
            }
            else
            {
                return;
            }
        }

        public void makeMaskOnce(int width,int height)
        {
            if (i==0)
            {
                mask = new Image<Bgr, byte>(width, height);
                i++;
            }
            else
            {
                return;
            }

        }

        public bool detectFall(Joint spineJoint,Joint leftfootJoint,Joint rightfootJoint,Joint HeadJoint, AllFramesReadyEventArgs e)
        {
            int counter = 0;
            int rangeN = -350;
            int rangeP = 350;
            int diff1 = Convert.ToInt32(spineJoint.Position.Y - leftfootJoint.Position.Y);
            int diff2 = Convert.ToInt32(spineJoint.Position.Y - rightfootJoint.Position.Y);
            int diff3 = Convert.ToInt32(spineJoint.Position.Y - HeadJoint.Position.Y);
            int diff4 = Convert.ToInt32(leftfootJoint.Position.Y - rightfootJoint.Position.Y);
            int diff5 = Convert.ToInt32(leftfootJoint.Position.Y - HeadJoint.Position.Y);
            int diff6 = Convert.ToInt32(rightfootJoint.Position.Y - HeadJoint.Position.Y);

            this.diff1.Content = diff1;
            this.diff2.Content = diff2;
            this.diff3.Content = diff3;
            this.diff4.Content = diff4;
            this.diff5.Content = diff5;
            

            if ((diff1>rangeN&&diff1<rangeP)&& (diff2 > rangeN && diff2 < rangeP)&& (diff3 > rangeN && diff3 < rangeP)&&(diff4 > rangeN && diff4 < rangeP)&&(diff5 > rangeN && diff5 < rangeP)&&(diff6 > rangeN && diff6 < rangeP))
            {
                counter++;   
            }
            if (counter > 0){
                ColorImageFrame colorImage = e.OpenColorImageFrame();
                if (colorImage != null)
                {
                    BitmapSource boundaryImg2 = ImageHelper.GenerateColoredBytes(colorImage);
                    FileStream fs = new FileStream(@"E:\NYU\1\Computer Vision\Projects\thresh\200\Image_" + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond + ".bmp", FileMode.Create);

                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(boundaryImg2));
                    encoder.Save(fs);
                }
                    return true;

            }
            else
            {
                return false;
            }
           
        }
        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }
                
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);
                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;
            }
        }
    }
}
