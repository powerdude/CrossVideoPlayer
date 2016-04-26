using Android.Widget;
using CrossVideoPlayer.FormsPlugin;
using CrossVideoPlayer.FormsPlugin.Droid;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Xamarin.Forms.View;

[assembly: ExportRenderer(typeof(CrossVideoPlayerView), typeof(CrossVideoPlayerViewRenderer))]
namespace CrossVideoPlayer.FormsPlugin.Droid
{
	/// <summary>
	/// CrossVideoPlayer Renderer for Android.
	/// </summary>
	public class CrossVideoPlayerViewRenderer : ViewRenderer<CrossVideoPlayerView,VideoView>
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CrossVideoPlayerView> e)
		{
			base.OnElementChanged(e);

			System.Diagnostics.Debug.WriteLine("CrossVideoPlayer: changing element");

		    if (Control == null)
		    {
			    System.Diagnostics.Debug.WriteLine("CrossVideoPlayer: creating control");
                var element = new VideoView(Context);
		        element.Prepared += MediaElement_Prepared;
		        element.Error += Element_Error;
		        element.Completion += Element_Completion;

		        var mediaController = new MediaController(Context);
		        mediaController.SetAnchorView(element);
		        element.SetMediaController(mediaController);
		        SetNativeControl(element);
		    }

		    // Hook up commands
			Element.PlayCommand = new Command(() => Control.Start());
			Element.PauseCommand = new Command(() => {
				System.Diagnostics.Debug.WriteLine("CrossVideoPlayer: Pause");
				Control.Pause();
			}
			, () => Control.IsPlaying);
			Element.SeekCommand = new Command<TimeSpan>((timeSpan) => {
				Control.SeekTo((int)timeSpan.TotalMilliseconds);
			}, (timeSpan) =>
			{
				return timeSpan.TotalMilliseconds < Control.CurrentPosition;
			});
			Element.StopCommand = new Command(() => Control.StopPlayback());
			Element.MuteCommand = new Command<bool>((muted) => { }, (muted) => false);

			UpdateUri();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (string.Compare(e.PropertyName, "VideoSource", StringComparison.OrdinalIgnoreCase) == 0)
			{
				UpdateUri();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void Element_Completion(object sender, EventArgs e)
		{
			Element?.OnMediaCompleted();
		}

		private void Element_Error(object sender, Android.Media.MediaPlayer.ErrorEventArgs e)
		{
			Element?.OnMediaErrorOccurred(e.What.ToString());

		}

		private void UpdateUri()
		{
			if (string.IsNullOrEmpty(Element.VideoSource)) return;

			var metrics = Resources.DisplayMetrics;

			Element.HeightRequest = metrics.HeightPixels / metrics.Density / Element.VideoScale;
			Element.WidthRequest = metrics.WidthPixels / metrics.Density / Element.VideoScale;

			var uri = Android.Net.Uri.Parse(Element.VideoSource);

			Control.SetVideoURI(uri);

			if (Element.AutoPlay)
			{
				Control.Start();
			}

		}

		private void MediaElement_Prepared(object sender, EventArgs e)
		{
			Element?.OnMediaLoaded();
		}
	}
}