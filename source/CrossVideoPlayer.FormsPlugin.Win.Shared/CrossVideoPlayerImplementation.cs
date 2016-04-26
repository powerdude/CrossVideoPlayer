using System;
using CrossVideoPlayer.FormsPlugin;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using CrossVideoPlayer.FormsPlugin.Windows;
using System.ComponentModel;

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
		MediaElement _mediaElement;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var crossVideoPlayerView = sender as CrossVideoPlayerView;

			if (crossVideoPlayerView != null)
			{
				UpdateOrCreateMediaElement(crossVideoPlayerView, string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0);

				UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		/// <summary>
		/// Reload the view and hit up the MediaElement.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<CrossVideoPlayerView> e)
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

				UpdateOrCreateMediaElement(crossVideoPlayerView, true);
			}

		}

		private void UpdateOrCreateMediaElement(CrossVideoPlayerView crossVideoPlayerView, bool updateSource = false)
		{
			if (_mediaElement == null)
			{
				_mediaElement = new MediaElement();

				// Hook up event handlers
				//mediaElement.BufferingProgressChanged += (o, e1) => { };
				//mediaElement.CurrentStateChanged += (o, e1) => { };
				//mediaElement.DownloadProgressChanged += (o, e1) => { };
				//mediaElement.MarkerReached += (o, e1) => { };
				_mediaElement.MediaFailed += (o, e1) => { crossVideoPlayerView.OnMediaErrorOccurred(e1.ErrorMessage); };
				_mediaElement.MediaEnded += (o, e1) => { crossVideoPlayerView.OnMediaCompleted(); };
				_mediaElement.MediaOpened += (o, e1) => { crossVideoPlayerView.OnMediaLoaded(); };
				//mediaElement.RateChanged += (o, e1) => { };
				//_mediaElement.SeekCompleted += (o, e1) => { crossVideoPlayerView.OnSeekCompleted(_mediaElement.Position); };
				//mediaElement.VolumeChanged += (o, e1) => { };

				// Hook up commands
				crossVideoPlayerView.PlayCommand = new Command(() => _mediaElement.Play());
				crossVideoPlayerView.PauseCommand = new Command(() => {
					System.Diagnostics.Debug.WriteLine("CrossVideoPlayer: Pause");
					_mediaElement.Pause();
				}
				, () => _mediaElement.CanPause);
				crossVideoPlayerView.SeekCommand = new Command<TimeSpan>((timeSpan) => {
					_mediaElement.Position = timeSpan;
				}, (timeSpan) =>
				{
					return _mediaElement.CanSeek && timeSpan.TotalSeconds < _mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
				});
				crossVideoPlayerView.StopCommand = new Command(() => _mediaElement.Stop());
				crossVideoPlayerView.MuteCommand = new Command<bool>((muted) => _mediaElement.IsMuted = muted);

				SetNativeControl(_mediaElement);
			}
			else
			{
				UpdateNativeControl();
			}

			//mediaElement.Height = mediaElement.Width / crossVideoPlayerView.VideoScale;

			if (updateSource && !string.IsNullOrEmpty(crossVideoPlayerView.VideoSource))
			{
				_mediaElement.Source = new Uri(crossVideoPlayerView.VideoSource);
			}

			_mediaElement.AutoPlay = crossVideoPlayerView.AutoPlay;
			_mediaElement.Width = crossVideoPlayerView.Width > 0 ? crossVideoPlayerView.Width : 480; // TODO: figure a better way to set the right Width of the current view
			_mediaElement.Height = (crossVideoPlayerView.Height > 0 ? crossVideoPlayerView.Height : (480 / crossVideoPlayerView.VideoScale));

			crossVideoPlayerView.WidthRequest = _mediaElement.Width;
			crossVideoPlayerView.HeightRequest = _mediaElement.Height;
		}
	}
}
