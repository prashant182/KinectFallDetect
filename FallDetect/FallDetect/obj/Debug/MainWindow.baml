   M S B A M L   `   `   `  ����E  AFallDetect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:FallDetect
FallDetect  5   	   ^ ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers 5   	       FallDetect.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:FallDetect     5   	   mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   	   -   5          � Title$  
MainWindow��5	   	   $	��350��6   $	��525��6)   .��5
      � .+�5   
   =  7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  -   5        $��kinectSensorChooser1��5   =   $
��Left=�6!   $	��Top=�6Y   $��89��6q   $��76��6|   $��	207,0,0,0q�6�   ;  5Microsoft.Samples.Kinect.WpfViewers.KinectDepthViewer  5   
   -   5        $��kinectDepthViewer1��5   ;   $
��Left=�6   $	��Top=�6U   $	��206��6m   $	��222��6z   $��	0,103,0,0q�6�   5  /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect 6�   ��  �� ElementName$ kinectSensorChooser1�� �� Path$ Kinect�6�   >  8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  5   
   -   5        $��kinectSkeletonViewer1��5   >   $
��Left=�6"   $	��Top=�6[   $	��206��6s   $	��232��6�   $��285,103,0,0q�6�    6�   �� $ kinectSensorChooser1��$ Kinect�6�   �� 5   
   -   5         ��Name$ points��5      $��Label��6    $
��Left=�60   $	��Top=�6K   �� 5   
   -   5        $ diff1��5      $��Label��6   $
��Left=�6/   $	��Top=�6J   $��	338,0,0,0q�6b   �� 5   
   -   5        $ diff2��5      $��Label��6   $
��Left=�6/   $	��Top=�6J   $��
338,26,0,0q�6b   �� 5   
   -   5        $ diff3��5      $��Label��6   $
��Left=�6/   $��Bottom=�6J   $��338,0,0,247q�6e   �� 5   
   -	   5        $ diff4��5      $��Label��6   $
��Left=�6/   $	��Top=�6J   $��
338,72,0,0q�6b   �� 5   
   -
   5        $ diff5��5      $��Label��6   $
��Left=�6/   $	��Top=�6J   $��
427,72,0,0q�6b   5      5      