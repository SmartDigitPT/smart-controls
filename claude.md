# SmartControls Project

## Project Overview

**SmartControls** is a Windows Forms control library that provides custom, modern UI components with enhanced styling, validation, and interactivity features. It extends the standard .NET Framework controls with rounded corners, smooth animations, advanced validation, and comprehensive icon support using Segoe MDL2 Assets.

- **Language**: C#
- **Target Framework**: .NET Framework 4.7.2
- **Output Type**: Class Library (DLL)
- **Default Namespace**: SmartControls / RoudedButton
- **Main Use Case**: Providing polished, modern Windows Forms controls for desktop applications

---

## File and Folder Structure

```
SmartControls/
├── Components/
│   ├── RoudedButton.cs           # Custom rounded button with image and hover effects
│   ├── RoudedButton.resx         # Designer resources for RoundedButton
│   ├── RoundedPanelControl.cs    # Custom rounded panel with headers and collapse
│   └── RoundedPanelControl.resx  # Designer resources for RoundedPanel
├── UserControls/
│   ├── SmartTextBox.cs           # Advanced textbox with validation and icons
│   └── SmartTextBox.resx         # Designer resources for SmartTextBox
├── InnerIconEnumEditor.cs        # Icon picker designer and enum editor
├── Properties/
│   └── AssemblyInfo.cs           # Assembly metadata and version info
├── SmartControls.csproj          # MSBuild project file
├── SmartControls.slnx            # Solution file
└── .gitignore                    # Git ignore rules
```

---

## Main Modules & Classes

### 1. **RoundedButton** (`Components/RoudedButton.cs`)
A custom Button component with rounded corners and advanced rendering capabilities.

#### Key Features:
- **Rounded Corners**: Configurable `BorderRadius` property
- **Border Styling**: Adjustable `BorderSize` and `BorderColor`
- **State Effects**: `HoverBackColor` and `PressedBackColor` for visual feedback
- **Image Support**: Integrated image rendering with sizing modes:
  - `Normal` - Display at original size
  - `Stretch` - Stretch image to fit target size
  - `Zoom` - Scale proportionally to fit target size
- **Centered Layout**: Text and image are centered and spaced automatically

#### Main Properties:
- `BorderRadius` (int, default: 20) - Corner radius in pixels
- `BorderSize` (int, default: 1) - Border thickness
- `BorderColor` (Color) - Border color
- `HoverBackColor` (Color) - Background color on hover
- `PressedBackColor` (Color) - Background color when pressed
- `ImageSizeMode` (ButtonImageSizeMode enum) - How to resize the button image
- `ImageTargetSize` (Size, default: 24x24) - Target size for button images

#### Key Methods:
- `OnPaint(PaintEventArgs)` - Renders button with high-quality graphics
- `DrawImageAndText(Graphics, Rectangle)` - Draws centered image and text
- `GetRoundedPath(RectangleF, float)` - Generates smooth Bézier curve rounded rectangle
- `UpdateRegion()` - Updates control region for smooth rounded appearance

---

### 2. **RoundedPanel** (`Components/RoundedPanelControl.cs`)
A custom Panel component with rounded corners, optional header bar, and collapse/expand functionality.

#### Key Features:
- **Rounded Corners**: Independent radius for each corner (`RadiusTopLeft`, `RadiusTopRight`, etc.)
- **Header Support**: Optional header bar with gradient background
  - Header text alignment and font customization
  - Header gradient colors for professional appearance
  - Optional header separator line
  - Collapse/expand icon (Segoe MDL2 Assets)
- **Inner Icons**: Header can display category icons from Segoe MDL2 Assets
- **Collapse/Expand**: Click header to toggle collapsed state; height animates
- **Border Styling**: Customizable border thickness and color

#### Main Properties:
- `RadiusTopLeft/Right/BottomLeft/Right` (int) - Individual corner radius values
- `BorderRadius` (int) - Sets all corners to same radius
- `BorderThickness` (int) - Border thickness in pixels
- `BorderColor` (Color) - Border color
- `ShowHeader` (bool) - Toggle header visibility
- `HeaderHeight` (int, default: 36) - Header height in pixels
- `HeaderColor` (Color) - Header background color
- `HeaderGradientStartColor/EndColor` (Color) - Gradient colors
- `TextHeader` (string) - Header text content
- `TextHeaderFont` (Font) - Header text font
- `TextHeaderColor` (Color) - Header text color
- `TextHeaderAlign` (ContentAlignment) - Header text alignment
- `InnerIcon` (InnerIconType enum) - Icon displayed in header
- `InnerIconColor` (Color) - Icon color
- `InnerIconSize` (int) - Icon size in pixels
- `IconFontFamily` (string, default: "Segoe Fluent Icons") - Font for rendering icons
- `IsCollapsed` (bool) - Current collapse state
- `CollapseOnHeaderClick` (bool) - Enable collapse on header click
- `ShowCollapseIcon` (bool) - Show expand/collapse indicator

#### Key Methods:
- `OnPaint(PaintEventArgs)` - Renders panel with header and rounded borders
- `GetRoundedPath(Rectangle)` - Generates rounded rectangle path
- `GetRoundedHeaderPath(Rectangle)` - Generates rounded header path
- `DrawCollapseIcon(Graphics)` - Renders collapse/expand icon
- `ApplyCollapseState()` - Handles collapse animation and height adjustment
- `GetGlyph(InnerIconType)` - Maps icon enum to Unicode glyph

#### InnerIconType Enum (Partial):
Communication, User/Security, Navigation, Files/Data, Status, Commerce, Media, and Miscellaneous icons. Examples: Email, Phone, Settings, Menu, Home, Search, File, Save, Lock, Shield, Chart, etc. (70+ icon types available)

---

### 3. **SmartTextBox** (`UserControls/SmartTextBox.cs`)
An advanced UserControl that extends TextBox with validation, error visualization, password masking, and icon support.

#### Key Features:
- **Real-time Validation**: Multiple validation modes with live error feedback
- **Error Visualization**: 
  - Visual error indicator (triangle with exclamation mark)
  - Animated error fade in/out
  - ToolTip with custom error messages
- **Password Support**: Built-in password masking with eye icon toggle
- **Icon Support**: Configurable left or right inner icons
- **Border Animation**: Smooth focus state border color transition
- **Customizable Styling**: Border radius, thickness, colors for normal/focus/error states
- **Auto-validation**: Optional automatic validation on text change or manual on Enter/Leave

#### Validation Modes:
- `None` - No validation
- `Required` - Text must not be empty/whitespace
- `Email` - Validates email format
- `NIF` - Portuguese National ID (9 digits)
- `NIB` - Portuguese Bank Account (21 digits)
- `Telephone` - Portuguese phone number (9XXXXXXXX)
- `URL` - URL format validation
- `Number` - Numeric values only
- `TextWithoutSpace` - No spaces allowed
- `CustomRegex` - Custom regex pattern validation

#### Main Properties:
- `Text` (string) - TextBox content
- `Font` (Font) - Text font
- `ForeColor` (Color) - Text color
- `BackColor` (Color) - Background color
- `BorderRadius` (int, default: 10) - Corner radius
- `BorderThickness` (int, default: 1) - Border thickness
- `BorderColor` (Color) - Normal border color
- `BorderFocusColor` (Color, default: RGB(0,120,215)) - Focused border color
- `BorderErrorColor` (Color, default: Red) - Error state border color
- `ShowBorderFocus` (bool, default: true) - Animate border on focus
- `InnerIcon` (InnerIconType enum) - Icon to display
- `InnerIconPosition` (InnerIconPosition enum) - Left or Right positioning
- `ValidationMode` (ValidationMode enum) - Selected validation mode
- `AutoValidationMode` (bool, default: true) - Validate on text change vs. on leave
- `CustomRegexPattern` (string) - Regex for custom validation
- `ValidationMessage` (string) - Error message for auto-validation
- `ErrorMessage` (string) - Error message for manual validation
- `FocusMessage` (string) - ToolTip shown on focus
- `TimeToolTipText` (int, default: 5) - ToolTip display time in seconds
- `IsPassword` (bool, default: false) - Enable password masking
- `ShowErrorIcon` (bool, default: true) - Show error indicator icon
- `IsValid` (bool, read-only) - Current validation state

#### Key Methods:
- `FocusTextBox()` - Set focus to internal TextBox
- `ValidateText()` - Perform validation based on current mode
- `ShowError()` - Manually show error state
- `ClearError()` - Clear error state and hide error visualization
- `OnPaint(PaintEventArgs)` - Renders border, icons, and error indicator
- `DrawInnerIcon(Graphics)` - Renders left/right icon
- `DrawErrorIcon(Graphics)` - Renders error triangle with fade animation
- `DrawPasswordIcon(Graphics)` - Renders eye icon for password visibility toggle
- `GetPattern()` - Returns regex pattern for current validation mode
- `Animate(object, EventArgs)` - Updates focus and error fade animations

#### InnerIconType Enum (Partial):
160+ icons including: Add, Alarm, Apps, Archive, Arrows (4 directions), Bell, Buildings, Camera, Calendar, Chart, Chat, Check, Close, Copy, Database, Download, Edit, Email, Error, Eye, File, Filter, Flag, Folder, Gift, Globe, Grid, Heart, History, Home, Image, Info, Key, Link, List, Location, Lock, Menu, Microphone, Money, More, Music, Paste, People, Phone, Play, Refresh, Save, Search, Send, Settings, Shield, Shopping, Star, Settings, Time, Trash, User, Upload, Wallet, Warning, Wifi, and more.

---

### 4. **InnerIconEnumEditor** (`InnerIconEnumEditor.cs`)
A custom designer and property editor for visual icon selection in the Visual Studio designer.

#### Key Classes:

##### `InnerIconEnumEditor`
- **Purpose**: UITypeEditor for selecting icons from enum types at design time
- **Features**:
  - Modal dialog picker with category filtering
  - Live icon preview in property grid
  - Icon rendering using Segoe MDL2 Assets font
  - Fallback to "Segoe Fluent Icons" if primary font unavailable
  - 160+ supported icons organized in 8 categories

##### `InnerIconPickerDialog`
- **Purpose**: Modal dialog for selecting icons
- **Features**:
  - Category dropdown filter (Communication, Users & Security, Navigation, Files & Data, Status, Commerce, Media & Scan, Misc)
  - ListBox with owner-drawn items showing icon preview + name
  - Double-click or Enter to confirm selection
  - Current selection memory
  - Fixed dialog size (360x470 pixels)

##### `InnerIconPickerObject` and `InnerIconPickerValue`
- Support classes for property descriptor integration with the designer

#### Main Methods:
- `EditValue(ITypeDescriptorContext, IServiceProvider, object)` - Opens picker dialog
- `TryPickEnumValue(Type, object, out object)` - Static method for programmatic icon selection
- `PaintValue(PaintValueEventArgs)` - Renders icon preview in property grid
- `GetGlyphByName(string)` - Maps icon name to Unicode character
- `GetCategoryByName(string)` - Categorizes icons for filtering
- `DrawItem(ListBox, DrawItemEventArgs)` - Renders list items with icon and name

#### Supported Icons (160+):
Organized in 8 categories:
- **Communication**: Email, Phone, Chat, Send, Inbox, MailOpen
- **Users & Security**: User, UserAdd, UserRemove, People, Lock, Unlock, Key, Shield, ShieldCheck, ShieldError
- **Navigation**: Search, Filter, Settings, Menu, More, Home, Back, Forward, Refresh, Help, History, ZoomIn, ZoomOut
- **Files & Data**: File, FileAdd, FileRemove, Document, Folder, FolderOpen, Save, Upload, Download, Cloud, Database, etc.
- **Status**: Check, Close, Cancel, Warning, Info, Error, Bell, Sync, Time
- **Commerce**: Cart, CreditCard, Money, Wallet, Tag, Chart, Calculator
- **Media & Scan**: Play, Pause, Stop, Camera, Image, Eye, Scan, QrCode
- **Misc**: Heart, Location, Link, Globe, Clipboard, Edit, Trash, and 50+ more

---

## Dependencies & Libraries

### System Libraries
- **System** - Core framework types
- **System.Core** - LINQ and extension methods
- **System.Drawing** - Graphics and drawing primitives (brushes, pens, paths, fonts)
- **System.Drawing.Drawing2D** - Advanced graphics (GraphicsPath, gradient brushes, smoothing modes)
- **System.Windows.Forms** - WinForms controls and UI components
- **System.Xml.Linq** - XML support (if needed)
- **System.Data & System.Data.DataSetExtensions** - Data access
- **System.Net.Http** - HTTP client support
- **Microsoft.CSharp** - Dynamic C# runtime support

### Key Framework Features Used
- **Graphics & Rendering**: Anti-aliased graphics, Bézier curves, gradient fills, clipping regions
- **Windows Forms**: Custom control painting, event handling, property descriptors
- **UI Type Editors**: Designer integration for custom property editors
- **Timer**: Animation and smooth transitions
- **Regex**: Text validation (System.Text.RegularExpressions)

---

## Usage Instructions

### Building the Project

```bash
# Build via Visual Studio
msbuild SmartControls.csproj /p:Configuration=Release

# Or use Visual Studio Build menu
# Output: bin/Release/RoudedButton.dll
```

### Adding Controls to a Windows Forms Application

1. **Add Reference**: Add `SmartControls.dll` (or project reference) to your Windows Forms application
2. **Use in Designer**: Drag controls from toolbox
3. **Use in Code**:

```csharp
using SmartControls.Components;
using SmartControls.UserControls;

// RoundedButton example
var button = new RoundedButton
{
    Text = "Click Me",
    BorderRadius = 20,
    BorderColor = Color.RoyalBlue,
    HoverBackColor = Color.LightBlue,
    Location = new Point(10, 10),
    Size = new Size(120, 40)
};
this.Controls.Add(button);

// SmartTextBox with email validation
var textBox = new SmartTextBox
{
    ValidationMode = ValidationMode.Email,
    ValidationMessage = "Please enter a valid email",
    BorderRadius = 8,
    Location = new Point(10, 60),
    Size = new Size(300, 40)
};
this.Controls.Add(textBox);

// Check validation
if (textBox.IsValid)
{
    MessageBox.Show("Valid email: " + textBox.Text);
}

// RoundedPanel with collapsible header
var panel = new RoundedPanel
{
    TextHeader = "Settings",
    ShowHeader = true,
    HeaderHeight = 36,
    InnerIcon = InnerIconType.Settings,
    CollapseOnHeaderClick = true,
    Location = new Point(10, 110),
    Size = new Size(400, 300)
};
this.Controls.Add(panel);
```

### Designer Integration

1. **Icon Picker**: In SmartTextBox or RoundedPanel properties, set `InnerIcon` property - a visual icon picker dialog opens
2. **Property Categorization**: Properties organized by category (Appearance, Header, Validation, Behavior)
3. **Real-time Preview**: Border radius, colors, and text changes update in designer preview

### Key Design Patterns

- **Custom Painting**: All controls override `OnPaint()` for high-quality rendering
- **Graphics Optimization**: Using `SmoothingMode.AntiAlias` and `PixelOffsetMode.HighQuality`
- **Animation**: Timer-based smooth transitions for focus and error states
- **Validation**: Regex-based pattern matching with customizable error messages
- **Icon Support**: Unicode character mapping to Segoe MDL2 Assets font

---

## Notes

- **Icon Font**: Requires "Segoe MDL2 Assets" or "Segoe Fluent Icons" installed on system
- **Framework Target**: .NET Framework 4.7.2 - not compatible with .NET Core/5+ without modification
- **High DPI**: Uses standard Windows Forms scaling; may need manual adjustment for very high DPI displays
- **Performance**: Double buffering enabled on all controls for smooth rendering
- **Designer Support**: Full Visual Studio designer support with custom property editors

---

## Build Configuration

- **Platform**: AnyCPU
- **Debug**: Symbols enabled, no optimization
- **Release**: PDB only symbols, optimized build
- **File Alignment**: 512 bytes
- **Deterministic Build**: Enabled for reproducible builds
