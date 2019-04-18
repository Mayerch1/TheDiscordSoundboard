using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.Compression;

namespace BotModule
{
    /// <summary>
    /// Contains all different (Re-)Samplers and Readers/Streams
    /// </summary>
    public class BotWave
    {
        //===============================
        /// <summary>
        /// Desired channelCount for Discord
        /// </summary>
        public const int channelCount = 2;
        /// <summary>
        /// Desired channelCount for Discord
        /// </summary>
        public const int sampleRate = 48000;

        //not used, yet
        private const int sampleQuality = 60;


        /// <summary>
        /// Desired channelCount for Discord
        /// </summary>
        public const int bitDepth = 16;
        //=================================

        /// <summary>
        /// Reader points to File or stream
        /// </summary>
        public MediaFoundationReader Reader { get; set; }

        /// <summary>
        /// SampleProvider for changing Volume of stream
        /// </summary>
        public VolumeWaveProvider16 Volume { get; set; }

        /// <summary>
        /// Stream for changing speed of stream
        /// </summary>
        public NAudio.SoundTouch.SoundTouchWaveStream Speed { get; set; }

        /// <summary>
        /// Provider for Pitched samples
        /// </summary>
        public NAudio.Wave.SampleProviders.SmbPitchShiftingSampleProvider Pitch { get; set; }

        /// <summary>
        /// Resampler used to get next byte[] for feeding to bot
        /// </summary>
        public MediaFoundationResampler ActiveResampler { get; set; }

        // public MediaFoundationResampler NormalResampler { get; set; }

        /// <summary>
        /// Is never modified, to keep quality for multiple pitch/volume changes
        /// </summary>
        public MediaFoundationResampler SourceResampler { get; set; }

        /// <summary>
        /// device which delivers data
        /// </summary>
        public WasapiCapture Capture { get; set; }

      
        /// <summary>
        /// Format for streaming
        /// </summary>
        public readonly WaveFormat OutFormat = new WaveFormat(sampleRate, bitDepth, channelCount);
     
    }
}