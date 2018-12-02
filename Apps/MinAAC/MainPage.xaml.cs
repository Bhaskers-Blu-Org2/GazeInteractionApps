﻿//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.SpeechSynthesis;
using EyeGazeUserControls;

namespace MinAAC
{
    public sealed partial class MainPage : Page
    {
        MediaElement        _mediaElement;
        SpeechSynthesizer   _speechSynthesizer;

        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            _mediaElement = new MediaElement();
            _speechSynthesizer = new SpeechSynthesizer();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            GazeKeyboard.Target = TextControl;
            await GazeKeyboard.LoadLayout("MinAAC.xaml");
        }

        private async void OnSpeak(object sender, RoutedEventArgs e)
        {
            var text = TextControl.Text.ToString();
            var stream = await _speechSynthesizer.SynthesizeTextToStreamAsync(text);
            _mediaElement.SetSource(stream, stream.ContentType);
            _mediaElement.AutoPlay = true;
            _mediaElement.Play();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
