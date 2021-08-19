﻿using System;

namespace SMPL
{
   public class ComponentTimer : ComponentAccess
   {
      private static event Events.ParamsOne<ComponentTimer> OnEnd, OnPause;
      private static event Events.ParamsTwo<ComponentTimer, int> OnEndCountChange;
      private static event Events.ParamsTwo<ComponentTimer, double> OnCreateAndStart, OnDurationChange, OnCountdownChange,
         OnProgressChange, OnProgressPercentChange;
      private static event Events.ParamsTwo<ComponentTimer, ComponentIdentity<ComponentTimer>> OnIdentityChange;

      public static class CallWhen
      {
         public static void CreateAndStart(Action<ComponentTimer, double> method, uint order = uint.MaxValue) =>
         OnCreateAndStart = Events.Add(OnCreateAndStart, method, order);
         public static void IdentityChange(Action<ComponentTimer, ComponentIdentity<ComponentTimer>> method,
            uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
         public static void End(Action<ComponentTimer> method, uint order = uint.MaxValue) =>
            OnEnd = Events.Add(OnEnd, method, order);
         public static void Pause(Action<ComponentTimer> method, uint order = uint.MaxValue) =>
            OnPause = Events.Add(OnPause, method, order);
         public static void EndCountChange(Action<ComponentTimer, int> method, uint order = uint.MaxValue) =>
            OnEndCountChange = Events.Add(OnEndCountChange, method, order);
         public static void DurationChange(Action<ComponentTimer, double> method, uint order = uint.MaxValue) =>
            OnDurationChange = Events.Add(OnDurationChange, method, order);
         public static void ProgressChange(Action<ComponentTimer, double> method, uint order = uint.MaxValue) =>
            OnProgressChange = Events.Add(OnProgressChange, method, order);
         public static void CountdownChange(Action<ComponentTimer, double> method, uint order = uint.MaxValue) =>
            OnCountdownChange = Events.Add(OnCountdownChange, method, order);
         public static void ProgressPercentChange(Action<ComponentTimer, double> method, uint order = uint.MaxValue) =>
            OnProgressPercentChange = Events.Add(OnProgressPercentChange, method, order);
      }

      private ComponentIdentity<ComponentTimer> identity;
      public ComponentIdentity<ComponentTimer> Identity
      {
         get { return identity; }
         set
         {
            if (identity == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var prev = identity;
            identity = value;
            OnIdentityChange?.Invoke(this, prev);
         }
      }

      private int endCount;
      public int EndCount
      {
         get { return endCount; }
         set
         {
            if (endCount == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var prev = endCount;
            endCount = value;
            if (Debug.CurrentMethodIsCalledByUser == false) return;
            OnEndCountChange?.Invoke(this, prev);
         }
      }
      public double Duration { get; private set; }
      private double progress;
      public double Progress
      {
         get { return progress; }
         set
         {
            if (progress == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var prevCd = Countdown;
            var prevPr = Progress;
            progress = value;
            countdown = Duration - value;
            var prevPrPer = ProgressPercent;
            if (Debug.CurrentMethodIsCalledByUser == false) return;
            OnCountdownChange?.Invoke(this, prevCd);
            OnProgressChange?.Invoke(this, prevPr);
            OnProgressPercentChange?.Invoke(this, prevPrPer);
         }
      }
      public double ProgressPercent
      {
         get { return Number.ToPercent(progress, new Bounds(0, Duration)); ; }
         set
         {
            value = Number.Limit(value, new Bounds(0, 100));
            var prPer = Number.FromPercent(value, new Bounds(0, Duration));
            if (prPer == progress || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var prevCd = Countdown;
            var prevPr = Progress;
            progress = prPer;
            var prevPrPer = ProgressPercent;
            if (Debug.CurrentMethodIsCalledByUser == false) return;
            OnCountdownChange?.Invoke(this, prevCd);
            OnProgressChange?.Invoke(this, prevPr);
            OnProgressPercentChange?.Invoke(this, prevPrPer);
         }
      }
      private double countdown;
      public double Countdown
      {
         get { return countdown; }
         set
         {
            if (countdown == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var prevCd = Countdown;
            var prevPr = Progress;
            countdown = value;
            progress = Duration - value;
            var prevPrPer = ProgressPercent;
            if (Debug.CurrentMethodIsCalledByUser == false) return;
            OnCountdownChange?.Invoke(this, prevCd);
            OnProgressChange?.Invoke(this, prevPr);
            OnProgressPercentChange?.Invoke(this, prevPrPer);
         }
      }
      private bool isPaused;
      public bool IsPaused
      {
         get { return isPaused; }
         set
         {
            if (isPaused == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            isPaused = value;
            if (Debug.CurrentMethodIsCalledByUser == false) return;
            OnPause?.Invoke(this);
         }
      }

      public ComponentTimer(string uniqueID, double durationInSeconds)
      {
			if (uniqueID == null)
			{
            Debug.LogError(1, "Timers' uniqueID cannot be 'null'.");
            return;
			}
         Identity = new(this, uniqueID);
         Duration = durationInSeconds;
         Countdown = Duration;
         OnCreateAndStart?.Invoke(this, Duration);
      }

      internal static void Update()
      {
         var timerUIDs = ComponentIdentity<ComponentTimer>.AllUniqueIDs;
         for (int j = 0; j < timerUIDs.Length; j++)
         {
            var dt = Performance.DeltaTime;
            var timer = ComponentIdentity<ComponentTimer>.PickByUniqueID(timerUIDs[j]);
            if (timer.Countdown < 0) timer.Countdown = 0;
            if (timer.IsPaused || timer.Countdown == 0) continue;
            var prevCd = timer.Countdown;
            var prevPr = timer.Progress;
            timer.Countdown -= dt;
            var prevPrPer = timer.ProgressPercent;
            OnCountdownChange?.Invoke(timer, prevCd);
            OnProgressChange?.Invoke(timer, prevPr);
            OnProgressPercentChange?.Invoke(timer, prevPrPer);
            if (Gate.EnterOnceWhile(timerUIDs[j] + "end-as;li3'f2", timer.Countdown <= 0) ||
               dt > timer.Duration)
            {
               timer.EndCount++;
               timer.Countdown = 0;
               OnEnd?.Invoke(timer);
            }
         }
      }
   }
}
