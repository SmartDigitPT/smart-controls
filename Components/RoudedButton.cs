using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartControls.Components
{

    public enum ButtonImageSizeMode
    {
        Normal,
        Stretch,
        Zoom
    }
    public class RoundedButton : Button
    {
        #region Fields

        private int _borderRadius = 20;
        private int _borderSize = 1;
        private Color _borderColor = Color.RoyalBlue;

        private Color _hoverBackColor = Color.FromArgb(180, 210, 240);
        private Color _pressedBackColor = Color.FromArgb(150, 190, 230);

        private ButtonImageSizeMode _imageSizeMode = ButtonImageSizeMode.Zoom;
        private Size _imageTargetSize = new Size(24, 24);

        private bool _isHovered = false;
        private bool _isPressed = false;

        #endregion

        #region Properties

        [Category("Appearance")]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value < 1 ? 1 : value;
                Invalidate();
                UpdateRegion();
            }
        }

        [Category("Appearance")]
        public int BorderSize
        {
            get => _borderSize;
            set
            {
                _borderSize = value < 0 ? 0 : value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color HoverBackColor
        {
            get => _hoverBackColor;
            set
            {
                _hoverBackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color PressedBackColor
        {
            get => _pressedBackColor;
            set
            {
                _pressedBackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public ButtonImageSizeMode ImageSizeMode
        {
            get => _imageSizeMode;
            set
            {
                _imageSizeMode = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Size ImageTargetSize
        {
            get => _imageTargetSize;
            set
            {
                _imageTargetSize = value;
                Invalidate();
            }
        }

        #endregion

        #region Constructor

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer,
                     true);

            BackColor = Color.LightSteelBlue;
            ForeColor = Color.Black;

            UpdateRegion();
        }

        #endregion

        #region Events

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            _isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _isPressed = false;
            Invalidate();
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color currentBackColor = BackColor;

            if (_isPressed)
                currentBackColor = _pressedBackColor;
            else if (_isHovered)
                currentBackColor = _hoverBackColor;

            RectangleF rectSurface = new RectangleF(
                0,
                0,
                Width,
                Height);

            using (GraphicsPath pathBorder = GetRoundedPath(rectSurface, _borderRadius))
            {
                // 🔴 1️⃣ Desenha a borda como área preenchida
                if (_borderSize > 0)
                {
                    using (SolidBrush borderBrush = new SolidBrush(_borderColor))
                        e.Graphics.FillPath(borderBrush, pathBorder);
                }

                // 🔵 2️⃣ Desenha o fundo interno
                RectangleF rectInner = rectSurface;

                if (_borderSize > 0)
                {
                    rectInner = new RectangleF(
                        _borderSize,
                        _borderSize,
                        Width - (_borderSize * 2),
                        Height - (_borderSize * 2));
                }

                float innerRadius = _borderRadius - _borderSize;
                if (innerRadius < 0)
                    innerRadius = 0;

                using (GraphicsPath pathInner = GetRoundedPath(rectInner, innerRadius))
                using (SolidBrush backBrush = new SolidBrush(currentBackColor))
                {
                    e.Graphics.FillPath(backBrush, pathInner);
                }

                // 🖼 Desenha imagem + texto
                DrawImageAndText(e.Graphics, Rectangle.Round(rectInner));
            }
        }

        #endregion

        #region Drawing Helpers

        private void DrawImageAndText(Graphics g, Rectangle bounds)
        {
            int spacing = 6;
            int totalWidth = 0;

            Size textSize = TextRenderer.MeasureText(Text, Font);
            Size imageSize = Image != null ? _imageTargetSize : Size.Empty;

            if (Image != null)
                totalWidth = imageSize.Width + spacing + textSize.Width;
            else
                totalWidth = textSize.Width;

            int startX = bounds.X + (bounds.Width - totalWidth) / 2;
            int centerY = bounds.Y + bounds.Height / 2;

            if (Image != null)
            {
                Rectangle imageRect = new Rectangle(
                    startX,
                    centerY - imageSize.Height / 2,
                    imageSize.Width,
                    imageSize.Height);

                DrawImage(g, imageRect);

                startX += imageSize.Width + spacing;
            }

            Rectangle textRect = new Rectangle(
                startX,
                centerY - textSize.Height / 2,
                textSize.Width,
                textSize.Height);

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
        private void DrawImage(Graphics g, Rectangle rect)
        {
            if (Image == null)
                return;

            Size size = _imageTargetSize;

            if (size.Width <= 0 || size.Height <= 0)
                size = Image.Size;

            Rectangle drawRect = new Rectangle(
                rect.X,
                rect.Y,
                size.Width,
                size.Height);

            switch (_imageSizeMode)
            {
                case ButtonImageSizeMode.Normal:
                    g.DrawImage(Image, drawRect.Location);
                    break;

                case ButtonImageSizeMode.Stretch:
                    g.DrawImage(Image, drawRect);
                    break;

                case ButtonImageSizeMode.Zoom:
                    Rectangle zoomRect = GetZoomRectangle(Image.Size, drawRect);
                    g.DrawImage(Image, zoomRect);
                    break;
            }
        }
        //private void DrawImage(Graphics g, Rectangle rect)
        //{
        //    if (Image == null)
        //        return;

        //    switch (_imageSizeMode)
        //    {
        //        case ButtonImageSizeMode.Normal:
        //            g.DrawImage(Image, rect.Location);
        //            break;

        //        case ButtonImageSizeMode.Stretch:
        //            g.DrawImage(Image, rect);
        //            break;

        //        case ButtonImageSizeMode.Zoom:
        //            Rectangle zoomRect = GetZoomRectangle(Image.Size, rect);
        //            g.DrawImage(Image, zoomRect);
        //            break;
        //    }
        //}

        private Rectangle GetZoomRectangle(Size imageSize, Rectangle target)
        {
            float ratioX = (float)target.Width / imageSize.Width;
            float ratioY = (float)target.Height / imageSize.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int width = (int)(imageSize.Width * ratio);
            int height = (int)(imageSize.Height * ratio);

            int x = target.X + (target.Width - width) / 2;
            int y = target.Y + (target.Height - height) / 2;

            return new Rectangle(x, y, width, height);
        }

        private GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0.0f)
            {
                path.AddRectangle(rect);
                return path;
            }

            float r = radius * 2f;

            // Clamp para evitar distorção
            if (r > rect.Width) r = rect.Width;
            if (r > rect.Height) r = rect.Height;

            float x = rect.X;
            float y = rect.Y;
            float w = rect.Width;
            float h = rect.Height;

            float right = x + w;
            float bottom = y + h;

            // Constante mágica para curva perfeita (Bézier circle approximation)
            float k = 0.55228475f;

            float c = (r / 2f) * k;

            path.StartFigure();

            // Top Left
            path.AddBezier(
                x, y + r / 2f,
                x, y + r / 2f - c,
                x + r / 2f - c, y,
                x + r / 2f, y);

            // Top
            path.AddLine(x + r / 2f, y, right - r / 2f, y);

            // Top Right
            path.AddBezier(
                right - r / 2f, y,
                right - r / 2f + c, y,
                right, y + r / 2f - c,
                right, y + r / 2f);

            // Right
            path.AddLine(right, y + r / 2f, right, bottom - r / 2f);

            // Bottom Right
            path.AddBezier(
                right, bottom - r / 2f,
                right, bottom - r / 2f + c,
                right - r / 2f + c, bottom,
                right - r / 2f, bottom);

            // Bottom
            path.AddLine(right - r / 2f, bottom, x + r / 2f, bottom);

            // Bottom Left
            path.AddBezier(
                x + r / 2f, bottom,
                x + r / 2f - c, bottom,
                x, bottom - r / 2f + c,
                x, bottom - r / 2f);

            // Left
            path.AddLine(x, bottom - r / 2f, x, y + r / 2f);

            path.CloseFigure();

            return path;
        }

        private void UpdateRegion()
        {
            using (GraphicsPath path = GetRoundedPath(
                new RectangleF(0, 0, Width, Height),
                _borderRadius))
            {
                Region = new Region(path);
            }
        }

        #endregion
    }
}