using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Audio.OpenAL;
using OpenTK.Audio;

namespace Flak
{
    class AudioManager
    {
        AudioContext Context { get; set; }
        uint phaserBuffer;
        uint phaserSource;

        uint boomBuffer;
        uint boomSource;

        uint explode1Buffer;
        uint explode1Source;

        uint explode2Buffer;
        uint explode2Source;

        uint songBuffer;
        uint songSource;

        private AudioManager()
        {
            Context = new AudioContext();
            AL.GenBuffer(out phaserBuffer);
            AL.GenSource(out phaserSource);

            AL.GenBuffer(out boomBuffer);
            AL.GenSource(out boomSource);

            AL.GenBuffer(out explode1Buffer);
            AL.GenSource(out explode1Source);

            AL.GenBuffer(out explode2Buffer);
            AL.GenSource(out explode2Source);

            AL.GenBuffer(out songBuffer);
            AL.GenSource(out songSource);

            Byte[] buf = Flak.Properties.Resources.phaser;
            AL.BufferData((int)phaserBuffer, ALFormat.Mono8, buf, buf.Length, 22050);
            AL.Source(phaserSource, ALSourcei.Buffer, (int)phaserBuffer);

            buf = Flak.Properties.Resources.Boom_1;
            AL.BufferData((int)boomBuffer, ALFormat.Mono8, buf, buf.Length, 22050);
            AL.Source(boomSource, ALSourcei.Buffer, (int)boomBuffer);

            buf = Flak.Properties.Resources.explode;
            AL.BufferData((int)explode1Buffer, ALFormat.Mono8, buf, buf.Length, 22050);
            AL.Source(explode1Source, ALSourcei.Buffer, (int)explode1Buffer);

            buf = Flak.Properties.Resources.explode2;
            AL.BufferData((int)explode2Buffer, ALFormat.Mono8, buf, buf.Length, 11025);
            AL.Source(explode2Source, ALSourcei.Buffer, (int)explode2Buffer);

            buf = Flak.Properties.Resources.Bounce;
            AL.BufferData((int)songBuffer, ALFormat.Stereo8, buf, buf.Length, 44100);
            AL.Source(songSource, ALSourcei.Buffer, (int)songBuffer);
            AL.Source(songSource, ALSourceb.Looping, true);

            System.Diagnostics.Debug.WriteLine(AL.GetError());
        }

        public static void Initialize()
        {
            theInstance = new AudioManager();
        }

        static AudioManager theInstance;

        public static AudioManager Manager()
        {
            return theInstance;
        }

        public void PlayPhaser()
        {
            AL.SourcePlay(phaserSource);
        }

        public void PlayBoom()
        {
            AL.SourcePlay(boomSource);
        }

        public void PlayExplosion1()
        {
            AL.SourcePlay(explode1Source);
        }

        public void PlayExplosion2()
        {
            AL.SourcePlay(explode2Source);
        }

        public void PlayMusic()
        {
            AL.SourcePlay(songSource);
        }
    }
}
