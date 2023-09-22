/******************************************
 * Class name:
 * Author:
 * Creation:
 * Last modify:
 * Version:
 *
 * DESCRIPTION
 *
 *
 * *****************************************/

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace BeautySim2023
{
    public class BitmapUtils
    {
        private static string[] _img_file_ext = new string[] { ".jpg", ".bmp", ".gif", ".png" };

        public static System.Drawing.Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));
            System.Drawing.Bitmap bitmap = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return new System.Drawing.Bitmap(bitmap);
        }

        public static BitmapImage BitmapImageFromImage(System.Drawing.Image image)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            using (MemoryStream ms = new MemoryStream())
            {

                image.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
            }
            return bi;
        }

        public static List<BitmapImage> BitmapImageListFromResourceFolder(string folder, Assembly a)
        {
            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();
            List<string> resource_names = new List<string>();

            foreach (string s in resNames)
            {
                if (s.ToUpper().StartsWith(folder.ToUpper()))
                {
                    // attach to stream to the resource in the manifest
                    resource_names.Add(s);
                }
            }
            List<BitmapImage> ret = new List<BitmapImage>();
            var items = from db in resource_names orderby db ascending select db;
            foreach (string s in items)
            {
                Stream stream = a.GetManifestResourceStream(s);
                ret.Add(ImageFromBuffer(ByteImageFromStream(stream)));
            }

            return ret;
        }

        public static byte[] BufferFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                System.Drawing.Image img = new System.Drawing.Bitmap(filename);
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Create an image buffer form image filename of specified width
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static byte[] BufferFromFile(string filename, int width)
        {
            if (File.Exists(filename))
            {
                try
                {
                    //image
                    System.Drawing.Image img = new System.Drawing.Bitmap(filename);
                    //scale ratio height
                    int height = (int)(img.Height * (float)width / (float)img.Width);
                    //callback (not used)
                    System.Drawing.Image.GetThumbnailImageAbort myCallback =
                    new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
                    //get thumbnail
                    System.Drawing.Image thumb = img.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);

                    //convert png to byte array
                    MemoryStream ms = new MemoryStream();
                    thumb.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static byte[] BufferFromImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        public static Byte[] BufferFromImage(BitmapImage imageSource)
        {
            try
            {
                MemoryStream stream = (MemoryStream)imageSource.StreamSource;

                Byte[] buffer = null;
                stream.Seek(0, SeekOrigin.Begin);
                if (stream != null && stream.Length > 0)
                {
                    //buffer = new byte[stream.Length];
                    //stream.Read(buffer, 0, (int)stream.Length);
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        buffer = br.ReadBytes((Int32)stream.Length);
                    }
                }
                stream.Close();

                return buffer;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static byte[] ByteImageFromStream(Stream s)
        {
            if (s == null || s.Length == 0)
                return null;
            byte[] buffer = new byte[s.Length];
            s.Read(buffer, 0, (int)s.Length);
            return buffer;
        }

        public static byte[] ByteImageResourceName(string name)
        {
            Stream s = ExtractStream(name);
            return ByteImageFromStream(s);
        }

        public static System.Drawing.Image DrawingImageFromResource(string name, Assembly a)
        {
            try
            {
                Stream s = ExtractStream(name, a);
                System.Drawing.Image img = System.Drawing.Image.FromStream(s);
                return img;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static Stream ExtractStream(string name)
        {
            // get a reference to the current assembly
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            Stream resStream = null;
            foreach (string s in resNames)
            {
                if (s.ToUpper().EndsWith(name.ToUpper()))
                {
                    // attach to stream to the resource in the manifest
                    resStream = a.GetManifestResourceStream(s);
                    break;
                }
            }
            return resStream;
        }

        public static Stream ExtractStream(string name, Assembly a)
        {
            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            Stream resStream = null;
            foreach (string s in resNames)
            {
                if (s.ToUpper().EndsWith(name.ToUpper()))
                {
                    // attach to stream to the resource in the manifest
                    resStream = a.GetManifestResourceStream(s);
                    break;
                }
            }
            return resStream;
        }

        public static BitmapImage ImageFromBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }

        public static BitmapSource ImageFromBitmapFast(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            lock (bitmap)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();

                try
                {
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        System.Windows.Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }
        }

        public static BitmapImage ImageFromBuffer(Byte[] bytes, int width)
        {
            try
            {
                MemoryStream stream = new MemoryStream(bytes);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.DecodePixelWidth = width;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                stream.Close();
                stream.Dispose();
                stream = null;
                return image;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            try
            {
                MemoryStream stream = new MemoryStream(bytes);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                stream.Close();
                stream.Dispose();
                stream = null;
                return image;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static BitmapImage ImageFromFile(string file_name)
        {
            BitmapImage bi = new BitmapImage();

            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(file_name);
            bi.EndInit();
            return bi;
        }

        public static BitmapImage ImageFromResource(string name, int width)
        {
            return ImageFromBuffer(ByteImageResourceName(name), width);
        }

        public static BitmapImage ImageFromResource(string name, Assembly a)
        {
            Stream s = ExtractStream(name, a);
            byte[] buffer = ByteImageFromStream(s);
            return ImageFromBuffer(buffer);
        }

        public static BitmapImage ImageFromResource(string assembly_name, string name)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            Uri uri = null;

            uri = new Uri("pack://application:,,," + assembly_name + ";component/" + name, UriKind.Relative);

            if (uri != null)
                image.UriSource = uri;

            image.EndInit();

            return image;
        }

        public static bool IsImage(FileInfo f)
        {
            if (File.Exists(f.FullName))
            {
                return _img_file_ext.Contains(f.Extension.ToLower());
            }

            return false;
        }

        public static bool ThumbnailCallback()
        {
            return false;
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}