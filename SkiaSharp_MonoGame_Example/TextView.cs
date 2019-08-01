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
    public class TextLine
    {
        public TextLine(int width, string line)
        {
            Width = width;
            Line = line;
        }

        public int Width { get; }
        public string Line { get; }
    }

    public class TextView
    {
        public static SKTypeface DefaultTypeface;

        public static string[] NewLines = new[] { Environment.NewLine, "\n", "\r", "\r\n" };

        static TextView()
        {
            DefaultTypeface = SKTypeface.FromFile("Content/simsun.ttf");
        }

        public string Text { get; set; } = string.Empty;
        public Texture2D Texture { get; private set; }
        public bool MultilineMode { get; set; } = false;
        public Color TextColor { get; set; } = Color.Black;

        public int RealWidth { get; private set; }
        public int RealHeight { get; private set; }

        private Point _size;
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

        public List<TextLine> SplitLines(SKPaint paint)
        {
            var result = new List<TextLine>();

            var lines = Text.Split(NewLines, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (_size.X <= 0)
                {
                    var width = (int)paint.MeasureText(line);
                    result.Add(new TextLine(width, line));
                    continue;
                }

                int restLineLength = line.Length;
                var restLineText = line;
                while (restLineLength > 0)
                {
                    var measuredLength = (int)paint.BreakText(restLineText, _size.X, out float measuredWidth, out string measuredText);

                    restLineLength -= measuredLength;
                    restLineText = restLineText.Substring(measuredLength, restLineLength);

                    if (measuredWidth > RealWidth)
                        RealWidth = (int)measuredWidth;

                    result.Add(new TextLine((int)measuredWidth, measuredText));
                }
            }

            RealHeight = (int)((result.Count * paint.FontSpacing) - paint.FontMetrics.Leading);

            return result;
        }

        public void Draw(SpriteBatch batch, Point position, Color color)
        {
            batch.Begin();

            if (Texture == null)
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                using (SKPaint paint = new SKPaint())
                {
                    paint.Typeface = DefaultTypeface;
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
                    //paint.StrokeCap = SKStrokeCap.Square;
                    //paint.StrokeWidth = 1;
                    //paint.IsStroke = true;

                    canvas.Clear(SKColor.Empty);

                    var textOffsetY = -paint.FontMetrics.Ascent;
                    var underline = paint.FontMetrics.UnderlinePosition ?? paint.FontMetrics.Descent;
                    var underlineOffsetY = textOffsetY + underline;

                    var lines = SplitLines(paint);

                    int counter = 0;
                    foreach (var line in lines)
                    {
                        var textPosition = new SKPoint(0, textOffsetY + counter * paint.FontSpacing);
                        canvas.DrawText(line.Line, textPosition, paint);

                        // Enable Underline Effect
                        {
                            var underlinePosition1 = new SKPoint(0, underlineOffsetY + counter * paint.FontSpacing);
                            var underlinePosition2 = new SKPoint(0 + line.Width, underlineOffsetY + counter * paint.FontSpacing);
                            paint.StrokeWidth = (int)(paint.FontMetrics.UnderlineThickness ?? 1);
                            paint.IsStroke = true;
                            canvas.DrawLine(underlinePosition1, underlinePosition2, paint);

                            paint.IsStroke = false;
                        }

                        counter++;
                    }

                    // Top of Text box.
                    canvas.DrawLine(new SKPoint(0, 1), new SKPoint(RealWidth, 1), paint);
                    // Bottom of Text box.
                    canvas.DrawLine(new SKPoint(0, 1 + RealHeight), new SKPoint(RealWidth, 1 + RealHeight), paint);
                    // Bottom of canvas.
                    canvas.DrawLine(new SKPoint(0, _size.Y - 1), new SKPoint(RealWidth, _size.Y - 1), paint);

                    Texture = new Texture2D(batch.GraphicsDevice, _size.X, _size.Y, false, SurfaceFormat.Color);
                    Texture.SetData(bitmap.Bytes);
                }
            }

            batch.Draw(Texture, position.ToVector2(), color);

            batch.End();
        }
    }
}
