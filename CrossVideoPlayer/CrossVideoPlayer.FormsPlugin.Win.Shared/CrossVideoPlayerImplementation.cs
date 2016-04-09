using System;
using CrossVideoPlayer.FormsPlugin.Abstractions;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using CrossVideoPlayer.FormsPlugin.Windows;

#if NETFX_CORE && WINDOWS_UWP
using Xamarin.Forms.Platform.UWP;
#elif NETFX_CORE && WINDOWS_PHONE_APP
using Xamarin.Forms.Platform.WinRT;
#elif NETFX_CORE && WINDOWS_APP
using Xamarin.Forms.Platform.WinRT;
#elif SILVERLIGHT && WINDOWS_PHONE
using Xamarin.Forms.Platform.WinPhone;
#endif

[assembly: ExportRenderer(typeof(CrossVideoPlayerView),
						  typeof(CrossVideoPlayerViewRenderer))]

namespace CrossVideoPlayer.FormsPlugin.Windows
{
	/// <summary>
	/// CrossVideoPlayer Renderer for Windows Phone Silverlight.
	/// </summary>
	public class CrossVideoPlayerViewRenderer : ViewRenderer<CrossVideoPlayerView, MediaElement>
	{

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}		

		/// <summary>
		/// Reload the view and hit up the MediaElement. 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<CrossVideoPlayerView> e)
		{

			base.OnElementChanged(e);

			var crossVideoPlayerView = Element as CrossVideoPlayerView;

			if ((crossVideoPlayerView != null) && (e.OldElement == null))
			{
				if (crossVideoPlayerView.VideoSource == null) return;

				var mediaElement = new MediaElement
				{
					Source = new Uri(crossVideoPlayerView.VideoSource),
					AutoPlay = crossVideoPlayerView.AutoPlay,
					Width = crossVideoPlayerView.WidthRequest > 0 ? crossVideoPlayerView.WidthRequest : 480, // TODO: figure a better way to set the right Width of the current view
					Height = (crossVideoPlayerView.HeightRequest > 0 ? crossVideoPlayerView.HeightRequest : 480) / crossVideoPlayerView.VideoScale,
				};

				//crossVideoPlayerView.WidthRequest = mediaElement.Width;
				//crossVideoPlayerView.HeightRequest = mediaElement.Height;				

				SetNativeControl(mediaElement);
				//Children.Add(mediaElement);
			}
		}
	}
}
