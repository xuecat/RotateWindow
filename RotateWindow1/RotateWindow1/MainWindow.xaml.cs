using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;


namespace RotateWindow1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        enum RotationDirection
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop,
            TopLeftToBottomRight,
            TopRightToBottomLeft,
            BottomLeftToTopRight,
            BottomRightToTopLeft
        };

        const int _animationLength = 600;
        bool _isRotating = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        void PrepareForRotation(
            out DoubleAnimation frontAnimation,
            out DoubleAnimation backAnimation,
            RotationDirection rotationDirection)
        {
            Vector3D axis;
            double delta;

            switch (rotationDirection)
            {
                case RotationDirection.LeftToRight:
                    axis = new Vector3D(0, +1, 0);
                    delta = +180;
                    break;

                case RotationDirection.RightToLeft:
                    axis = new Vector3D(0, +1, 0);
                    delta = -180;
                    break;

                case RotationDirection.TopToBottom:
                    axis = new Vector3D(+1, 0, 0);
                    delta = +180;
                    break;

                case RotationDirection.BottomToTop:
                    axis = new Vector3D(+1, 0, 0);
                    delta = -180;
                    break;

                case RotationDirection.TopLeftToBottomRight:
                    axis = new Vector3D(+1, +1, 0);
                    delta = +180;
                    break;

                case RotationDirection.TopRightToBottomLeft:
                    axis = new Vector3D(-1, +1, 0);
                    delta = -180;
                    break;

                case RotationDirection.BottomLeftToTopRight:
                    axis = new Vector3D(-1, +1, 0);
                    delta = +180;
                    break;

                case RotationDirection.BottomRightToTopLeft:
                    axis = new Vector3D(+1, +1, 0);
                    delta = -180;
                    break;

                default:
                    throw new ApplicationException("Unrecognized RotationDirection value: ");
            }

            frontaxis.Axis = backaxis.Axis = axis;

            frontAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(_animationLength)),
                From = frontaxis.Angle,
                To = frontaxis.Angle + delta
            };

            backAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(_animationLength)),
                From = backaxis.Angle,
                To = backaxis.Angle + delta
            };
        }

        void OnRotationCompleted(object sender, EventArgs e)
        {
            AnimationClock clock = sender as AnimationClock;
            clock.Completed -= this.OnRotationCompleted;

            this._isRotating = false;

            CommandManager.InvalidateRequerySuggested();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            if (btn != null)
            {
                string s = btn.Content.ToString();
                if (s == "关闭")
                {
                    this.Close();
                }

                if (_isRotating)
                    return;

                DoubleAnimation frontAnimation = null, backAnimation = null;

                Point3DAnimation cameraZoomAnim = new Point3DAnimation
                {
                    To = new Point3D(0, 0, 500),
                    Duration = new Duration(TimeSpan.FromMilliseconds(_animationLength / 2)),
                    AutoReverse = true,
                    FillBehavior = FillBehavior.Stop
                };
                cameraZoomAnim.Completed += this.OnRotationCompleted;

                if (s == "向前")
                {
                    this.PrepareForRotation(out frontAnimation, out backAnimation, RotationDirection.LeftToRight);
                }
                else if (s == "向后")
                {
                    this.PrepareForRotation(out frontAnimation, out backAnimation, RotationDirection.RightToLeft);
                }

                frontaxis.BeginAnimation(AxisAngleRotation3D.AngleProperty, frontAnimation);
                backaxis.BeginAnimation(AxisAngleRotation3D.AngleProperty, backAnimation);
                camera.BeginAnimation(PerspectiveCamera.PositionProperty, cameraZoomAnim);
                _isRotating = true;

            }
        }
    
    }
}
