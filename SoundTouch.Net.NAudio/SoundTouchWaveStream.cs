using System;
using System.Collections.Generic;

using NAudio.Wave;
using SoundTouch;

namespace NAudio.SoundTouch
{
	public class SoundTouchWaveStream : IWaveProvider
	{
		IWaveProvider _source;
		byte[] _sourceBuffer;
		short[] _sourceSamples;
		SoundTouch<short, long> _stretcher;
		short[] _stretchedSamples;
		double _tempo;
        double _pitch;

		public SoundTouchWaveStream(IWaveProvider source)
		{
			if (source.WaveFormat.BitsPerSample != 16)
				throw new FormatException("Can't process bit depth of " + source.WaveFormat.BitsPerSample);

			_source = source;

			_sourceSamples = new short[32768];
			_sourceBuffer = new byte[_sourceSamples.Length * 2];
			_stretchedSamples = new short[32768];

			_stretcher = new SoundTouch<short, long>();
			_stretcher.SetSampleRate(_source.WaveFormat.SampleRate);
			_stretcher.SetChannels(_source.WaveFormat.Channels);

			_tempo = 1.0;
		}

		public double Tempo
		{
			get { return _tempo; }
			set
			{
				_stretcher.SetTempo(value);
				_tempo = value;
			}
		}

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                _stretcher.SetPitch(value);
                _pitch = value;
            }
        }

		public WaveFormat WaveFormat => _source.WaveFormat;

		List<byte> _sourceExtraBytes = new List<byte>();
		List<byte> _outputExtraBytes = new List<byte>();

		public event EventHandler SourceRead;
		public event EventHandler EndOfStream;

		public int Read(byte[] buffer, int offset, int count)
		{
			int numRead = 0;

			// Mismatched formats/interpretations:
			//
			// - Source returns raw bytes, lets us interpret.
			// - SoundTouch takes samples (Int16), counts one frame across all channels as a single sample (one left + one right == one sample).
			// - When converting to/from bytes, we need to count each channel in a frame as a separate sample (one left + one right == two samples).
			// - We implement IWaveProvider, the same as source, and are thus expected to return raw bytes.
			// - We may be asked for a number of bytes that isn't a multiple of the stretcher's output block size.
			// - We may be provided with source data that isn't a multiple of the stretcher's input block size.
			//
			// Hooray!

			if (_outputExtraBytes.Count > 0)
			{
				if (_outputExtraBytes.Count > count)
				{
					_outputExtraBytes.CopyTo(0, buffer, offset, count);
					_outputExtraBytes.RemoveRange(0, count);

					return count;
				}
				else
				{
					_outputExtraBytes.CopyTo(buffer);

					count -= _outputExtraBytes.Count;
					numRead += _outputExtraBytes.Count;

					_outputExtraBytes.Clear();
				}
			}

			int bytesPerFrame = 2 * _source.WaveFormat.Channels;

			while (true)
			{
				int stretchedFramesToRead = (count + bytesPerFrame - 1) / bytesPerFrame;
				int stretchedFramesBytes = stretchedFramesToRead * bytesPerFrame;

				if (stretchedFramesBytes > _stretchedSamples.Length)
				{
					stretchedFramesToRead = _stretchedSamples.Length / bytesPerFrame;
					stretchedFramesBytes = stretchedFramesToRead * bytesPerFrame;
				}

				int numberOfFramesRead = _stretcher.ReceiveSamples(_stretchedSamples, stretchedFramesToRead);

				if (numberOfFramesRead == 0)
				{
					int sourceBytesRead = _sourceExtraBytes.Count;

					if (sourceBytesRead > 0)
					{
						_sourceExtraBytes.CopyTo(_sourceBuffer);
						_sourceExtraBytes.Clear();
					}

					sourceBytesRead += _source.Read(_sourceBuffer, sourceBytesRead, _sourceBuffer.Length - sourceBytesRead);

					SourceRead?.Invoke(this, EventArgs.Empty);

					if (sourceBytesRead == 0)
					{
						// End of stream, zero pad
						Array.Clear(buffer, offset, count);

						numRead += count;

						EndOfStream?.Invoke(this, EventArgs.Empty);

						return numRead;
					}

					int numberOfSourceSamples = (sourceBytesRead / 2 / _source.WaveFormat.Channels) * _source.WaveFormat.Channels;

					int sourceBytesInSamples = numberOfSourceSamples * 2;

					if (sourceBytesInSamples < sourceBytesRead)
					{
						// We got a misaligned read, stash the bytes we aren't going to process for the next pass.
						for (int i = sourceBytesInSamples; i < sourceBytesRead; i++)
							_sourceExtraBytes.Add(_sourceBuffer[i]);
					}

					for (int i = 0; i < numberOfSourceSamples; i++)
					{
						int lo = _sourceBuffer[i + i];
						int hi = _sourceBuffer[i + i + 1];

						_sourceSamples[i] = unchecked((short)((hi << 8) | lo));
					}

					_stretcher.PutSamples(_sourceSamples, numberOfSourceSamples / _source.WaveFormat.Channels);
				}
				else
				{
					int numberOfBytesAvailable = numberOfFramesRead * bytesPerFrame;

					int numberOfSamplesAvailable = numberOfBytesAvailable / 2;

					int i;

					for (i = 0; i < numberOfSamplesAvailable; i++)
					{
						if (count == 0)
							break;

						int sample = _stretchedSamples[i];

						unchecked
						{
							byte hi = (byte)(sample >> 8);
							byte lo = (byte)(sample & 0xFF);

							buffer[offset++] = lo;
							numRead++;
							count--;

							if (count == 0)
							{
								_outputExtraBytes.Add(hi);
								break;
							}

							buffer[offset++] = hi;
							numRead++;
							count--;
						}
					}

					for (; i < numberOfSamplesAvailable; i++)
					{
						int sample = _stretchedSamples[i];

						unchecked
						{
							byte hi = (byte)(sample >> 8);
							byte lo = (byte)(sample & 0xFF);

							_outputExtraBytes.Add(lo);
							_outputExtraBytes.Add(hi);
						}
					}

					if (count == 0)
						return numRead;
				}
			}
		}
	}
}
