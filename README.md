# XamEffects - UI effects for Xamarin.Forms
[![NuGet](https://img.shields.io/nuget/v/xameffects.svg?maxAge=259200&style=flat)](http://www.nuget.org/packages/XamEffects/)

### Features
* [TouchEffect](#toucheffect) - Add touch effect to views.
* [Commands](#commands) - Add command to views.
* [EffectsConfig](#effectsconfig) - Config for effects.

### Install
```bash
Install-Package XamEffects
```
You have to install this nuget package to Xamarin.Forms project and each platform project.

To use in iOS, you need to call Init method in AppDelegate.cs
```csharp 
XamEffects.iOS.Effects.Init();
```

For Android add Init to MainActivity.cs
```csharp
XamEffects.Droid.Effects.Init();
```

### Minimum requirements
iOS 8, Android 4.1 (API 16)

Xamarin.Forms 2.5.0

Operability of older versions is not guaranteed.

## TouchEffect
Add touch effect to views.

For Android API >=21 using Ripple effect, for Android API <21 and iOS using animated highlighted view.

iOS|Android API >=21|Android API <21
------------|------------|------------
<img src="images/touch/ios.gif" width="450"/>|<img src="images/touch/android.gif" width="450"/>|<img src="images/touch/old_android.gif" width="450"/>


### Supported Views 
Almost all usual views and layouts without another gestures and effects, e.g. for views like Button, Slider, Picker, Entry, Editor etc. effect won't work. Also not working in some views with enabled Fast Renderers. If effect doesn't work, try wrap view with ContentView and add effect to wrapper.

### Bindable properties

* **Color** - Front/Ripple color when touched. For deactivate effect set Color.Default value. If color will have alpha channel is 255 effect will change it to ±80.
    
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

```csharp
TouchEffect.SetColor(view, Color.Red);
```

**Important: if you need some gestures with touch effect, use not GestureRecognizer, but [Commands](#commands) because effects doesn't work correctly with standard gestures in Xamarin.Forms.**

## Commands

Add command to views.

### Bindable properties

* **Tap** - Tap Command
* **TapParameter** - Tap Command Parameter
* **LongTap** - Long Tap Command
* **LongTapParameter** - Long Tap Command Parameter
    
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

```csharp
Commands.SetTap(view, new Command(() => {
  //do something
}));
Commands.SetTapParameter(view, someObject);
```

## EffectsConfig

Configs and helpers for effects.

### Bindable properties

#### ChildrenInputTransparent 
If you use **TouchEffect** or **Commands** for Layout (Grid, StackLayout, etc.) and EffectsConfig.AutoChildrenInputTransparent is False you have to set this parameter to True otherwise in Android layout's children will overlaps these effects. Also you can set `InputTransparent = True` for each children (EXCEPT views using any effect) manually.
    
### Another fields

#### AutoChildrenInputTransparent 
Set ChildrenInputTransparent automatically for views with TouchEffect or Command. If value is True you **DON'T** need manually configure **ChildrenInputTransparent**. Default value is True.


## License
MIT Licensed.

## To do
Support UWP

### Release notes
Moved to [Release notes](ReleaseNotes.md)
