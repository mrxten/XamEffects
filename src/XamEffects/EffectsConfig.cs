using System;
using Xamarin.Forms;

namespace XamEffects
{
	public static class EffectsConfig
	{
		public static void Init()
		{
			// for linker
		}

		public static readonly BindableProperty ChildrenInputTransparentProperty =
			BindableProperty.CreateAttached(
				"ChildrenInputTransparent",
				typeof(bool),
				typeof(EffectsConfig),
				false,
				propertyChanged: PropertyChanged
			);

		public static void SetChildrenInputTransparent(BindableObject view, bool value)
		{
			view.SetValue(ChildrenInputTransparentProperty, value);
		}

		public static bool GetChildrenInputTransparent(BindableObject view)
		{
			return (bool)view.GetValue(ChildrenInputTransparentProperty);
		}

		private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (!(bindable is Layout layout))
				return;

			if (GetChildrenInputTransparent(bindable))
			{
				layout.ChildAdded += Layout_ChildAdded;
				foreach (var layoutChild in layout.Children)
				{
					if (layoutChild is VisualElement ve && AddInputTransparentToElement(layoutChild))
					{
						ve.InputTransparent = true;
					}
				}
			}
			else
			{
				layout.ChildAdded -= Layout_ChildAdded;
			}
		}

		private static void Layout_ChildAdded(object sender, ElementEventArgs e)
		{
			if (e.Element is VisualElement ve && AddInputTransparentToElement(e.Element))
				ve.InputTransparent = true;
		}

		private static bool AddInputTransparentToElement(BindableObject obj)
		{
			if (TouchEffect.GetColor(obj) != Color.Default || Commands.GetTap(obj) != null || Commands.GetLongTap(obj) != null)
				return false;
			return true;
		}
	}
}