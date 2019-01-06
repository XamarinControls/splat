﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Splat
{
    /// <summary>
    /// A XAML based platform bitmap loader which will load our bitmaps for us.
    /// </summary>
    public class PlatformBitmapLoader : IBitmapLoader
    {
        /// <inheritdoc />
        public Task<IBitmap> Load(Stream sourceStream, float? desiredWidth, float? desiredHeight)
        {
            return Task.Run(() =>
            {
                var ret = new BitmapImage();

                WithInit(ret, source =>
                {
                    if (desiredWidth != null)
                    {
                        source.DecodePixelWidth = (int)desiredWidth;
                    }

                    if (desiredHeight != null)
                    {
                        source.DecodePixelHeight = (int)desiredHeight;
                    }

                    source.StreamSource = sourceStream;
                    source.CacheOption = BitmapCacheOption.OnLoad;
                });

                return (IBitmap)new BitmapSourceBitmap(ret);
            });
        }

        /// <inheritdoc />
        public Task<IBitmap> LoadFromResource(string resource, float? desiredWidth, float? desiredHeight)
        {
            return Task.Run(() =>
            {
                var ret = new BitmapImage();
                WithInit(ret, x =>
                {
                    if (desiredWidth != null)
                    {
                        x.DecodePixelWidth = (int)desiredWidth;
                    }

                    if (desiredHeight != null)
                    {
                        x.DecodePixelHeight = (int)desiredHeight;
                    }

                    x.UriSource = new Uri(resource, UriKind.RelativeOrAbsolute);
                });

                return (IBitmap)new BitmapSourceBitmap(ret);
            });
        }

        /// <inheritdoc />
        public IBitmap Create(float width, float height)
        {
            return new BitmapSourceBitmap(new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Default, null));
        }

        private static void WithInit(BitmapImage source, Action<BitmapImage> block)
        {
            source.BeginInit();
            block(source);
            source.EndInit();
            source.Freeze();
        }
    }
}