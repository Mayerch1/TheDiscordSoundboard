using System;
using System.Collections.Generic;
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
        public WaveFormat OutFormat { get; set; }
    }
}
