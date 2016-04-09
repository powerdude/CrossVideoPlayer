using System;
using CrossVideoPlayer.FormsPlugin.Abstractions;
using Xamarin.Forms;
using CrossVideoPlayer.FormsPlugin.WinUWP;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;

[assembly: ExportRenderer(typeof(CrossVideoPlayerView),
                          typeof(CrossVideoPlayerViewRenderer))]

namespace CrossVideoPlayer.FormsPlugin.WinUWP
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

                var mediaElement = new MediaElement
                {
                    Source = new Uri(crossVideoPlayerView.VideoSource),
                    AutoPlay = true,
                    Width = crossVideoPlayerView.Width > 0 ? crossVideoPlayerView.Width : 480, // TODO: figure a better way to set the right Width of the current view
                    Height = (crossVideoPlayerView.Height > 0 ? crossVideoPlayerView.Height : 480) / crossVideoPlayerView.VideoScale,
                };				

                crossVideoPlayerView.WidthRequest = mediaElement.Width;
                crossVideoPlayerView.HeightRequest = mediaElement.Height;

                Children.Add(mediaElement);
            }
        }
    }
}
