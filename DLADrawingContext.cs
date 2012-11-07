using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DLA
{
    class DLADrawingContext : Canvas, INotifyPropertyChanged
    {
        const uint FILE_MAP_ALL_ACCESS = 0xF001F;
        const uint PAGE_READWRITE = 0x04;

        private Map map;                        // reference to DLAMap object
        private InteropBitmap interopBitmap;    // map allowing writing to memory
        IntPtr mapPointer;                      // pointer to the bitmap in memory
        Image image;                            // image control

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile,
        IntPtr lpFileMappingAttributes,
        uint flProtect,
        uint dwMaximumSizeHigh,
        uint dwMaximumSizeLow,
        string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,
        uint dwDesiredAccess,
        uint dwFileOffsetHigh,
        uint dwFileOffsetLow,
        uint dwNumberOfBytesToMap);

        // properties
        internal Color SeedColor { get; set; }
        internal Color TreeColor { get; set; }
        internal Color BackgroundColor { get; set; }
        public BitmapSource Source
        {
            get
            {
                interopBitmap.Invalidate();
                return (BitmapSource)interopBitmap;
            }
        }

        // initial map settings
        internal void SetMap(ref Map DLAMap)
        {
            map = DLAMap;
            map.GenerateSeed();
            CreateBitmap();
        }

        // drawing
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Draw(drawingContext);
        }

        // creates interop bitmap
        private void CreateBitmap()
        {
            if (map != null)
            {
                // byte count
                uint byteCount = (uint)(map.X * map.Y * PixelFormats.Bgr32.BitsPerPixel / 8);

                // allocate memory
                var sectionPointer = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, PAGE_READWRITE, 0, byteCount, null);
                mapPointer = MapViewOfFile(sectionPointer, FILE_MAP_ALL_ACCESS, 0, 0, byteCount);

                //byte[] pixels = new byte[byteCount];
                //Marshal.Copy(pixels, 0, mapPointer, (int)byteCount);
                
                // create the interopBitmap
                interopBitmap = Imaging.CreateBitmapSourceFromMemorySection(sectionPointer, map.X, map.Y, PixelFormats.Bgr32, (int)(map.X * PixelFormats.Bgr32.BitsPerPixel / 8), 0) as InteropBitmap;
                SetImage();
            }
        }

        // set the interopbitmap as Source to the wpf image
        internal void SetImage()
        {
            if (this.Children.Contains(image))
                this.Children.Remove(image);
            image = new Image();
            image.Name = "DLAImage";
            image.Height = this.ActualHeight;
            image.Width = this.ActualWidth;
            image.Stretch = Stretch.Fill;
            // scaling mode and aliasing
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
            this.VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor;
            image.Source = (BitmapSource)interopBitmap;
            this.Children.Add(image);
        }

        // draws the map
        private void Draw(DrawingContext drawingContext)
        {
            byte c = (byte)(Map.Step % 2);

            if (map != null)
            {
                unsafe
                {
                    double deltaX = this.ActualWidth / map.X;
                    double deltaY = this.ActualHeight / map.Y;
                    Point pos = new Point(0, 0);

                    uint* pBuffer = (uint*)mapPointer;
                    Color col;

                    for (int i = 0; i < map.X; ++i)
                    {
                        for (int j = 0; j < map.Y; ++j)
                        {
                            if (map[c, i, j] > Map.BLACK && map[c, i, j] < Map.GREEN)
                            {
                                col = SeedColor;
                                
                            }
                            else if (map[c, i, j] == Map.GREEN)
                            {
                                col = TreeColor;
                            }
                            else
                            {
                                col = BackgroundColor;
                            }
                            // create int describing color - shift left alpha channel, R, G, B and do logical 'or' operation 
                            pBuffer[(int)(i + j * map.X)] = (uint)((uint)0xFF << 24) | (uint)(col.R << 16) | (uint)(col.G << 8) | (uint)(col.B);
                            pos.Y += (int)deltaY;
                        }
                        pos.Y = 0;
                        pos.X += (int)deltaX;
                    }
                }
                // update memory
                interopBitmap.Invalidate();
                //NotifyPropertyChanged("Source");
            }
        }

        // not yet know what to do with it
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }


}
