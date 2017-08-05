# XamEffects - UI effects for Xamarin.Forms
[![NuGet](https://img.shields.io/nuget/v/xameffects.svg?maxAge=259200&style=flat)](http://www.nuget.org/packages/XamEffects/)

### Features
* [TouchEffects](#toucheffect)
    * Add touch effect to views.
* [Commands](#commands)
    * Add command to views.

### Install
```bash
Install-Package XamEffects
```
You have to install this nuget package to PCL project and each platform project.

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

Also, for Linker you can call this method in Xamarin.Forms App.cs 
```csharp
public App()
{
    XamEffects.Effects.Init();
    ...
```

### Minimum requirements
iOS 8+, Android 4.3 (API 18) 

Operability of older versions is not guaranteed.

## TouchEffect

Add touch effect to views.

For Android API >=21 using Ripple effect, for Android API <21 and iOS using animated color selection.

iOS|Android API >=21|Android API <21
------------|------------|------------
<img src="images/touch/ios.gif" height="450" width="685"/>|<img src="images/touch/android.gif" height="450" width="711"/>|<img src="images/touch/old_android.gif" height="450" width="687"/>


### Supported Views (in case Xamarin.Forms 2.3.4)

|                 |iOS |Android|
|-----------------|----|-------|
|ActivityIndicator|✅   |✅      |
|BoxView          |✅   |✅      |
|Button           |✅   |✅      |
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
    * Background/Ripple color when touched. For deactivated effect set Color.Default value.
    
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

**Important:** if you need some gestures, use not GestureRecognizer, but [Commands](#commands) because effects doesn't work correctly with standard gestures in Xamarin.Forms.

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

## License

MIT Licensed.
