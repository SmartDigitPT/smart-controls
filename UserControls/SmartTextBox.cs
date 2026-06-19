using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace SmartControls.UserControls
{

    #region ENUMS

    public enum InnerIconType
    {
        None,
        Add,
        Alarm,
        Apps,
        Archive,
        ArrowDown,
        ArrowLeft,
        ArrowRight,
        ArrowUp,
        Attachment,
        Back,
        Basket,
        Bell,
        BellOff,
        Bluetooth,
        Book,
        Bookmark,
        Building,
        Bug,
        Calculator,
        Calendar,
        CalendarAdd,
        Camera,
        Cancel,
        Car,
        Cart,
        Chart,
        ChartBar,
        ChartLine,
        Chat,
        Check,
        Checklist,
        ChevronDown,
        ChevronLeft,
        ChevronRight,
        ChevronUp,
        Clipboard,
        Close,
        Cloud,
        CloudDownload,
        CloudUpload,
        Code,
        Collapse,
        Comment,
        Compass,
        Copy,
        CreditCard,
        Cut,
        Dashboard,
        Database,
        Desktop,
        DeviceLaptop,
        DevicePhone,
        Document,
        DocumentSearch,
        Download,
        Drag,
        Edit,
        Email,
        Error,
        Excel,
        Expand,
        Export,
        Eye,
        EyeHide,
        File,
        FileAdd,
        FileRemove,
        Filter,
        Flag,
        Folder,
        FolderOpen,
        Forward,
        Fullscreen,
        Gift,
        Globe,
        Grid,
        Heart,
        Help,
        History,
        Home,
        Image,
        Import,
        Inbox,
        Info,
        Key,
        Link,
        List,
        Location,
        Lock,
        MailOpen,
        Menu,
        Microphone,
        Money,
        Moon,
        More,
        Music,
        Paste,
        Pause,
        Pdf,
        People,
        Phone,
        Pin,
        PinOff,
        Play,
        Plus,
        Print,
        QrCode,
        Receipt,
        Redo,
        Refresh,
        Remove,
        Reply,
        Save,
        Scan,
        Search,
        Send,
        Settings,
        Share,
        Shield,
        ShieldCheck,
        ShieldError,
        ShoppingBag,
        Star,
        Stop,
        Sync,
        Tag,
        Terminal,
        Time,
        Trash,
        Truck,
        Undo,
        Unlock,
        Unlink,
        Upload,
        User,
        UserAdd,
        UserRemove,
        Video,
        VolumeHigh,
        VolumeLow,
        VolumeMute,
        Wallet,
        Warning,
        Wifi,
        Window,
        Wrench,
        ZoomIn,
        ZoomOut
    }

    public enum InnerIconPosition
    {
        Left,
        Right
    }

    public enum ValidationMode
    {
        None,
        Required,
        Email,
        NIF,
        NIB,
        Telephone,
        URL,
        Number,
        TextWithoutSpace,
        CustomRegex
    }

    #endregion
    public class SmartTextBox : UserControl
    {


        private bool showErrorVisual = false;
        private Timer errorTimer = new Timer();

        private System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
        private Timer animationTimer = new Timer();
        private ToolTip toolTip = new ToolTip();


        private const string IconFont = "Segoe MDL2 Assets";


        private InnerIconType innerIcon = InnerIconType.None;
        private InnerIconPosition innerIconPosition = InnerIconPosition.Left;
        private ValidationMode validationMode = ValidationMode.None;

        private int borderRadius = 10;
        private int borderThickness = 1;
        private int iconTextSpacing = 12;

        private bool showBorderFocus = true;
        private bool autoValidationMode = true;
        private bool showErrorIcon = true;
        private bool isPassword = false;
        private bool isPasswordVisible = false;

        private int timeToolTipText = 5;

        private Color borderColor = Color.Black;
        private Color borderFocusColor = Color.FromArgb(0, 120, 215);
        private Color borderErrorColor = Color.Red;

        private bool isValid = true;
        private bool isFocused = false;
        private bool forcedError = false;

        private float focusProgress = 0f;
        private float errorFadeProgress = 0f;

        private string customRegexPattern = "";
        private string validationMessage = "Invalid value.";
        private string errorMessage = "Invalid value.";
        private string focusMessage = "";
        private bool innerIconPickerPending = false;

        #region PROPERTIES

        [Browsable(true)]
        public override string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsPassword
        {
            get => isPassword;
            set
            {
                isPassword = value;
                if (!isPassword)
                    isPasswordVisible = false;

                UpdatePasswordMaskState();
                AdjustLayout();
                Invalidate();
            }
        }

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                textBox.Font = value;
                AdjustLayout();
            }
        }

        public override Color ForeColor
        {
            get => textBox.ForeColor;
            set => textBox.ForeColor = value;
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                textBox.BackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public int BorderRadius { get => borderRadius; set { borderRadius = value; Invalidate(); } }

        [Category("Appearance")]
        public int BorderThickness { get => borderThickness; set { borderThickness = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderColor { get => borderColor; set { borderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderFocusColor { get => borderFocusColor; set { borderFocusColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderErrorColor { get => borderErrorColor; set { borderErrorColor = value; Invalidate(); } }

        [Category("Appearance")]
        public bool ShowBorderFocus { get => showBorderFocus; set { showBorderFocus = value; Invalidate(); } }

        [Category("Appearance")]
        public bool ShowErrorIcon { get => showErrorIcon; set { showErrorIcon = value; Invalidate(); } }

        [Category("Appearance")]
        [Browsable(true)]
        [DefaultValue(InnerIconType.None)]
        [RefreshProperties(RefreshProperties.All)]
        public InnerIconType InnerIcon { get => innerIcon; set { innerIcon = value; AdjustLayout(); Invalidate(); } }

        [Category("Appearance")]
        [Browsable(true)]
        [DisplayName("InnerIcon Picker")]
        [Description("Marque para abrir o formulário de seleção de ícones por categoria (Designer).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool InnerIconPicker
        {
            get => false;
            set
            {
                if (!value)
                    return;

                bool isDesignTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                                    (Site != null && Site.DesignMode);

                if (!isDesignTime)
                    return;

                if (innerIconPickerPending)
                    return;

                innerIconPickerPending = true;

                if (IsHandleCreated)
                    BeginInvoke(new Action(OpenInnerIconPickerDeferred));
                else
                    OpenInnerIconPickerDeferred();
            }
        }

        [Category("Custom")]
        public InnerIconPosition InnerIconPosition { get => innerIconPosition; set { innerIconPosition = value; AdjustLayout(); Invalidate(); } }

        [Category("Validation")]
        public ValidationMode ValidationMode { get => validationMode; set => validationMode = value; }

        [Category("Validation")]
        public bool AutoValidationMode { get => autoValidationMode; set => autoValidationMode = value; }

        [Category("Validation")]
        public string CustomRegexPattern { get => customRegexPattern; set => customRegexPattern = value; }

        [Category("Validation")]
        public string ValidationMessage { get => validationMessage; set => validationMessage = value; }

        [Category("Validation")]
        public string ErrorMessage { get => errorMessage; set => errorMessage = value; }

        [Category("Behavior")]
        public string FocusMessage { get => focusMessage; set => focusMessage = value; }

        [Category("Behavior")]
        public int TimeToolTipText { get => timeToolTipText; set => timeToolTipText = value < 1 ? 1 : value; }

        [Browsable(false)]
        public bool IsValid => isValid;

        #endregion

        private void OpenInnerIconPickerDeferred()
        {
            try
            {
                if (InnerIconEnumEditor.TryPickEnumValue(typeof(InnerIconType), innerIcon, out object selected) &&
                    selected is InnerIconType icon)
                {
                    InnerIcon = icon;
                }
            }
            catch
            {
                // Evita bloquear o Designer em caso de erro no picker.
            }
            finally
            {
                innerIconPickerPending = false;
            }
        }

        #region CONSTRUCTOR

        public SmartTextBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Height = 40;

            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Color.White;

            textBox.TextChanged += OnTextChanged;
            textBox.KeyDown += OnKeyDown;
            textBox.Leave += OnLeave;
            textBox.LostFocus += OnLostFocus;

            Controls.Add(textBox);

            animationTimer.Interval = 15;
            animationTimer.Tick += Animate;

            Resize += (s, e) => AdjustLayout();

            DoubleBuffered = true;
            Height = 40;

            borderColor = Color.Black;
            borderErrorColor = Color.Red;
            borderFocusColor = Color.FromArgb(0, 120, 215);

            BackColor = Color.White;
            textBox.BackColor = Color.White;
            textBox.ForeColor = Color.Black;

            errorTimer.Tick += (s, e) =>
            {
                errorTimer.Stop();
                showErrorVisual = false;
                animationTimer.Start();
                Invalidate();
            };

            AdjustLayout();
        }
        public void FocusTextBox()
        {
            ActiveControl = textBox;
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            isFocused = true;

            if (!string.IsNullOrWhiteSpace(focusMessage))
            {
                toolTip.Show(
                    focusMessage,
                    this,
                    Width / 2,
                    Height,
                    timeToolTipText * 1000);
            }

            animationTimer.Start();
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            isFocused = false;
            animationTimer.Start();
        }

        #endregion

        #region VALIDATION

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (autoValidationMode)
                ValidateText();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!autoValidationMode && e.KeyCode == Keys.Enter)
            {
                ValidateText();
                e.SuppressKeyPress = true;
            }

            // Bubble the key event up so subscribers of this UserControl's KeyDown
            // (e.g. TbVoucher_KeyDown in Transaction) fire correctly.
            // OnKeyDown(KeyEventArgs) is Control.OnKeyDown — different signature from
            // this private handler, so there is no recursion.
            OnKeyDown(e);
        }

        private void OnLeave(object sender, EventArgs e)
        {
            if (!autoValidationMode)
                ValidateText();
        }

        private void ValidateText()
        {
            if (validationMode == ValidationMode.None)
            {
                isValid = true;
                toolTip.Hide(this);
                Invalidate();
                return;
            }

            if (validationMode == ValidationMode.Required)
            {
                isValid = !string.IsNullOrWhiteSpace(textBox.Text);
            }
            else
            {
                string pattern = GetPattern();
                isValid = string.IsNullOrEmpty(pattern) ||
                          Regex.IsMatch(textBox.Text ?? "", pattern);
            }

            if (!isValid)
            {
                showErrorVisual = true;

                toolTip.Show(
                    autoValidationMode ? validationMessage : errorMessage,
                    this,
                    Width / 2,
                    Height,
                    timeToolTipText * 1000);

                errorTimer.Interval = timeToolTipText * 1000;
                errorTimer.Start();
            }
            else
            {
                showErrorVisual = false;
                toolTip.Hide(this);
            }

            animationTimer.Start();

            AdjustLayout();

            Invalidate();
        }

        private string GetPattern()
        {
            switch (validationMode)
            {
                case ValidationMode.Email: return @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                case ValidationMode.NIF: return @"^\d{9}$";
                case ValidationMode.NIB: return @"^\d{21}$";
                case ValidationMode.Telephone: return @"^9\d{8}$";
                case ValidationMode.URL: return @"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+";
                case ValidationMode.Number: return @"^\d+$";
                case ValidationMode.TextWithoutSpace: return @"^\S+$";
                case ValidationMode.CustomRegex: return customRegexPattern;
                default: return null;
            }
        }

        #endregion

        #region ANIMATION

        private void Animate(object sender, EventArgs e)
        {
            float speed = 0.1f;

            focusProgress = isFocused
                ? Math.Min(1f, focusProgress + speed)
                : Math.Max(0f, focusProgress - speed);

            errorFadeProgress = !isValid
                ? Math.Min(1f, errorFadeProgress + speed)
                : Math.Max(0f, errorFadeProgress - speed);

            if ((focusProgress == 0f || focusProgress == 1f) &&
                (errorFadeProgress == 0f || errorFadeProgress == 1f))
                animationTimer.Stop();

            Invalidate();
        }

        #endregion

        #region LAYOUT

        private void AdjustLayout()
        {
            int padding = 12;
            int iconSize = (int)(Height * 0.55f);

            int left = padding;
            int right = padding;

            if (innerIcon != InnerIconType.None)
            {
                if (innerIconPosition == InnerIconPosition.Left)
                    left += iconSize + iconTextSpacing;
                else
                    right += iconSize + iconTextSpacing;
            }

            if (showErrorVisual && showErrorIcon)
            {
                if (innerIcon == InnerIconType.None)
                {
                    right += iconSize + 6;
                }
                else
                {
                    if (innerIconPosition == InnerIconPosition.Left)
                        right += iconSize + 6;
                    else
                        left += iconSize + 6;
                }
            }

            if (isPassword)
                right += iconSize + 6;

            textBox.Location = new Point(left, (Height - textBox.Font.Height) / 2);
            textBox.Width = Width - left - right;
        }

        #endregion

        #region PAINT

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (SolidBrush brush =
                   new SolidBrush(Parent?.BackColor ?? SystemColors.Control))
                e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(
                borderThickness,
                borderThickness,
                Width - borderThickness * 2 - 1,
                Height - borderThickness * 2 - 1);

            using (GraphicsPath path = GetRoundedRect(rect, borderRadius))
            using (SolidBrush brush = new SolidBrush(BackColor))
                e.Graphics.FillPath(brush, path);

            Color border = !isValid
                ? borderErrorColor
                : (showBorderFocus
                    ? InterpolateColor(borderColor, borderFocusColor, focusProgress)
                    : borderColor);

            using (Pen pen = new Pen(border, borderThickness))
            using (GraphicsPath path = GetRoundedRect(rect, borderRadius))
                e.Graphics.DrawPath(pen, path);

            DrawInnerIcon(e.Graphics);
            DrawErrorIcon(e.Graphics);
            DrawPasswordIcon(e.Graphics);
        }

        private void DrawInnerIcon(Graphics g)
        {
            if (innerIcon == InnerIconType.None) return;

            float size = Height * 0.5f;

            using (Font font = new Font(IconFont, size, GraphicsUnit.Pixel))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                string glyph = GetGlyph(innerIcon);
                SizeF s = g.MeasureString(glyph, font);

                float x = innerIconPosition == InnerIconPosition.Left
                    ? 12
                    : Width - s.Width - 12;

                float y = (Height - s.Height) / 2;

                g.DrawString(glyph, font, brush, x, y);
            }
        }

        private void DrawErrorIcon(Graphics g)
        {
            if (isValid || !showErrorVisual || !showErrorIcon)
                return;

            float size = Height * 0.55f;
            float padding = 12;

            float x = innerIconPosition == InnerIconPosition.Left
                ? Width - size - padding
                : padding;

            if (isPassword && innerIconPosition == InnerIconPosition.Left)
                x -= size + 6;

            float y = (Height - size) / 2;

            int alpha = (int)(255 * errorFadeProgress);

            using (GraphicsPath triangle = new GraphicsPath())
            {
                triangle.AddPolygon(new[]
                {
                    new PointF(x + size/2, y),
                    new PointF(x + size, y + size),
                    new PointF(x, y + size)
                });

                using (SolidBrush brush =
                       new SolidBrush(Color.FromArgb(alpha, Color.Goldenrod)))
                    g.FillPath(brush, triangle);
            }

            using (SolidBrush ex =
                   new SolidBrush(Color.FromArgb(alpha, Color.Black)))
            {
                float lw = size * 0.1f;
                g.FillRectangle(ex,
                    x + size / 2 - lw / 2,
                    y + size * 0.3f,
                    lw,
                    size * 0.4f);

                g.FillEllipse(ex,
                    x + size / 2 - lw / 2,
                    y + size * 0.8f,
                    lw,
                    lw);
            }
        }

        private void DrawPasswordIcon(Graphics g)
        {
            if (!isPassword)
                return;

            RectangleF bounds = GetPasswordIconBounds();
            string glyph = GetGlyph(isPasswordVisible ? InnerIconType.Lock : InnerIconType.Eye);

            using (Font font = new Font(IconFont, bounds.Height, GraphicsUnit.Pixel))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                SizeF s = g.MeasureString(glyph, font);
                float x = bounds.X + (bounds.Width - s.Width) / 2f;
                float y = bounds.Y + (bounds.Height - s.Height) / 2f;
                g.DrawString(glyph, font, brush, x, y);
            }
        }

        #endregion

        #region HELPERS

        private GraphicsPath GetRoundedRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Color InterpolateColor(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private RectangleF GetPasswordIconBounds()
        {
            float size = Height * 0.55f;
            float padding = 12f;
            return new RectangleF(Width - size - padding, (Height - size) / 2f, size, size);
        }

        private void UpdatePasswordMaskState()
        {
            textBox.UseSystemPasswordChar = isPassword && !isPasswordVisible;
        }

        private string GetGlyph(InnerIconType icon)
        {
            switch (icon)
            {
                case InnerIconType.Add: return "\uE710";
                case InnerIconType.Alarm: return "\uE823";
                case InnerIconType.Apps: return "\uE71D";
                case InnerIconType.Archive: return "\uF03F";
                case InnerIconType.ArrowDown: return "\uE74B";
                case InnerIconType.ArrowLeft: return "\uE76B";
                case InnerIconType.ArrowRight: return "\uE76C";
                case InnerIconType.ArrowUp: return "\uE74A";
                case InnerIconType.Attachment: return "\uE723";
                case InnerIconType.Back: return "\uE72B";
                case InnerIconType.Basket: return "\uE7BF";
                case InnerIconType.Bell: return "\uE7ED";
                case InnerIconType.BellOff: return "\uE7F6";
                case InnerIconType.Bluetooth: return "\uE702";
                case InnerIconType.Book: return "\uE82D";
                case InnerIconType.Bookmark: return "\uE8A4";
                case InnerIconType.Building: return "\uEA3D";
                case InnerIconType.Bug: return "\uEBE8";
                case InnerIconType.Calculator: return "\uE8EF";
                case InnerIconType.Calendar: return "\uE787";
                case InnerIconType.CalendarAdd: return "\uED0E";
                case InnerIconType.Camera: return "\uE722";
                case InnerIconType.Cancel: return "\uE711";
                case InnerIconType.Car: return "\uE804";
                case InnerIconType.Cart: return "\uE7BF";
                case InnerIconType.Chart: return "\uE9D2";
                case InnerIconType.ChartBar: return "\uE9D2";
                case InnerIconType.ChartLine: return "\uE9E9";
                case InnerIconType.Chat: return "\uE8BD";
                case InnerIconType.Check: return "\uE73E";
                case InnerIconType.Checklist: return "\uE9D5";
                case InnerIconType.ChevronDown: return "\uE70D";
                case InnerIconType.ChevronLeft: return "\uE76B";
                case InnerIconType.ChevronRight: return "\uE76C";
                case InnerIconType.ChevronUp: return "\uE70E";
                case InnerIconType.Clipboard: return "\uE8C8";
                case InnerIconType.Close: return "\uE711";
                case InnerIconType.Cloud: return "\uE753";
                case InnerIconType.CloudDownload: return "\uEBD3";
                case InnerIconType.CloudUpload: return "\uEBD4";
                case InnerIconType.Code: return "\uE943";
                case InnerIconType.Collapse: return "\uE73F";
                case InnerIconType.Comment: return "\uE90A";
                case InnerIconType.Compass: return "\uEBE6";
                case InnerIconType.Copy: return "\uE8C8";
                case InnerIconType.CreditCard: return "\uE8C7";
                case InnerIconType.Cut: return "\uE8C6";
                case InnerIconType.Dashboard: return "\uF246";
                case InnerIconType.Database: return "\uE9F1";
                case InnerIconType.Desktop: return "\uE770";
                case InnerIconType.DeviceLaptop: return "\uE7F8";
                case InnerIconType.DevicePhone: return "\uE8EA";
                case InnerIconType.Document: return "\uE8A5";
                case InnerIconType.DocumentSearch: return "\uE721";
                case InnerIconType.Download: return "\uE896";
                case InnerIconType.Drag: return "\uE7C1";
                case InnerIconType.Edit: return "\uE70F";
                case InnerIconType.Email: return "\uE715";
                case InnerIconType.Error: return "\uEA39";
                case InnerIconType.Excel: return "\uEC28";
                case InnerIconType.Expand: return "\uE740";
                case InnerIconType.Export: return "\uEDE1";
                case InnerIconType.Eye: return "\uE890";
                case InnerIconType.EyeHide: return "\uE8F4";
                case InnerIconType.File: return "\uE7C3";
                case InnerIconType.FileAdd: return "\uE8B7";
                case InnerIconType.FileRemove: return "\uE8B6";
                case InnerIconType.Filter: return "\uE71C";
                case InnerIconType.Flag: return "\uE7C1";
                case InnerIconType.Folder: return "\uE8B7";
                case InnerIconType.FolderOpen: return "\uE838";
                case InnerIconType.Forward: return "\uE72A";
                case InnerIconType.Fullscreen: return "\uE740";
                case InnerIconType.Gift: return "\uEC1F";
                case InnerIconType.Globe: return "\uE774";
                case InnerIconType.Grid: return "\uECA5";
                case InnerIconType.Heart: return "\uEB52";
                case InnerIconType.Help: return "\uEA09";
                case InnerIconType.History: return "\uE81C";
                case InnerIconType.Home: return "\uE80F";
                case InnerIconType.Image: return "\uEB9F";
                case InnerIconType.Import: return "\uE8B5";
                case InnerIconType.Inbox: return "\uE719";
                case InnerIconType.Info: return "\uE946";
                case InnerIconType.Key: return "\uE192";
                case InnerIconType.Link: return "\uE71B";
                case InnerIconType.List: return "\uEA37";
                case InnerIconType.Location: return "\uE707";
                case InnerIconType.Lock: return "\uE72E";
                case InnerIconType.MailOpen: return "\uE8A8";
                case InnerIconType.Menu: return "\uE700";
                case InnerIconType.Microphone: return "\uE720";
                case InnerIconType.Money: return "\uEAFD";
                case InnerIconType.Moon: return "\uE708";
                case InnerIconType.More: return "\uE712";
                case InnerIconType.Music: return "\uE8D6";
                case InnerIconType.Paste: return "\uE77F";
                case InnerIconType.Pause: return "\uE769";
                case InnerIconType.Pdf: return "\uEA90";
                case InnerIconType.People: return "\uE716";
                case InnerIconType.Phone: return "\uE717";
                case InnerIconType.Pin: return "\uE718";
                case InnerIconType.PinOff: return "\uE77A";
                case InnerIconType.Play: return "\uE768";
                case InnerIconType.Plus: return "\uE710";
                case InnerIconType.Print: return "\uE749";
                case InnerIconType.QrCode: return "\uED14";
                case InnerIconType.Receipt: return "\uEDDC";
                case InnerIconType.Redo: return "\uE7A6";
                case InnerIconType.Refresh: return "\uE72C";
                case InnerIconType.Remove: return "\uE738";
                case InnerIconType.Reply: return "\uE97A";
                case InnerIconType.Save: return "\uE74E";
                case InnerIconType.Scan: return "\uE8FE";
                case InnerIconType.Search: return "\uE721";
                case InnerIconType.Send: return "\uE724";
                case InnerIconType.Settings: return "\uE713";
                case InnerIconType.Share: return "\uE72D";
                case InnerIconType.Shield: return "\uE83D";
                case InnerIconType.ShieldCheck: return "\uE73E";
                case InnerIconType.ShieldError: return "\uEA39";
                case InnerIconType.ShoppingBag: return "\uE719";
                case InnerIconType.Star: return "\uE734";
                case InnerIconType.Stop: return "\uE71A";
                case InnerIconType.Sync: return "\uE895";
                case InnerIconType.Tag: return "\uE8EC";
                case InnerIconType.Terminal: return "\uF489";
                case InnerIconType.Time: return "\uE823";
                case InnerIconType.Trash: return "\uE74D";
                case InnerIconType.Truck: return "\uEC4A";
                case InnerIconType.Undo: return "\uE7A7";
                case InnerIconType.Unlock: return "\uE785";
                case InnerIconType.Unlink: return "\uF127";
                case InnerIconType.Upload: return "\uE898";
                case InnerIconType.User: return "\uE77B";
                case InnerIconType.UserAdd: return "\uE8FA";
                case InnerIconType.UserRemove: return "\uE8FB";
                case InnerIconType.Video: return "\uE714";
                case InnerIconType.VolumeHigh: return "\uE995";
                case InnerIconType.VolumeLow: return "\uE993";
                case InnerIconType.VolumeMute: return "\uE74F";
                case InnerIconType.Wallet: return "\uE8C9";
                case InnerIconType.Warning: return "\uE7BA";
                case InnerIconType.Wifi: return "\uE701";
                case InnerIconType.Window: return "\uE8A7";
                case InnerIconType.Wrench: return "\uE90F";
                case InnerIconType.ZoomIn: return "\uE8A3";
                case InnerIconType.ZoomOut: return "\uE71F";

                default: return string.Empty;
            }
        }
        public void ShowError()
        {
            forcedError = true;
            isValid = false;
            showErrorVisual = true;

            toolTip.Show(
                errorMessage,
                this,
                Width / 2,
                Height,
                timeToolTipText * 1000);

            errorTimer.Interval = timeToolTipText * 1000;
            errorTimer.Start();

            animationTimer.Start();
            AdjustLayout();
            Invalidate();
        }

        public void ClearError()
        {
            forcedError = false;
            isValid = true;
            showErrorVisual = false;

            toolTip.Hide(this);

            animationTimer.Start();
            AdjustLayout();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!isPassword)
                return;

            if (GetPasswordIconBounds().Contains(e.Location))
            {
                isPasswordVisible = !isPasswordVisible;
                UpdatePasswordMaskState();
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isPassword && GetPasswordIconBounds().Contains(e.Location))
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.IBeam;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }

        #endregion
    }

}
