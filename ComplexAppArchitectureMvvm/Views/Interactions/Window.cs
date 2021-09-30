using System;
using System.Windows;
using System.Windows.Media;

namespace ComplexAppArchitectureMvvm.Views.Interactions
{
	public static class Window
	{
		// Allow for optional modification of the title before it is set, e.g. translations
		public static Func<string, string> TitleTransformerHook { get; set; }
		// Allow for optional way to constrain the window that the title will be set on
		public static Func<System.Windows.Window, bool> TitleWindowPredicateHook { get; set; }
		public static string GetTitle(DependencyObject obj)
		{
			return (string)obj.GetValue(TitleProperty);
		}
		public static void SetTitle(DependencyObject obj, string value)
		{
			obj.SetValue(TitleProperty, value);
		}
		public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(FrameworkElement), new PropertyMetadata(string.Empty, OnTitlePropertyChanged));
		private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not FrameworkElement el) return;

			el.Unloaded -= OnTitleElementUnloaded;
			el.IsVisibleChanged -= OnTitleElementVisibilityChanged;

			el.Unloaded += OnTitleElementUnloaded;
			el.IsVisibleChanged += OnTitleElementVisibilityChanged;

			// Trigger initial update
			OnTitleElementVisibilityChanged(d, e);
		}
		private static void OnTitleElementUnloaded(object sender, RoutedEventArgs e)
		{
			if (sender is not FrameworkElement el) return;

			el.Unloaded -= OnTitleElementUnloaded;
			el.IsVisibleChanged -= OnTitleElementVisibilityChanged;
		}
		private static void OnTitleElementVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not UIElement el) return;
			if (el.Visibility != Visibility.Visible) return;

			// We need to go up the visual tree until we find a window and set it's title property to the value of the attached property
			var parentWindow = VisualTreeUtilities.GetParent<System.Windows.Window>(el);
			// Stop if we have no parent window, or the window is not valid
			if (parentWindow == null || !(TitleWindowPredicateHook?.Invoke(parentWindow) ?? true)) return;

			var newTitle = GetTitle(el);
			parentWindow.Title = TitleTransformerHook?.Invoke(newTitle) ?? newTitle;
		}
	}

	public class VisualTreeUtilities
	{
		public static T GetParent<T>(DependencyObject child, bool recursive = true) where T : DependencyObject
		{
			DependencyObject parent;

			do
			{
				parent = VisualTreeHelper.GetParent(child);

				if (parent is T tParent) return tParent;
				else child = parent;

				if (!recursive) break;

			} while (parent != null);

			return default;
		}
	}
}
