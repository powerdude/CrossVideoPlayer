using Android.Widget;
using CrossVideoPlayer.FormsPlugin;
using CrossVideoPlayer.FormsPlugin.Droid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Xamarin.Forms.View;

[assembly: ExportRenderer(typeof(CrossVideoPlayerView), typeof(CrossVideoPlayerViewRenderer))]

namespace CrossVideoPlayer.FormsPlugin.Droid
{
	/// <summary>
	/// CrossVideoPlayer Renderer for Android.
	/// </summary>
	public class CrossVideoPlayerViewRenderer : ViewRenderer
	{

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			var crossVideoPlayerView = Element as CrossVideoPlayerView;

			if ((crossVideoPlayerView != null) && (e.OldElement == null))
			{
				if (string.IsNullOrEmpty(crossVideoPlayerView.VideoSource)) return;

				var metrics = Resources.DisplayMetrics;

				crossVideoPlayerView.HeightRequest = metrics.WidthPixels/metrics.Density/crossVideoPlayerView.VideoScale;

				var mediaElement = new VideoView(Context);

				var uri = Android.Net.Uri.Parse(crossVideoPlayerView.VideoSource);

				mediaElement.SetVideoURI(uri);

				// Hook up event handlers
				mediaElement.Prepared += (o, e1) => { crossVideoPlayerView.OnMediaLoaded(); };
				mediaElement.Error += (o, e1) => { crossVideoPlayerView.OnMediaErrorOccurred(e1.What.ToString()); };
				mediaElement.Completion += (o, e1) => { crossVideoPlayerView.OnMediaCompleted(); };
				//videoView.Info += (o, e1) => { };


				// Hook up commands
				crossVideoPlayerView.PlayCommand = new Command(() => mediaElement.Start());
				crossVideoPlayerView.PauseCommand = new Command(() => {
					System.Diagnostics.Debug.WriteLine("CrossVideoPlayer: Pause");
					mediaElement.Pause();
				}
				, () => mediaElement.IsPlaying);
				crossVideoPlayerView.SeekCommand = new Command<TimeSpan>((timeSpan) => {
					mediaElement.SeekTo((int)timeSpan.TotalMilliseconds);
				}, (timeSpan) =>
				{
					return timeSpan.TotalMilliseconds < mediaElement.CurrentPosition;
				});
				crossVideoPlayerView.StopCommand = new Command(() => mediaElement.StopPlayback());
				crossVideoPlayerView.MuteCommand = new Command<bool>((muted) => { }, (muted) => false);

				var mediaController = new MediaController(Context);

				mediaController.SetAnchorView(mediaElement);

				mediaElement.SetMediaController(mediaController);

				if (crossVideoPlayerView.AutoPlay)
				{
					mediaElement.Start();
				}

				SetNativeControl(mediaElement);
			}
		}
	}
}