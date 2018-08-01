# XamEffects - UI effects for Xamarin.Forms
[![NuGet](https://img.shields.io/nuget/v/xameffects.svg?maxAge=259200&style=flat)](http://www.nuget.org/packages/XamEffects/)

### Features
* [TouchEffects](#toucheffect)
    * Add touch effect to views.
* [Commands](#commands)
    * Add command to views.
* [EffectsConfig](#effectsconfig)
   * Config for effects.

### Install
```bash
Install-Package XamEffects
```
You have to install this nuget package to Xamarin.Forms project and each platform project.

To use by iOS, you need to call Init method in AppDelegate.cs

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    global::Xamarin.Forms.Forms.Init();

    XamEffects.iOS.Effects.Init(); //write here

    LoadApplication(new App());
    return base.FinishedLaunching(app, options);
}
```

For Android should add Init to MainActivity.cs
```csharp
protected override void OnCreate(Bundle bundle)
{
    TabLayoutResource = Resource.Layout.Tabbar;
    ToolbarResource = Resource.Layout.Toolbar;

    base.OnCreate(bundle);

    XamEffects.Droid.Effects.Init();

    global::Xamarin.Forms.Forms.Init(this, bundle);
    LoadApplication(new App());
}
```

Also, for Linker you can call this method in Xamarin.Forms App.cs 
```csharp
public App()
{
    XamEffects.Effects.Init();
    ...
}
```

### Minimum requirements
iOS 8+, Android 4.3 (API 18) 

Operability of older versions is not guaranteed.

## TouchEffect

Add touch effect to views.

For Android API >=21 using Ripple effect, for Android API <21 and iOS using animated highlighted view.

iOS|Android API >=21|Android API <21
------------|------------|------------
<img src="images/touch/ios.gif" height="450" width="685"/>|<img src="images/touch/android.gif" height="450" width="711"/>|<img src="images/touch/old_android.gif" height="450" width="687"/>


### Supported Views (in case Xamarin.Forms 2.5.0)

|                 |iOS |Android|
|-----------------|----|-------|
|ActivityIndicator|✅   |✅      |
|BoxView          |✅   |✅      |
|Button           |❌   |❌      |
|DatePicker       |❌   |✅      |
|Editor           |❌   |❌      |
|Entry            |❌   |❌      |
|Image            |✅   |✅      |
|Label            |✅   |✅      |
|ListView         |✅   |❌      |
|Picker           |❌   |✅      |
|ProgressBar      |✅   |✅      |
|SearchBar        |❌   |❌      |
|Slider           |✅   |❌      |
|Stepper          |✅   |❌      |
|Switch           |❌   |✅      |
|TableView        |❌   |❌      |
|TimePicker       |❌   |✅      |
|WebView          |❌   |❌      |
|ContentPresenter |✅   |✅      |
|ContentView      |✅   |✅      |
|Frame            |✅   |❌      |
|ScrollView       |✅   |❌      |
|TemplatedView    |✅   |✅      |
|AbsoluteLayout   |✅   |✅      |
|Grid             |✅   |✅      |
|RelativeLayout   |✅   |✅      |
|StackLayout      |✅   |✅      |

### Parameters

* Color
    * Background/Ripple color when touched. For deactivate effect set Color.Default value.
    
### Example 

```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:XamEffects.Sample"
             xmlns:xe="clr-namespace:XamEffects;assembly=XamEffects"
             x:Class="XamEffects.Sample.MainPage">
    <Grid HorizontalOptions="Center"
          VerticalOptions="Center"
          HeightRequest="100"
          WidthRequest="200"
          BackgroundColor="LightGray" 
          xe:TouchEffect.Color="Red">
        <Label Text="Test touch effect"
               HorizontalOptions="Center"
               VerticalOptions="Center"/>
    </Grid>
</ContentPage>
```

**Important:** if you need some gestures with touch effect, use not GestureRecognizer, but [Commands](#commands) because effects doesn't work correctly with standard gestures in Xamarin.Forms.

## Commands

Add command to views.

### Parameters

* Tap
    * Tap Command
* TapParameter
    * Tap Command Parameter
* LongTap
    * Long Tap Command
* LongTapParameter
    * Long Tap Command Parameter
    
### Example 

```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:XamEffects.Sample"
             xmlns:xe="clr-namespace:XamEffects;assembly=XamEffects"
             x:Class="XamEffects.Sample.MainPage">
    <Grid HorizontalOptions="Center"
          VerticalOptions="Center"
          HeightRequest="100"
          WidthRequest="200"
          BackgroundColor="LightGray" 
          xe:Commands.Tap="{Binding TapCommand}"
          xe:Commands.LongTap="{Binding LongTapCommand}">
        <Label Text="Test commands"
               HorizontalOptions="Center"
               VerticalOptions="Center"/>
    </Grid>
</ContentPage>
```

## EffectsConfig

Config for effects.

### Parameters

* ChildrenInputTransparent
    * Set InputTransparent = True for all layout's children

#### ChildrenInputTransparent
If you use **TouchEffect** or **Commands** for Layout (Grid, StackLayout, etc.) you have to set this parameter to True otherwise in Android layout's children will overlaps these effects. Also you can set `InputTransparent = True` for each children (EXCEPT views using any effect) manually.

### Example 

```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:XamEffects.Sample"
             xmlns:xe="clr-namespace:XamEffects;assembly=XamEffects"
             x:Class="XamEffects.Sample.MainPage">
    <Grid HorizontalOptions="Center"
          VerticalOptions="Center"
          HeightRequest="100"
          WidthRequest="200"
          BackgroundColor="LightGray" 
          xe:Commands.Tap="{Binding TapCommand}"
          xe:EffectsConfig.ChildrenInputTransparent="True">
        <Label Text="Now you can tap to Label too"
               HorizontalOptions="Center"
               VerticalOptions="Center"/>
    </Grid>
</ContentPage>
```

## License
MIT Licensed.

### Release notes
#### 1.5.1
Fix bugs, add support tap through overlapped effect for fast clicks

#### 1.5.0
Update to .NETStandard 2.0, fix bugs, add EffectsConfig.

#### 1.5.0-pre
Update to .NETStandard 1.6

#### 1.4.0
Update XForms to 2.5.0, fix bug with nesting effects, fix bug with iOS long tap gesture.

#### 1.4.0-pre
Updat XForms to 2.5+

#### 1.3.3
Stable version for XF 2.3.4
