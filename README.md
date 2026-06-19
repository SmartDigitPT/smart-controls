# SmartControls

A modern Windows Forms control library providing enhanced, polished UI components with rounded corners, smooth animations, built-in validation, and comprehensive icon support.

## Features

✨ **Custom Controls**
- **RoundedButton** - Button with rounded corners, hover effects, and integrated images
- **RoundedPanel** - Panel with rounded corners, optional header bar, and collapse functionality
- **SmartTextBox** - Advanced textbox with real-time validation and error visualization

🎨 **Visual Enhancements**
- Smooth anti-aliased graphics rendering
- Animated focus and error states
- Gradient header backgrounds
- Professional border styling and radius customization

✅ **Validation Support**
- Email, phone number (Portuguese), NIF, NIB, URL, and custom regex validation
- Real-time error feedback with visual indicators
- Customizable error messages and validation tooltips
- Password field with visibility toggle

🎯 **Icon Support**
- 160+ built-in icons from Segoe MDL2 Assets
- Visual icon picker in Visual Studio designer
- Categorized icons for easy selection (Communication, Security, Navigation, Files, Status, Commerce, Media, Misc)
- Support for both left and right icon positioning

## Quick Start

### Installation

1. Add reference to `SmartControls.dll` in your Windows Forms project
2. Build the solution
3. Drag controls from toolbox or use them in code

### Usage Examples

#### RoundedButton
```csharp
var button = new RoundedButton
{
    Text = "Click Me",
    BorderRadius = 20,
    BorderColor = Color.RoyalBlue,
    HoverBackColor = Color.LightBlue,
    Size = new Size(120, 40)
};
this.Controls.Add(button);
```

#### SmartTextBox with Email Validation
```csharp
var emailBox = new SmartTextBox
{
    ValidationMode = ValidationMode.Email,
    ValidationMessage = "Please enter a valid email",
    BorderRadius = 8,
    InnerIcon = InnerIconType.Email,
    Size = new Size(300, 40)
};
this.Controls.Add(emailBox);

// Check if valid
if (emailBox.IsValid)
    MessageBox.Show("Email: " + emailBox.Text);
```

#### RoundedPanel with Collapsible Header
```csharp
var panel = new RoundedPanel
{
    TextHeader = "Settings",
    ShowHeader = true,
    InnerIcon = InnerIconType.Settings,
    CollapseOnHeaderClick = true,
    Size = new Size(400, 300)
};
this.Controls.Add(panel);
```

## Component Details

### RoundedButton
- **Customizable**: Border radius, color, hover/press effects
- **Image Support**: Normal, Stretch, or Zoom sizing modes
- **State Effects**: Automatic color changes on hover and press
- **High Quality**: Anti-aliased rendering for smooth edges

**Key Properties**: `BorderRadius`, `BorderColor`, `HoverBackColor`, `PressedBackColor`, `ImageSizeMode`

### SmartTextBox
- **10 Validation Modes**: Required, Email, NIF, NIB, Telephone, URL, Number, TextWithoutSpace, CustomRegex
- **Error Visualization**: Triangle indicator with fade animation
- **Password Support**: Optional field masking with eye icon toggle
- **Focus Animation**: Smooth border color transition
- **Icon Support**: 160+ icons, left/right positioning

**Key Properties**: `ValidationMode`, `BorderRadius`, `IsPassword`, `InnerIcon`, `BorderFocusColor`, `BorderErrorColor`

### RoundedPanel
- **Header Options**: Optional header bar with customizable text, font, color
- **Collapse/Expand**: Click header to toggle, animates height smoothly
- **Corner Radius**: Independent control of each corner
- **Icons**: Display category icons in header
- **Gradient**: Optional gradient header background

**Key Properties**: `ShowHeader`, `HeaderHeight`, `TextHeader`, `IsCollapsed`, `BorderRadius`, `InnerIcon`

## Validation Modes

| Mode | Usage | Example |
|------|-------|---------|
| `Required` | Non-empty check | Username field |
| `Email` | Email format | Email address |
| `NIF` | Portuguese National ID (9 digits) | NIF: 123456789 |
| `NIB` | Portuguese Bank Account (21 digits) | Bank account |
| `Telephone` | Portuguese phone (9XXXXXXXX) | 912345678 |
| `URL` | Website URL | https://example.com |
| `Number` | Numeric only | Quantity |
| `TextWithoutSpace` | No spaces | Username |
| `CustomRegex` | Custom pattern | Custom validation |

## Supported Icons (160+)

Organized in 8 categories:

- **Communication**: Email, Phone, Chat, Send, Inbox
- **Users & Security**: User, UserAdd, Lock, Unlock, Shield, Key
- **Navigation**: Search, Settings, Menu, Home, Back, Forward, Refresh
- **Files & Data**: File, Folder, Save, Upload, Download, Database
- **Status**: Check, Close, Warning, Info, Error, Bell
- **Commerce**: Cart, CreditCard, Money, Wallet, Tag, Chart
- **Media & Scan**: Play, Pause, Camera, Image, Scan, QrCode
- **Misc**: Heart, Star, Location, Link, Globe, Edit, Trash, and more

## System Requirements

- **.NET Framework**: 4.7.2 or higher
- **Operating System**: Windows Vista or later
- **Fonts**: Segoe MDL2 Assets or Segoe Fluent Icons (for icons)

## Building

```bash
# Visual Studio
msbuild SmartControls.csproj /p:Configuration=Release

# Or open in Visual Studio and build normally
```

Output: `bin/Release/RoudedButton.dll`

## Designer Integration

SmartControls fully supports Visual Studio designer:

1. **Property Grid**: All properties organized by category (Appearance, Header, Validation, Behavior)
2. **Icon Picker**: Set `InnerIcon` property to open visual icon selection dialog
3. **Live Preview**: See changes in real-time on the designer surface
4. **Custom Editors**: UITypeEditor support for enum properties

## Performance Notes

- **Double Buffering**: Enabled on all controls for smooth rendering
- **Anti-aliasing**: High-quality graphics with Bézier curves for rounded corners
- **Animations**: Timer-based smooth transitions (15ms interval)
- **Memory**: Efficient resource management with proper disposal of brushes/pens

## License

This project is provided as-is.

## Support

For technical details, see [claude.md](claude.md) for comprehensive documentation of all classes, methods, and properties.