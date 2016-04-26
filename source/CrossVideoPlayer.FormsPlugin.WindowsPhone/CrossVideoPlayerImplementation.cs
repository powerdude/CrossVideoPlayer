using System;
using System.Windows.Controls;
using CrossVideoPlayer.FormsPlugin;
using Xamarin.Forms;
using CrossVideoPlayer.FormsPlugin.WindowsPhone;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CrossVideoPlayerView),
						  typeof(CrossVideoPlayerViewRenderer))]

namespace CrossVideoPlayer.FormsPlugin.WindowsPhone
{
	/// <summary>
	/// CrossVideoPlayer Renderer for Windows Phone Silverlight.
	/// </summary>
	public class CrossVideoPlayerViewRenderer : ViewRenderer
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
		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{

			base.OnElementChanged(e);

			var crossVideoPlayerView = Element as CrossVideoPlayerView;

			if (e.OldElement != null)
			{
				var oldCrossVideoPlayerView = e.OldElement as CrossVideoPlayerView;
			}

			if ((crossVideoPlayerView != null) && (e.OldElement == null))
			{
				if (string.IsNullOrEmpty(crossVideoPlayerView.VideoSource)) return;

				var mediaElement = new MediaElement
				{
					Source = new Uri(crossVideoPlayerView.VideoSource),
					AutoPlay = true,
					Width = crossVideoPlayerView.Width > 0 ? crossVideoPlayerView.Width : 480, // TODO: figure a better way to set the right Width of the current view
					Height = (crossVideoPlayerView.Height > 0 ? crossVideoPlayerView.Height : (480 / crossVideoPlayerView.VideoScale)),
				};

				//mediaElement.BufferingProgressChanged += (o, e1) => { };
				//mediaElement.CurrentStateChanged += (o, e1) => { };
				//mediaElement.DownloadProgressChanged += (o, e1) => { };
				//mediaElement.LogReady += (o, e1) => { };
				//mediaElement.MarkerReached += (o, e1) => { };
				mediaElement.MediaFailed += (o, e1) => { crossVideoPlayerView.OnMediaErrorOccurred(e1.ErrorException?.Message, e1.ErrorException); };
				mediaElement.MediaEnded += (o, e1) => { crossVideoPlayerView.OnMediaCompleted(); };
				mediaElement.MediaOpened += (o, e1) => { crossVideoPlayerView.OnMediaLoaded(); };

				crossVideoPlayerView.WidthRequest = mediaElement.Width;
				crossVideoPlayerView.HeightRequest = mediaElement.Height;

				Children.Add(mediaElement);
			}
		}
	}
}
