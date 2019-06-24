#if OnWPF
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SRect = System.Windows.Rect;


namespace GKit {
	public static class BitmapUtility {
		public static BitmapSource OpenImageFile() {
			BitmapSource image;
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "이미지 파일 (*.png;*.jpg;*.jpeg;*.gif;*.tiff;*.bmp)|*.png;*.jpg;*.gif;*.tiff;*.bmp|All files (*.*)|*.*";
			dialog.Title = "이미지 파일을 선택하세요.";
			try {
				bool? result = dialog.ShowDialog();
				if (result.HasValue && result.Value) {
					image = dialog.FileName.GetBitmap();

					return image;
				}
			} catch (Exception ex) {
				GDebug.Log("Failed to load image. " + ex.ToString());
			}
			return null;
		}
		public static BitmapImage GetBitmap(this string filePath) {
			try {
				FileInfo fileInfo = new FileInfo(filePath);
				if (fileInfo.Exists) {
					BitmapImage image = new BitmapImage();
					image.BeginInit();
					image.UriSource = new Uri(filePath);
					image.CreateOptions = BitmapCreateOptions.DelayCreation;
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.EndInit();
					return image;
				}
			} catch (Exception ex) {
				MessageBox.Show("Failed to load image." + Environment.NewLine + ex.ToString());
			}
			return null;
		}
		public static BitmapImage GetBitmapResource(this string path) {
			return new BitmapImage(new Uri("pack://application:,,,/Resources/" + path, UriKind.RelativeOrAbsolute));
		}
		public static BitmapSource Resize(this ImageSource source, int width, int height, int dpi = 96) {
			return source.Resize(width, height, PixelFormats.Default, dpi);
		}
		public static BitmapSource Resize(this ImageSource source, int width, int height, PixelFormat format, int dpi = 96) {
			if(source.Width == width && source.Height == height) {
				return source as BitmapSource;
			}

			SRect rect = new SRect(0f, 0f, width, height);

			DrawingGroup group = new DrawingGroup();
			RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
			group.Children.Add(new ImageDrawing(source, rect));

			DrawingVisual drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
				drawingContext.DrawDrawing(group);

			RenderTargetBitmap resizedImage = new RenderTargetBitmap(
				width, height,         // Resized dimensions
				dpi, dpi,                // Default DPI values
				format); // Default pixel format
			resizedImage.Render(drawingVisual);

			return BitmapFrame.Create(resizedImage);
		}
	}
}

#endif