using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace BotModule
{
    /// <summary>
    /// Contains all different (Re-)Samplers and Readers/Streams
    /// </summary>
    public class BotWave
    {
        //===============================
        public const int channelCount = 2;
        public const int sampleRate = 48000;
        private const int sampleQuality = 60;

        public const int bitDepth = 16;
        //=================================


        /// <summary>
        /// Reader points to File or stream
        /// </summary>
        public MediaFoundationReader Reader { get; set; }

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

        public bool IncompatibleWaveFormat { get; set; }= false;
    }
}