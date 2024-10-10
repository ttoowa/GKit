using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GKitForWPF.UI.Controls;

public class NinePatchImage : ContentControl {
    private ImageSource[] patchs;
    
    [Category("Background")]
    [Description("Set nine patch background image")]
    public ImageSource ImageSource {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
    
    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(NinePatchImage), new PropertyMetadata(null, SetImage));
    
    [Category("Background")]
    [Description("Set the center split area")]
    public Thickness SideAspect {
        get => (Thickness)GetValue(SideAspectProperty);
        set => SetValue(SideAspectProperty, value);
    }
    
    public static readonly DependencyProperty SideAspectProperty =
        DependencyProperty.Register("CenterArea", typeof(Thickness), typeof(NinePatchImage), new PropertyMetadata(new Thickness(0.3d, 0.3d, 0.3d, 0.3d), SetArea));
    
    // [ Constructor ]
    public NinePatchImage() : base() {
    }
    
    // [ Event ]
    protected override void OnRender(DrawingContext drawingContext) {
        if (patchs != null) {
            double x1 = patchs[0].Width, x2 = Math.Max(ActualWidth - patchs[2].Width, 0);
            double y1 = patchs[0].Height, y2 = Math.Max(ActualHeight - patchs[6].Height, 0);
            double w1 = patchs[0].Width, w2 = Math.Max(x2 - x1, 0), w3 = patchs[2].Width;
            double h1 = patchs[0].Height, h2 = Math.Max(y2 - y1, 0), h3 = patchs[6].Height;
            Rect[] rects = new[] {
                new Rect(0, 0, w1, h1),
                new Rect(x1, 0, w2, h1),
                new Rect(x2, 0, w3, h1),
                new Rect(0, y1, w1, h2),
                new Rect(x1, y1, w2, h2),
                new Rect(x2, y1, w3, h2),
                new Rect(0, y2, w1, h3),
                new Rect(x1, y2, w2, h3),
                new Rect(x2, y2, w3, h3)
            };
            for (int i = 0; i < 9; i++) {
                drawingContext.DrawImage(patchs[i], rects[i]);
            }
        }
        
        base.OnRender(drawingContext);
    }
    
    private static void SetArea(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
        NinePatchImage ninePatch = dependencyObject as NinePatchImage;
        if (ninePatch == null)
            return;
        BitmapSource bitmap = ninePatch.ImageSource as BitmapSource;
        
        SetPatchs(ninePatch, bitmap);
    }
    
    private static void SetImage(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
        NinePatchImage nimePatch = dependencyObject as NinePatchImage;
        if (nimePatch == null)
            return;
        
        BitmapSource bitmap = nimePatch.ImageSource as BitmapSource;
        
        SetPatchs(nimePatch, bitmap);
    }
    
    private static void SetPatchs(NinePatchImage ninePatch, BitmapSource bitmap) {
        if (bitmap == null) {
            ninePatch.patchs = null;
            
            return;
        }
        
        try {
            int x1 = (int)(ninePatch.SideAspect.Left * bitmap.PixelWidth);
            int x2 = (int)((1d - ninePatch.SideAspect.Right) * bitmap.PixelWidth);
            
            int y1 = (int)(ninePatch.SideAspect.Top * bitmap.PixelHeight);
            int y2 = (int)((1d - ninePatch.SideAspect.Bottom) * bitmap.PixelHeight);
            
            int maxWidth = bitmap.PixelWidth - 1;
            int w1 = (int)x1;
            int w2 = (int)((1d - ninePatch.SideAspect.Right - ninePatch.SideAspect.Left) * bitmap.PixelWidth);
            int w3 = (int)(ninePatch.SideAspect.Right * bitmap.PixelWidth);
            x1 = Mathf.Min(x1, maxWidth);
            x2 = Mathf.Min(x2, maxWidth);
            w1 = Mathf.Clamp(w1, 1, maxWidth);
            w2 = Mathf.Clamp(w2, 1, maxWidth);
            w3 = Mathf.Clamp(w3, 1, maxWidth);
            
            int maxHeight = bitmap.PixelHeight - 1;
            int h1 = (int)y1;
            int h2 = (int)((1d - ninePatch.SideAspect.Top - ninePatch.SideAspect.Bottom) * bitmap.PixelHeight);
            int h3 = (int)(ninePatch.SideAspect.Bottom * bitmap.PixelHeight);
            y1 = Mathf.Min(y1, maxHeight);
            y2 = Mathf.Min(y2, maxHeight);
            h1 = Mathf.Clamp(h1, 1, maxHeight);
            h2 = Mathf.Clamp(h2, 1, maxHeight);
            h3 = Mathf.Clamp(h3, 1, maxHeight);
            
            
            ninePatch.patchs = new[] {
                new CroppedBitmap(bitmap, new Int32Rect(0, 0, w1, h1)),
                new CroppedBitmap(bitmap, new Int32Rect(x1, 0, w2, h1)),
                new CroppedBitmap(bitmap, new Int32Rect(x2, 0, w3, h1)),
                new CroppedBitmap(bitmap, new Int32Rect(0, y1, w1, h2)),
                new CroppedBitmap(bitmap, new Int32Rect(x1, y1, w2, h2)),
                new CroppedBitmap(bitmap, new Int32Rect(x2, y1, w3, h2)),
                new CroppedBitmap(bitmap, new Int32Rect(0, y2, w1, h3)),
                new CroppedBitmap(bitmap, new Int32Rect(x1, y2, w2, h3)),
                new CroppedBitmap(bitmap, new Int32Rect(x2, y2, w3, h3))
            };
            ninePatch.InvalidateVisual();
        } catch (Exception ex) {
            ninePatch.patchs = null;
            
            return;
        }
    }
}