using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace CrossVideoPlayer.FormsPlugin
{
	/// <summary>
	/// CrossVideoPlayerView is a View which contains a MediaElement to play a video.
	/// </summary>
	public class CrossVideoPlayerView : View
	{
		#region Commands
		public ICommand PlayCommand { get; internal set; }
		public ICommand PauseCommand { get; internal set; }
		public ICommand StopCommand { get; internal set; }
		public ICommand SeekCommand { get; internal set; }
		public ICommand MuteCommand { get; internal set; }
        #endregion Commands

        #region Properties
        #region Video Source
        /// <summary>
        /// The url source of the video.
        /// </summary>
        public static readonly BindableProperty VideoSourceProperty =
            BindableProperty.Create<CrossVideoPlayerView, string>(p => p.VideoSource, "");
			//BindableProperty.Create(nameof(VideoSource), typeof(string), typeof(CrossVideoPlayerView), 
			//														"",
			//														BindingMode.Default
			//														, (bindable, value) =>
			//														{
			//															// validate the new value
			//															System.Diagnostics.Debug.WriteLine("CrossVideoPlayerView: VideoSource [Value: {0}]", value);

			//															return true;
			//														},
			//														(bindable, oldvalue, newvalue) =>
			//														{
			//															//property changed
			//															System.Diagnostics.Debug.WriteLine("CrossVideoPlayerView: VideoSource [OldValue: {0}] [NewValue: {1}]", oldvalue, newvalue);
			//														},
			//														(bindable, oldvalue, newvalue) =>
			//														{
			//															//property changing
			//															System.Diagnostics.Debug.WriteLine("CrossVideoPlayerView: VideoSource [OldValue: {0}] [NewValue: {1}]", oldvalue, newvalue);
			//														},
			//														(bindable, value) =>
			//														{
			//															//coerce value
			//															System.Diagnostics.Debug.WriteLine("CrossVideoPlayerView: VideoSource [Value: {0}]", value);
			//															return value;
			//														},
			//														(bindable) =>
			//														{
			//															//default constructor
			//															return "";
			//														}
			//														);

		/// <summary>
		/// The url source of the video.
		/// </summary>
		public string VideoSource { get { return (string)GetValue(VideoSourceProperty); } set { SetValue(VideoSourceProperty, value); } }
		#endregion Video Source

		#region Auto Play
		public static readonly BindableProperty AutoPlayProperty =
			//BindableProperty.Create<CrossVideoPlayerView, bool>(p => p.AutoPlay, true);
			BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(CrossVideoPlayerView),
																	true,
																	BindingMode.Default);

		public bool AutoPlay { get { return (bool)GetValue(AutoPlayProperty); } set { SetValue(AutoPlayProperty, value); } }
		#endregion Auto Play

		#region Is Muted
		public static readonly BindableProperty MutedProperty =
			BindableProperty.Create(nameof(IsMuted), typeof(bool), typeof(CrossVideoPlayerView),
																	true,
																	BindingMode.Default, 
																	propertyChanged: (bindable, value, newValue) =>
																	{
																		var view = bindable as CrossVideoPlayerView;
																		if (view != null) view.IsMuted = (bool) newValue;
																	});

		public bool IsMuted { get { return (bool)GetValue(MutedProperty); } set { SetValue(MutedProperty, value); } }
		#endregion Is Muted

		#region Media State
		public static readonly BindableProperty MediaStateProperty =
			BindableProperty.Create(nameof(MediaState), typeof(MediaState), typeof(CrossVideoPlayerView),
																	MediaState.Unknown,
																	BindingMode.TwoWay);

		public MediaState MediaState { get { return (MediaState)GetValue(MediaStateProperty); } set { SetValue(MediaStateProperty, value); } }
		#endregion Media State

		#region Video Scale
		/// <summary>
		/// The scale format of the video which is in most cases 16:9 (1.77) or 4:3 (1.33).
		/// </summary>
		public static readonly BindableProperty VideoScaleProperty =
			//BindableProperty.Create<CrossVideoPlayerView, double>(p => p.VideoScale, 1.77);
			BindableProperty.Create(nameof(VideoScale), typeof(double), typeof(CrossVideoPlayerView),
																	1.77,
																	BindingMode.Default);

		/// <summary>
		/// The scale format of the video which is in most cases 16:9 (1.77) or 4:3 (1.33).
		/// </summary>
		public double VideoScale { get { return (double)GetValue(VideoScaleProperty); } set	{ SetValue(VideoScaleProperty, value); } }
		#endregion Video Scale

		#region Position
		/// <summary>
		/// The position of the player for the current media file
		/// </summary>
		public static readonly BindableProperty PositionProperty =
			BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(CrossVideoPlayerView),
																	new TimeSpan(),
																	BindingMode.TwoWay);

		/// <summary>
		/// The position of the player for the current media file
		/// </summary>
		public double Position { get { return (double)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
		#endregion Position
		#endregion Properties

		#region Events Handlers
		public event EventHandler<MediaProgressEventArgs> MediaStateChanged;
		public event EventHandler<MediaErrorEventArgs> MediaError;
		#endregion Events Handlers

		#region Methods

		public void Play()
		{
			if (PlayCommand?.CanExecute(null) == true)
			{
				PlayCommand?.Execute(null);
			}

			MediaState = MediaState.Playing;
			OnMediaStateChanged();
		}

		public void Stop()
		{
			if (StopCommand?.CanExecute(null) == true)
			{
				StopCommand?.Execute(null);
			}

			MediaState = MediaState.Stopped;
			OnMediaStateChanged();
		}

		public void Pause()
		{
			if (PauseCommand?.CanExecute(null) == true)
			{
				PauseCommand?.Execute(null);
			}

			MediaState = MediaState.Paused;
			OnMediaStateChanged();
		}

		public void Seek(TimeSpan timeSpan)
		{
			if (SeekCommand?.CanExecute(timeSpan) == true)
			{
				SeekCommand?.Execute(timeSpan);
			}

			MediaState = MediaState.Seeking;
			OnMediaStateChanged();
		}

		public void Seek(int seconds)
		{
			Seek(new TimeSpan(0, 0, 0, seconds));
		}

		public void Mute(bool enabled)
		{
			if (MuteCommand?.CanExecute(enabled) == true)
			{
				MuteCommand?.Execute(enabled);
			}
		}
		#endregion Methods

		#region Events
		internal void OnMediaLoaded()
		{
			MediaState = MediaState.Loaded;

			OnMediaStateChanged();
		}

		internal void OnMediaStarted()
		{
			MediaState = MediaState.Playing;

			OnMediaStateChanged();
		}

		internal void OnMediaCompleted()
		{
			MediaState = MediaState.Finished;

			OnMediaStateChanged();
		}

		internal void OnMediaErrorOccurred(string errorMessaige, Exception ex = null)
		{
			MediaState = MediaState.Error;
			
			MediaError?.    Invoke(this, new MediaErrorEventArgs { ErrorMessage = errorMessaige, ErrorObject = ex, OriginalSource = VideoSource });
		}

		internal void OnMediaStateChanged()
		{
			MediaStateChanged?.Invoke(this, new MediaProgressEventArgs { State = MediaState });
		}

		#endregion Events
	}

	#region Enums

	public enum MediaState
	{
		Unknown,
		Stopped,
		Starting,
		Playing,
		Seeking,
		Paused,
		Loaded,
		Finished,
		Error
	}
	#endregion Enums

	#region Event Arguments
	public class MediaProgressEventArgs : EventArgs
	{
		public MediaState State { get; set; }
	}

	public class MediaErrorEventArgs : EventArgs
	{
		public string ErrorMessage { get; set; }
		public Exception ErrorObject { get; set; }
		public string OriginalSource { get; set; }
	}
	#endregion Event Arguments
}
