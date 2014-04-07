using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TiledLib;


namespace LDEngine
{
    public static class AudioController
    {
        private static float _sfxVolume = 1f;
        public static float SFXVolume
        {
            get { return _sfxVolume; } 
            set { _sfxVolume = MathHelper.Clamp(value, 0f, 1f); }
        }

        private static float _musicVolume = 1f;
        public static float MusicVolume
        {
            get { return _musicVolume; }
            set { _musicVolume = MathHelper.Clamp(value, 0f, 1f); }
        }

        private static Dictionary<string, SoundEffect> _effects;

        private static Dictionary<string, SoundEffectInstance> _songs;

        private static string _playingTrack = "";
        private static bool _isPlaying;

        public static void LoadContent(ContentManager content)
        {
            _effects = new Dictionary<string, SoundEffect>();

            //_effects.Add("explode", content.Load<SoundEffect>("sfx/explode"));
            

            _songs = new Dictionary<string, SoundEffectInstance>();

            //_songs.Add("theme", content.Load<SoundEffect>("music").CreateInstance());

            foreach (SoundEffectInstance s in _songs.Values)
            {
                s.IsLooped = true;
                s.Volume = _musicVolume;
            }
        }

        public static void PlayMusic(string track)
        {
            if (!_songs.ContainsKey(track.ToLower())) return;

            StopMusic();

            _playingTrack = track.ToLower();
            _isPlaying = true;
            _songs[track].Play();
        }

        public static void StopMusic()
        {
            if (!_isPlaying) return;

            _isPlaying = false;
            _songs[_playingTrack].Stop();
        }

        public static void PlaySFX(string name)
        {
            _effects[name].Play(_sfxVolume, 0f, 0f);
        }
        public static void PlaySFX(string name, float pitch)
        {
            _effects[name].Play(_sfxVolume, pitch, 0f);
        }
        public static void PlaySFX(string name, float volume, float pitch, float pan)
        {
            if (pan < -1f || pan > 1f) return;

            pitch = MathHelper.Clamp(pitch, -1.00f, 1.00f);
            volume = MathHelper.Clamp(volume, 0f, 1f);

            _effects[name].Play(volume * _sfxVolume, pitch, pan);
        }
        public static void PlaySFX(string name, float minpitch, float maxpitch)
        {
            _effects[name].Play(_sfxVolume, minpitch + (Helper.RandomFloat(1f) * (maxpitch - minpitch)), 0f);
        }

        internal static void PlaySFX(string name, float volume, float minpitch, float maxpitch, Camera gameCamera, Vector2 position)
        {
            Vector2 screenPos = Vector2.Transform(position, gameCamera.CameraMatrix);

            float pan = (screenPos.X - (gameCamera.Width / 2f)) / (gameCamera.Width / 2f);
            if(pan>-1f && pan<1f)
                _effects[name].Play(volume * _sfxVolume, minpitch + (Helper.RandomFloat(1f) * (maxpitch - minpitch)), pan);
        }

        public static void Update(GameTime gameTime)
        {
            if (_playingTrack == "") return;

            foreach (SoundEffectInstance s in _songs.Values)
                s.Volume = _musicVolume;
        }


    }
}
