﻿using SFML.Audio;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
   public class Audio : Thing
   {
      private readonly static List<Audio> audios = new();
      private bool stopped, rewindIsNotLoop;
      private double lastFrameProg;

      public static class Event
      {
         public static class Subscribe
         {
            public static void OnStart(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.AudioStart, thingUID, order);
            public static void OnPlay(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.AudioPlay, thingUID, order);
            public static void OnPause(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.AudioPause, thingUID, order);
            public static void OnStop(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.AudioStop, thingUID, order);
            public static void OnEnd(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.AudioEnd, thingUID, order);
            public static void OnLoop(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.AudioLoop, thingUID, order);
         }
         public static class Unsubscribe
         {
            public static void OnStart(string thingUID) =>
               Events.Disable(Events.Type.AudioStart, thingUID);
            public static void OnPlay(string thingUID) =>
               Events.Disable(Events.Type.AudioPlay, thingUID);
            public static void OnPause(string thingUID) =>
               Events.Disable(Events.Type.AudioPause, thingUID);
            public static void OnStop(string thingUID) =>
               Events.Disable(Events.Type.AudioStop, thingUID);
            public static void OnEnd(string thingUID) =>
               Events.Disable(Events.Type.AudioEnd, thingUID);
            public static void OnLoop(string thingUID) =>
               Events.Disable(Events.Type.AudioLoop, thingUID);
         }
      }

      private bool IsMono()
      {
         if ((CurrentType == Type.Sound && sound.SoundBuffer.ChannelCount == 1) ||
            (CurrentType == Type.Music && music.ChannelCount == 1) ||
            CurrentType == Type.NotLoaded)
            return false;

         Debug.LogError(2,
            $"This will have no effect since the file '{FilePath}' has 2 channels (stereo) and\n" +
            $"only single channelled (mono) files support this.\n" +
            $"Please convert to mono before using environmental audio.");
         return true;
      }
      private bool IsNotLoaded()
      {
         if (CurrentType != Type.NotLoaded) return false;

         Debug.LogError(2, $"No audio file is taken into account. Use the {nameof(FilePath)} property if needed.");
         return true;
      }

      //===============

      internal Sound sound;
      internal Music music;
		internal static void Update()
		{
			for (int i = 0; i < audios.Count; i++)
			{
            if (Gate.EnterOnceWhile($"{audios[i].FilePath}-enda915'kf", audios[i].IsPlaying == false &&
               audios[i].IsPaused == false && audios[i].stopped == false))
               Events.Notify(Events.Type.AudioEnd, new() { Audio = audios[i] });
            if (audios[i].FileProgress + audios[i].FileDuration / 2 < audios[i].lastFrameProg && audios[i].rewindIsNotLoop == false)
               Events.Notify(Events.Type.AudioLoop, new() { Audio = audios[i] });
            audios[i].lastFrameProg = audios[i].FileProgress;
            audios[i].rewindIsNotLoop = false;
         }
		}

      //=============

      public enum Type { NotLoaded, Sound, Music }

      public static Point ListenerPosition
      {
         get { return new() { X = Listener.Position.X, Y = Listener.Position.Z }; }
         set { Listener.Position = new Vector3f((float)value.X, 0, (float)value.Y); }
      }

      public string FilePath { get; private set; }
      public bool IsLooping
      {
         get { return !IsNotLoaded() && (CurrentType == Type.Sound ? sound.Loop : music.Loop); }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Loop = value;
            else music.Loop = value;
         }
      }
      public bool IsPaused 
      {
         get
         { return !IsNotLoaded() &&
                (CurrentType == Type.Sound ? sound.Status == SoundStatus.Paused : music.Status == SoundStatus.Paused); }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound)
            {
               if (value && IsPlaying) sound.Pause();
               else if (value == false && IsPaused) sound.Play();
            }
            else
            {
               if (value && IsPlaying) music.Pause();
               else if (value == false && IsPaused) music.Play();
            }
            if (value && IsPlaying) Events.Notify(Events.Type.AudioPause, new() { Audio = this });
            else if (value == false && IsPaused) Events.Notify(Events.Type.AudioPlay, new() { Audio = this });
         }
      }
      public bool IsPlaying
      {
         get
         { return !IsNotLoaded() &&
               (CurrentType == Type.Sound ? sound.Status == SoundStatus.Playing : music.Status == SoundStatus.Playing); }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound)
            {
               stopped = !value;
               if (value) sound.Play();
               else if (IsPlaying || IsPaused) sound.Stop();
            }
            else
            {
               stopped = !value;
               if (value) music.Play();
               else if (IsPlaying || IsPaused) music.Stop();
            }
            if (value)
            {
               if (IsPaused == false) Events.Notify(Events.Type.AudioStart, new() { Audio = this });
               Events.Notify(Events.Type.AudioPlay, new() { Audio = this });
            }
            else if ((IsPlaying || IsPaused)) Events.Notify(Events.Type.AudioStop, new() { Audio = this });
         }
      }
      public double Duration => IsNotLoaded() ? default : FileDuration / Speed;
      public double FileDuration => IsNotLoaded() ? default :
         CurrentType == Type.Sound ? sound.SoundBuffer.Duration.AsSeconds() : music.Duration.AsSeconds();
      public double VolumePercent
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Volume : music.Volume; }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Volume = (float)value;
            else music.Volume = (float)value;
         }
      }
      public double Progress
      {
         get { return IsNotLoaded() ? default : FileProgress / Speed; }
         set { if (ErrorIfDestroyed() == false) FileProgress = value * Speed; }
      }
      public double FileProgress
      {
         get { return IsNotLoaded() ? default :
               CurrentType == Type.Sound ? sound.PlayingOffset.AsSeconds() : music.PlayingOffset.AsSeconds(); }
         set
         {
            if (ErrorIfDestroyed()) return;
            var v = SFML.System.Time.FromSeconds((float)Number.Limit(value, new Number.Range(0, FileDuration)));
            if (CurrentType == Type.Sound) sound.PlayingOffset = v;
            else music.PlayingOffset = v;
            rewindIsNotLoop = true;
         }
      }
      public double ProgressPercent
      {
         get { return IsNotLoaded() ? default : 100 - Number.ToPercent(FileDuration - FileProgress,
            new Number.Range(0, FileDuration)); }
         set { if (ErrorIfDestroyed() == false)  FileProgress = Number.FromPercent(value, new Number.Range(0, FileDuration)); }
      }
      public double Speed
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Pitch : music.Pitch; }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Pitch = (float)value;
            else music.Pitch = (float)value;
         }
      }
      public double DistanceFade
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Attenuation : music.Attenuation; }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Attenuation = (float)value;
            else music.Attenuation = (float)value;
         }
      }
      public Point Position
      {
         get
         {
				return IsNotLoaded() ? default :
               CurrentType == Type.Sound ?
               new Point(sound.Position.X, sound.Position.Z) :
               new Point(music.Position.X, music.Position.Z);
			}
			set
         {
            if (ErrorIfDestroyed() || IsMono()) return;
            if (CurrentType == Type.Sound) sound.Position = new Vector3f((float)value.X, 0, (float)value.Y);
				else music.Position = new Vector3f((float)value.X, 0, (float)value.Y);
         }
      }
      public Type CurrentType { get; private set; }

      public Audio(string uniqueID, string audioPath = "folder/audio.extension") : base(uniqueID)
      {
         if (Assets.sounds.ContainsKey(audioPath))
         {
            sound = Assets.sounds[audioPath];
            sound.RelativeToListener = false;
            sound.Attenuation = 0;
            CurrentType = Type.Sound;
         }
			else if (Assets.music.ContainsKey(audioPath))
			{
            music = Assets.music[audioPath];
            music.RelativeToListener = false;
            music.Attenuation = 0;
            CurrentType = Type.Music;
         }
			else { IsNotLoaded(); return; }

         audios.Add(this);
         FilePath = audioPath;
         VolumePercent = 50;
         if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
      }
		public override void Destroy()
		{
         if (ErrorIfDestroyed()) return;
         audios.Remove(this);
         if (sound != null) sound.Dispose();
         if (music != null) music.Dispose();
         sound = null;
         music = null;
         base.Destroy();
		}
   }
}
