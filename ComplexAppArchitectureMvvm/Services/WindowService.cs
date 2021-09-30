using System.Windows;

namespace ComplexAppArchitectureMvvm.Services
{
	public static class WindowService
	{
		public static void Maximize(Window window, bool useKioskMode = true)
		{
			var alreadyInKioskMode = window.ResizeMode == ResizeMode.NoResize && window.WindowStyle == WindowStyle.None;
			var alreadyMaximized = window.WindowState == WindowState.Maximized;

			if (alreadyMaximized && alreadyInKioskMode == useKioskMode) return;

			// Setting these properties will enable a full-screen kiosk mode
			if (useKioskMode)
			{
				window.ResizeMode = ResizeMode.NoResize;
				window.WindowStyle = WindowStyle.None;
			}
			else
			{
				window.ResizeMode = ResizeMode.CanResizeWithGrip;
				window.WindowStyle = WindowStyle.SingleBorderWindow;
			}

			if (window.WindowState == WindowState.Maximized)
			{
				// When the screen is already maximized, the taskbar keeps showing and the window has an additional (taskbar related) offset
				// To counter this, simply set the screen to 'normal' and the schedule a maximized switch afterwards
				// Put the maximized switch on the dispatcher to ensure that the window state has fully become 'normal' before going to 'maximized'

				window.WindowState = WindowState.Normal;
				_ = window.Dispatcher.Invoke(() => window.WindowState = WindowState.Maximized);
			}
			else
			{
				window.WindowState = WindowState.Maximized;
			}
		}
	}
}
