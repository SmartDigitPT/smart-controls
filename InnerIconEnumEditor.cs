using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SmartControls
{
    internal interface IInnerIconPickerAdapter
    {
        Type InnerIconEnumType { get; }
        object GetInnerIconValue();
        void SetInnerIconValue(object value);
    }

    [Editor(typeof(InnerIconEnumEditor), typeof(UITypeEditor))]
    public sealed class InnerIconPickerValue
    {
        public string Name { get; }

        public InnerIconPickerValue(string name)
        {
            Name = name ?? string.Empty;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class InnerIconPickerObject : IInnerIconPickerAdapter
    {
        private readonly object _owner;

        public InnerIconPickerObject(object owner)
        {
            _owner = owner;
        }

        [Browsable(false)]
        public Type InnerIconEnumType
        {
            get
            {
                var prop = _owner?.GetType().GetProperty("InnerIcon");
                return prop?.PropertyType;
            }
        }

        [Category("InnerIcon")]
        [DisplayName("Value")]
        [NotifyParentProperty(true)]
        [ReadOnly(true)]
        [Editor(typeof(InnerIconEnumEditor), typeof(UITypeEditor))]
        public InnerIconPickerValue Value
        {
            get
            {
                object current = GetInnerIconValue();
                return new InnerIconPickerValue(current?.ToString() ?? "None");
            }
            set
            {
                if (value == null)
                    return;

                Type enumType = InnerIconEnumType;
                if (enumType == null || !enumType.IsEnum)
                    return;

                try
                {
                    object parsed = Enum.Parse(enumType, value.ToString());
                    SetInnerIconValue(parsed);
                }
                catch
                {
                    // ignore invalid design-time values
                }
            }
        }

        public object GetInnerIconValue()
        {
            var prop = _owner?.GetType().GetProperty("InnerIcon");
            return prop?.GetValue(_owner, null);
        }

        public void SetInnerIconValue(object value)
        {
            var prop = _owner?.GetType().GetProperty("InnerIcon");
            if (prop == null || !prop.CanWrite)
                return;

            prop.SetValue(_owner, value, null);
        }

        public override string ToString()
        {
            object current = GetInnerIconValue();
            return current?.ToString() ?? "None";
        }
    }

    public class InnerIconEnumEditor : UITypeEditor
    {
        private const string PreferredIconFont = "Segoe MDL2 Assets";
        private const string FallbackIconFont = "Segoe Fluent Icons";

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || value == null)
                return value;

            Type enumType = GetEnumType(context);
            object currentValue = GetCurrentEnumValue(context, value);

            if (enumType == null || !enumType.IsEnum || currentValue == null)
                return value;

            if (TryPickEnumValue(enumType, currentValue, out object pickedValue))
            {
                if (context.PropertyDescriptor != null &&
                    (context.PropertyDescriptor.PropertyType == typeof(string) ||
                     context.PropertyDescriptor.PropertyType == typeof(InnerIconPickerValue)))
                {
                    SetInnerIconOnInstance(context, pickedValue);
                    if (context.PropertyDescriptor.PropertyType == typeof(InnerIconPickerValue))
                        return new InnerIconPickerValue(pickedValue.ToString());

                    return pickedValue.ToString();
                }

                return pickedValue;
            }

            return value;
        }

        public static bool TryPickEnumValue(Type enumType, object currentValue, out object selectedValue)
        {
            selectedValue = currentValue;

            if (enumType == null || !enumType.IsEnum || currentValue == null)
                return false;

            using (var picker = new InnerIconPickerDialog(enumType, currentValue, new InnerIconEnumEditor()))
            {
                if (picker.ShowDialog() == DialogResult.OK && picker.SelectedValue != null)
                {
                    selectedValue = picker.SelectedValue;
                    return true;
                }
            }

            return false;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e == null || e.Value == null)
                return;

            string name = GetPaintValueName(e);
            string glyph = GetGlyphByName(name);

            e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

            Rectangle iconRect = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            if (!string.IsNullOrEmpty(glyph))
            {
                using (Font iconFont = CreateIconFont(Math.Max(10, e.Bounds.Height - 4)))
                {
                    TextRenderer.DrawText(
                        e.Graphics,
                        glyph,
                        iconFont,
                        iconRect,
                        SystemColors.ControlText,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                }
            }
        }

        private void DrawItem(ListBox listBox, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0 || e.Index >= listBox.Items.Count)
                return;

            object item = listBox.Items[e.Index];
            string name = item.ToString();
            string glyph = GetGlyphByName(name);

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color textColor = selected ? SystemColors.HighlightText : e.ForeColor;
            Rectangle bounds = e.Bounds;

            Rectangle iconRect = new Rectangle(bounds.X + 8, bounds.Y + 2, 24, bounds.Height - 4);
            Rectangle textRect = new Rectangle(bounds.X + 36, bounds.Y, bounds.Width - 40, bounds.Height);

            if (!string.IsNullOrEmpty(glyph))
            {
                using (Font iconFont = CreateIconFont(14))
                {
                    TextRenderer.DrawText(
                        e.Graphics,
                        glyph,
                        iconFont,
                        iconRect,
                        textColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                }
            }

            TextRenderer.DrawText(
                e.Graphics,
                name,
                listBox.Font,
                textRect,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

            e.DrawFocusRectangle();
        }

        private Font CreateIconFont(float size)
        {
            try
            {
                return new Font(PreferredIconFont, size, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            catch
            {
                return new Font(FallbackIconFont, size, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

        private string GetGlyphByName(string iconName)
        {
            switch (iconName)
            {
                case "Add": return "\uE710";
                case "Alarm": return "\uE823";
                case "Apps": return "\uE71D";
                case "Archive": return "\uF03F";
                case "ArrowDown": return "\uE74B";
                case "ArrowLeft": return "\uE76B";
                case "ArrowRight": return "\uE76C";
                case "ArrowUp": return "\uE74A";
                case "Attachment": return "\uE723";
                case "Back": return "\uE72B";
                case "Basket": return "\uE7BF";
                case "Bell": return "\uE7ED";
                case "BellOff": return "\uE7F6";
                case "Bluetooth": return "\uE702";
                case "Book": return "\uE82D";
                case "Bookmark": return "\uE8A4";
                case "Building": return "\uEA3D";
                case "Bug": return "\uEBE8";
                case "Calculator": return "\uE8EF";
                case "Calendar": return "\uE787";
                case "CalendarAdd": return "\uED0E";
                case "Camera": return "\uE722";
                case "Cancel": return "\uE711";
                case "Car": return "\uE804";
                case "Cart": return "\uE7BF";
                case "Chart": return "\uE9D2";
                case "ChartBar": return "\uE9D2";
                case "ChartLine": return "\uE9E9";
                case "Chat": return "\uE8BD";
                case "Check": return "\uE73E";
                case "Checklist": return "\uE9D5";
                case "ChevronDown": return "\uE70D";
                case "ChevronLeft": return "\uE76B";
                case "ChevronRight": return "\uE76C";
                case "ChevronUp": return "\uE70E";
                case "Clipboard": return "\uE8C8";
                case "Close": return "\uE711";
                case "Cloud": return "\uE753";
                case "CloudDownload": return "\uEBD3";
                case "CloudUpload": return "\uEBD4";
                case "Code": return "\uE943";
                case "Collapse": return "\uE73F";
                case "Comment": return "\uE90A";
                case "Compass": return "\uEBE6";
                case "Copy": return "\uE8C8";
                case "CreditCard": return "\uE8C7";
                case "Cut": return "\uE8C6";
                case "Dashboard": return "\uF246";
                case "Database": return "\uE9F1";
                case "Desktop": return "\uE770";
                case "DeviceLaptop": return "\uE7F8";
                case "DevicePhone": return "\uE8EA";
                case "Document": return "\uE8A5";
                case "DocumentSearch": return "\uE721";
                case "Download": return "\uE896";
                case "Drag": return "\uE7C1";
                case "Edit": return "\uE70F";
                case "Email": return "\uE715";
                case "Error": return "\uEA39";
                case "Excel": return "\uEC28";
                case "Expand": return "\uE740";
                case "Export": return "\uEDE1";
                case "Eye": return "\uE890";
                case "EyeHide": return "\uE8F4";
                case "File": return "\uE7C3";
                case "FileAdd": return "\uE8B7";
                case "FileRemove": return "\uE8B6";
                case "Filter": return "\uE71C";
                case "Flag": return "\uE7C1";
                case "Folder": return "\uE8B7";
                case "FolderOpen": return "\uE838";
                case "Forward": return "\uE72A";
                case "Fullscreen": return "\uE740";
                case "Gift": return "\uEC1F";
                case "Globe": return "\uE774";
                case "Grid": return "\uECA5";
                case "Heart": return "\uEB52";
                case "Help": return "\uEA09";
                case "History": return "\uE81C";
                case "Home": return "\uE80F";
                case "Image": return "\uEB9F";
                case "Import": return "\uE8B5";
                case "Inbox": return "\uE719";
                case "Info": return "\uE946";
                case "Key": return "\uE192";
                case "Link": return "\uE71B";
                case "List": return "\uEA37";
                case "Location": return "\uE707";
                case "Lock": return "\uE72E";
                case "MailOpen": return "\uE8A8";
                case "Menu": return "\uE700";
                case "Microphone": return "\uE720";
                case "Money": return "\uEAFD";
                case "Moon": return "\uE708";
                case "More": return "\uE712";
                case "Music": return "\uE8D6";
                case "Paste": return "\uE77F";
                case "Pause": return "\uE769";
                case "Pdf": return "\uEA90";
                case "People": return "\uE716";
                case "Phone": return "\uE717";
                case "Pin": return "\uE718";
                case "PinOff": return "\uE77A";
                case "Play": return "\uE768";
                case "Plus": return "\uE710";
                case "Print": return "\uE749";
                case "QrCode": return "\uED14";
                case "Receipt": return "\uEDDC";
                case "Redo": return "\uE7A6";
                case "Refresh": return "\uE72C";
                case "Remove": return "\uE738";
                case "Reply": return "\uE97A";
                case "Save": return "\uE74E";
                case "Scan": return "\uE8FE";
                case "Search": return "\uE721";
                case "Send": return "\uE724";
                case "Settings": return "\uE713";
                case "Share": return "\uE72D";
                case "Shield": return "\uE83D";
                case "ShieldCheck": return "\uE73E";
                case "ShieldError": return "\uEA39";
                case "ShoppingBag": return "\uE719";
                case "Star": return "\uE734";
                case "Stop": return "\uE71A";
                case "Sync": return "\uE895";
                case "Tag": return "\uE8EC";
                case "Terminal": return "\uF489";
                case "Time": return "\uE823";
                case "Trash": return "\uE74D";
                case "Truck": return "\uEC4A";
                case "Undo": return "\uE7A7";
                case "Unlock": return "\uE785";
                case "Unlink": return "\uF127";
                case "Upload": return "\uE898";
                case "User": return "\uE77B";
                case "UserAdd": return "\uE8FA";
                case "UserRemove": return "\uE8FB";
                case "Video": return "\uE714";
                case "VolumeHigh": return "\uE995";
                case "VolumeLow": return "\uE993";
                case "VolumeMute": return "\uE74F";
                case "Wallet": return "\uE8C9";
                case "Warning": return "\uE7BA";
                case "Wifi": return "\uE701";
                case "Window": return "\uE8A7";
                case "Wrench": return "\uE90F";
                case "ZoomIn": return "\uE8A3";
                case "ZoomOut": return "\uE71F";
                default: return string.Empty;
            }
        }

        private string GetCategoryByName(string iconName)
        {
            switch (iconName)
            {
                case "Email":
                case "Inbox":
                case "MailOpen":
                case "Phone":
                case "Send":
                case "Chat":
                case "Attachment":
                    return "Communication";

                case "User":
                case "UserAdd":
                case "UserRemove":
                case "People":
                case "Lock":
                case "Unlock":
                case "Key":
                case "Shield":
                case "ShieldCheck":
                case "ShieldError":
                    return "Users & Security";

                case "Search":
                case "Filter":
                case "Settings":
                case "Menu":
                case "More":
                case "Home":
                case "Back":
                case "Forward":
                case "Refresh":
                case "Link":
                case "Help":
                case "History":
                case "ZoomIn":
                case "ZoomOut":
                    return "Navigation";

                case "File":
                case "FileAdd":
                case "FileRemove":
                case "Document":
                case "DocumentSearch":
                case "Folder":
                case "FolderOpen":
                case "Save":
                case "Upload":
                case "Download":
                case "Cloud":
                case "CloudUpload":
                case "CloudDownload":
                case "Database":
                case "Copy":
                case "Paste":
                case "Clipboard":
                case "Print":
                case "Pdf":
                case "Excel":
                    return "Files & Data";

                case "Check":
                case "Close":
                case "Cancel":
                case "Warning":
                case "Info":
                case "Error":
                case "Bell":
                case "BellOff":
                case "Sync":
                case "Time":
                    return "Status";

                case "Cart":
                case "CreditCard":
                case "Money":
                case "Wallet":
                case "Tag":
                case "Chart":
                case "ChartBar":
                case "ChartLine":
                case "Calculator":
                    return "Commerce";

                case "Play":
                case "Pause":
                case "Stop":
                case "Camera":
                case "Image":
                case "Eye":
                case "EyeHide":
                case "Scan":
                case "QrCode":
                    return "Media & Scan";

                default:
                    return "Misc";
            }
        }

        private Type GetEnumType(ITypeDescriptorContext context)
        {
            if (context.Instance is IInnerIconPickerAdapter adapter && adapter.InnerIconEnumType != null)
                return adapter.InnerIconEnumType;

            Type propertyType = context.PropertyDescriptor?.PropertyType;
            if (propertyType != null && propertyType.IsEnum)
                return propertyType;

            var innerIconProp = context.Instance?.GetType().GetProperty("InnerIcon");
            if (innerIconProp != null && innerIconProp.PropertyType.IsEnum)
                return innerIconProp.PropertyType;

            return null;
        }

        private object GetCurrentEnumValue(ITypeDescriptorContext context, object fallbackValue)
        {
            if (context.Instance is IInnerIconPickerAdapter adapter)
            {
                object adaptedValue = adapter.GetInnerIconValue();
                if (adaptedValue != null)
                    return adaptedValue;
            }

            if (fallbackValue != null && fallbackValue.GetType().IsEnum)
                return fallbackValue;

            var innerIconProp = context.Instance?.GetType().GetProperty("InnerIcon");
            if (innerIconProp == null)
                return null;

            try
            {
                return innerIconProp.GetValue(context.Instance, null);
            }
            catch
            {
                return null;
            }
        }

        private void SetInnerIconOnInstance(ITypeDescriptorContext context, object enumValue)
        {
            if (context.Instance is IInnerIconPickerAdapter adapter)
            {
                adapter.SetInnerIconValue(enumValue);
                return;
            }

            var innerIconProp = context.Instance?.GetType().GetProperty("InnerIcon");
            if (innerIconProp == null || !innerIconProp.CanWrite)
                return;

            try
            {
                innerIconProp.SetValue(context.Instance, enumValue, null);
            }
            catch
            {
                // ignore designer reflection failures
            }
        }

        private string GetPaintValueName(PaintValueEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.GetType().IsEnum)
                    return e.Value.ToString();

                string s = e.Value as string;
                if (!string.IsNullOrWhiteSpace(s))
                    return s;
            }

            var innerIconProp = e.Context?.Instance?.GetType().GetProperty("InnerIcon");
            if (e.Context?.Instance is IInnerIconPickerAdapter adapter)
            {
                try
                {
                    object v = adapter.GetInnerIconValue();
                    return v?.ToString() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }

            if (innerIconProp != null)
            {
                try
                {
                    object v = innerIconProp.GetValue(e.Context.Instance, null);
                    return v?.ToString() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        private sealed class InnerIconPickerDialog : Form
        {
            private const string AllCategories = "Todos";
            private readonly ListBox _listBox;
            private readonly ComboBox _categoryCombo;
            private readonly Button _okButton;
            private readonly Button _cancelButton;
            private readonly InnerIconEnumEditor _owner;
            private readonly List<object> _allItems = new List<object>();

            public object SelectedValue => _listBox.SelectedItem;

            public InnerIconPickerDialog(Type enumType, object currentValue, InnerIconEnumEditor owner)
            {
                _owner = owner;

                Text = "Selecionar ícone";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                StartPosition = FormStartPosition.CenterParent;
                MinimizeBox = false;
                MaximizeBox = false;
                ShowInTaskbar = false;
                Width = 360;
                Height = 470;
                TopMost = true;

                _categoryCombo = new ComboBox
                {
                    Dock = DockStyle.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Height = 28
                };

                _listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    DrawMode = DrawMode.OwnerDrawFixed,
                    IntegralHeight = false,
                    ItemHeight = 22
                };

                foreach (object enumValue in Enum.GetValues(enumType))
                    _allItems.Add(enumValue);

                var categories = _allItems
                    .Select(i => _owner.GetCategoryByName(i.ToString()))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                _categoryCombo.Items.Add(AllCategories);
                foreach (string category in categories)
                    _categoryCombo.Items.Add(category);

                _categoryCombo.SelectedIndexChanged += (s, e) => ApplyCategoryFilter(currentValue);
                _categoryCombo.SelectedIndex = 0;

                _listBox.DrawItem += (s, e) => _owner.DrawItem(_listBox, e);
                _listBox.DoubleClick += (s, e) => ConfirmSelection();
                _listBox.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        ConfirmSelection();
                        e.Handled = true;
                    }
                };

                _okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Width = 88
                };

                _cancelButton = new Button
                {
                    Text = "Cancelar",
                    DialogResult = DialogResult.Cancel,
                    Width = 88
                };

                var buttonsPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 42,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(8),
                    WrapContents = false
                };

                var topPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 40,
                    Padding = new Padding(8, 8, 8, 4)
                };

                var categoryLabel = new Label
                {
                    Text = "Categoria:",
                    Dock = DockStyle.Left,
                    Width = 70,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                _categoryCombo.Dock = DockStyle.Fill;
                topPanel.Controls.Add(_categoryCombo);
                topPanel.Controls.Add(categoryLabel);

                buttonsPanel.Controls.Add(_cancelButton);
                buttonsPanel.Controls.Add(_okButton);

                Controls.Add(_listBox);
                Controls.Add(topPanel);
                Controls.Add(buttonsPanel);

                AcceptButton = _okButton;
                CancelButton = _cancelButton;

                Shown += (s, e) =>
                {
                    Activate();
                    TopMost = false;
                };
            }

            private void ApplyCategoryFilter(object preferredSelection)
            {
                object currentSelection = _listBox.SelectedItem ?? preferredSelection;
                string selectedCategory = _categoryCombo.SelectedItem as string ?? AllCategories;

                _listBox.BeginUpdate();
                _listBox.Items.Clear();

                foreach (object item in _allItems)
                {
                    string category = _owner.GetCategoryByName(item.ToString());
                    if (selectedCategory == AllCategories || string.Equals(category, selectedCategory, StringComparison.Ordinal))
                        _listBox.Items.Add(item);
                }

                if (currentSelection != null)
                {
                    foreach (object item in _listBox.Items)
                    {
                        if (Equals(item, currentSelection))
                        {
                            _listBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (_listBox.SelectedIndex < 0 && _listBox.Items.Count > 0)
                    _listBox.SelectedIndex = 0;

                _listBox.EndUpdate();
            }

            private void ConfirmSelection()
            {
                if (_listBox.SelectedItem == null)
                    return;

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
