using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkiaSharp;

namespace SkiaSharp_MonoGame_Example
{
    public class TextView
    {
        public string Text { get; set; } = string.Empty;
        public Texture2D Texture { get; private set; }
        public bool MultilineMode { get; set; } = false;
        public Color TextColor { get; set; } = Color.Black;

        public Point _size;
        public Point Size
        {
            get { return _size; }
            set
            {
                if (_size == value)
                    return;

                _size = value;
                Initialize();
            }
        }

        public uint _textSize = 14;
        public uint TextSize
        {
            get { return _textSize; }
            set
            {
                if (_textSize == value)
                    return;

                _textSize = value;
                Initialize();
            }
        }

        private SKBitmap bitmap;

        private void Initialize()
        {
            if (Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }

            if (bitmap != null)
                bitmap.Dispose();

            bitmap = new SKBitmap(_size.X, _size.Y, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        }

        public TextView(Point size, uint textSize = 14)
        {
            _size = size;
            _textSize = textSize;
            Initialize();
        }

        public TextView(string text, Point size, uint textSize = 14) : this(size, textSize)
        {
            Text = text;
        }

        const float ShadowOffsetX = 2.0F;
        const float ShadowOffsetY = 2.0F;
        const float ShadowSigmaX = 1.0F;
        const float ShadowSigmaY = 1.0F;

        public void Draw(SpriteBatch batch, Point position, Color color)
        {
            batch.Begin();

            if (Texture == null)
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                using (SKPaint paint = new SKPaint())
                {
                    paint.TextSize = _textSize;
                    paint.Color = new SKColor(TextColor.R, TextColor.G, TextColor.B, TextColor.A);

                    // Enable Text Shadow
                    paint.ImageFilter = SKImageFilter.CreateDropShadow(
                        ShadowOffsetX,
                        ShadowOffsetY,
                        ShadowSigmaX,
                        ShadowSigmaY,
                        SKColors.Black,
                        SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);

                    // Enable Text Stroke
                    paint.StrokeCap = SKStrokeCap.Square;
                    paint.StrokeWidth = 1;
                    paint.IsStroke = true;

                    // Enable fake Italic effect 
                    paint.TextSkewX = -0.5F;

                    SKRect textBounds = new SKRect();
                    paint.MeasureText(Text, ref textBounds);

                    canvas.Clear(SKColor.Empty);
                    canvas.DrawText(Text, new SKPoint(0, -textBounds.Top), paint);

                    Texture = new Texture2D(batch.GraphicsDevice, _size.X, _size.Y, false, SurfaceFormat.Color);
                    Texture.SetData(bitmap.Bytes);
                }
            }

            batch.Draw(Texture, position.ToVector2(), color);

            batch.End();
        }
    }
}
