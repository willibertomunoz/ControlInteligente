using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;
using Accord.Imaging;
using Accord.Math.Geometry;
using Accord.Video;
using Accord.Video.DirectShow;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using ControlInteligente.Negocio;

namespace ControlInteligente
{
    public partial class Tracking_face : Form
    {
        #region - Declaracion de Variables -
        FilterInfoCollection videoDevices;

        // opened video source
        private IVideoSource videoSource = null;

        // object detector
        HaarObjectDetector detector;

        // object tracker
        Camshift tracker = null;

        // window marker
        RectanglesMarker marker;


        private bool detecting = false;
        private bool tracking = false;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount = new int[statLength];

        #endregion

        public Tracking_face()
        {
            InitializeComponent();

            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    devicesCombo.Items.Add(device.Name);
                }
                HaarCascade cascade = new FaceHaarCascade();
                detector = new HaarObjectDetector(cascade,
                    25, ObjectDetectorSearchMode.Single, 1.2f,
                    ObjectDetectorScalingMode.GreaterToSmaller);
            }
            catch (ApplicationException)
            {
                devicesCombo.Items.Add("No local capture devices");
                devicesCombo.Enabled = false;
            }

            devicesCombo.SelectedIndex = 1;

            // create video source
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[devicesCombo.SelectedIndex].MonikerString);

            // set frame size
            videoSource.VideoResolution = selectResolution(videoSource);

            // open it
            OpenVideoSource(videoSource);

            detecting = true;
        }


        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, NewFrameEventArgs args)
        {
            var direccion = "Centro";
            int direccionServo = 2;
            if (!detecting && !tracking)
                return;

            lock (this)
            {
                if (detecting)
                {
                    detecting = false;
                    tracking = false;

                    UnmanagedImage im = UnmanagedImage.FromManagedImage(args.Frame);

                    float xscale = im.Width / 160f;
                    float yscale = im.Height / 120f;

                    ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
                    UnmanagedImage downsample = resize.Apply(im);

                    Rectangle[] regions = detector.ProcessFrame(downsample);

                    if (regions.Length > 0)
                    {
                        tracker.Reset();

                        // Will track the first face found
                        Rectangle face = regions[0];

                        // Reduce the face size to avoid tracking background
                        Rectangle window = new Rectangle(
                            (int)((regions[0].X + regions[0].Width / 2f) * xscale),
                            (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
                            1, 1);
                        Console.Write("x:" + (int)((regions[0].X + regions[0].Width / 2f) * xscale));
                        Console.Write("y:" + (int)((regions[0].X + regions[0].Height / 2f) * xscale));
                        window.Inflate(
                            (int)(0.2f * regions[0].Width * xscale),
                            (int)(0.4f * regions[0].Height * yscale));

                        // Initialize tracker
                        tracker.SearchWindow = window;
                        tracker.ProcessFrame(im);

                        marker = new RectanglesMarker(window);
                        marker.ApplyInPlace(im);

                        args.Frame = im.ToManagedImage();

                        tracking = true;
                        //detecting = true;
                    }
                    else
                    {
                        detecting = true;
                    }
                }
                else if (tracking)
                {
                    UnmanagedImage im = UnmanagedImage.FromManagedImage(args.Frame);

                    // Track the object
                    tracker.ProcessFrame(im);

                    // Get the object position
                    var obj = tracker.TrackingObject;
                    var wnd = tracker.SearchWindow;

                    //if (displayBackprojectionToolStripMenuItem.Checked)
                    //{
                    //    var backprojection = tracker.GetBackprojection(PixelFormat.Format24bppRgb);
                    //    im = UnmanagedImage.FromManagedImage(backprojection);
                    //}

                    //if (drawObjectAxisToolStripMenuItem.Checked)
                    //{
                    //    LineSegment axis = obj.GetAxis();

                    //    // Draw X axis
                    //    if (axis != null)
                    //        Drawing.Line(im, axis.Start.Round(), axis.End.Round(), Color.Red);
                    //    else detecting = true;
                    //}
                    if (obj.Rectangle.Width < (args.Frame.Width / 3) * 2)
                        if (obj.Rectangle.X < args.Frame.Width / 3)
                        {
                            direccion = "Izquierda";
                            direccionServo = 1;
                        }
                        else if (obj.Rectangle.X > (args.Frame.Width / 3) * 2)
                        {
                            direccion = "Derecha";
                            direccionServo = 3;
                        }
                    try
                    {
                        this.Invoke((MethodInvoker)delegate
                    {
                        if (textBox1 != null)
                        {
                            textBox1.Text = obj.Rectangle.X.ToString();
                            textBox2.Text = obj.Rectangle.Y.ToString();
                            label1.Text = direccion;
                        }
                    });
                        ComunicacionPuertoSerie.Instance.enviarEvento(direccionServo.ToString());
                    }
                    catch (Exception e) { }
                    if (/*drawObjectBoxToolStripMenuItem.Checked && drawTrackingWindowToolStripMenuItem.Checked*/false)
                    {
                        marker = new RectanglesMarker(new Rectangle[] { wnd, obj.Rectangle });
                    }
                    else if (/*drawObjectBoxToolStripMenuItem.Checked*/true)
                    {
                        marker = new RectanglesMarker(obj.Rectangle);
                    }
                    else if (/*drawTrackingWindowToolStripMenuItem.Checked*/true)
                    {
                        marker = new RectanglesMarker(wnd);
                    }
                    else
                    {
                        marker = null;
                    }


                    if (marker != null)
                        marker.ApplyInPlace(im);
                    args.Frame = im.ToManagedImage();
                }
                else
                {
                    if (marker != null)
                        args.Frame = marker.Apply(args.Frame);
                }

            }
        }

        // Open video source
        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close previous video source
            CloseVideoSource();

            // start new video source
            videoSourcePlayer1.VideoSource = source;
            videoSourcePlayer1.Start();

            // reset statistics
            statIndex = statReady = 0;

            // start timers
            timer.Start();

            videoSource = source;

            this.Cursor = Cursors.Default;

            //objectsCountLabel.Text = "Double-click the screen to find faces!";
        }

        // Close current video source
        private void CloseVideoSource()
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer1.SignalToStop();

            // wait 2 seconds until camera stops
            for (int i = 0; (i < 50) && (videoSourcePlayer1.IsRunning); i++)
            {
                Thread.Sleep(100);
            }
            if (videoSourcePlayer1.IsRunning)
                videoSourcePlayer1.Stop();

            // stop timers
            timer.Stop();

            // reset motion detector
            tracker = new Camshift();
            tracker.Mode = /*hSLToolStripMenuItem.Checked*/true ? CamshiftMode.HSL :
                           /*mixedToolStripMenuItem.Checked */false ? CamshiftMode.Mixed :
                           CamshiftMode.RGB;
            tracker.Conservative = true;
            tracker.AspectRatio = 1.5f;

            videoSourcePlayer1.BorderColor = Color.Black;
            this.Cursor = Cursors.Default;
        }

        private static VideoCapabilities selectResolution(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Height == 240)
                    return cap;
                if (cap.FrameSize.Width == 320)
                    return cap;
            }

            return device.VideoCapabilities.Last();
        }

        private void videoSourcePlayer1_Click(object sender, EventArgs e)
        {
            detecting = true;
        }
    }
}
