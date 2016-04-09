using CrossVideoPlayer.FormsPlugin.Abstractions;
using Xamarin.Forms;
using CrossVideoPlayer.FormsPlugin.iOSUnified;
using Xamarin.Forms.Platform.iOS;
using AVKit;
using AVFoundation;
using UIKit;

[assembly: ExportRenderer(typeof(CrossVideoPlayerView), typeof(CrossVideoPlayerRenderer))]

namespace CrossVideoPlayer.FormsPlugin.iOSUnified
{
	/// <summary>
	/// CrossVideoPlayer Renderer for iOS (Not implemented!).
	/// </summary>
	public class CrossVideoPlayerRenderer : ViewRenderer<CrossVideoPlayerView, UIView>
	{
		//globally declare variables
		AVAsset _asset;
		AVPlayerItem _playerItem;
		AVPlayer _player;

		AVPlayerLayer _playerLayer;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CrossVideoPlayerView> e)
		{
			base.OnElementChanged(e);

			var crossVideoPlayerView = Element as CrossVideoPlayerView;

			if ((crossVideoPlayerView != null) && (e.OldElement == null))
			{
				if (crossVideoPlayerView.VideoSource == null) return;

				_asset = AVAsset.FromUrl(new Foundation.NSUrl(crossVideoPlayerView.VideoSource));
				_playerItem = new AVPlayerItem(_asset);
				_player = new AVPlayer(_playerItem);
				_playerLayer = AVPlayerLayer.FromPlayer(_player);

				NativeView.Layer.AddSublayer(_playerLayer);

				if (crossVideoPlayerView.AutoPlay)
				{
					_player.Play();
				}
			}
		}
		
		//public override void LayoutSubviews()
		//{
		//	base.LayoutSubviews();

		//	//layout the elements depending on what screen orientation we are. 
		//	if (DeviceHelper.iOSDevice.Orientation == UIDeviceOrientation.Portrait)
		//	{
		//		playButton.Frame = new CGRect(0, NativeView.Frame.Bottom - 50, NativeView.Frame.Width, 50);
		//		_playerLayer.Frame = NativeView.Frame;
		//		NativeView.Layer.AddSublayer(_playerLayer);
		//		NativeView.Add(playButton);
		//	}
		//	else if (DeviceHelper.iOSDevice.Orientation == UIDeviceOrientation.LandscapeLeft || DeviceHelper.iOSDevice.Orientation == UIDeviceOrientation.LandscapeRight)
		//	{
		//		_playerLayer.Frame = NativeView.Frame;
		//		NativeView.Layer.AddSublayer(_playerLayer);
		//		playButton.Frame = new CGRect(0, 0, 0, 0);
		//	}
		//}
	}
}
