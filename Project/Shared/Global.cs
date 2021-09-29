using System;
using System.Collections.Generic;
using TimerToysTwo.Components;

namespace TimerToysTwo.Shared
{
    public static class Global
    {
        public static TimerToysTwo.Model.Timer SelectedTimer { get; set; }

        public static string StringifyTime(int seconds)
        {

            int sec = seconds;
            int hrs = sec / 3600;

            sec -= (hrs * 3600);

            int min = sec / 60;

            sec -= (min * 60);

            //if (seconds >= 0 && isCountDownTimer)
            //{
            //    if (myTimer.IsRunning) ToggleTimer();
            //}
            string result;

            if (hrs > 0) result = string.Format("{0:00}:{1:00}:{2:00}", Math.Abs(hrs), Math.Abs(min), Math.Abs(sec));
            else if (min > 0) result = string.Format("{0:00}:{1:00}", Math.Abs(min), Math.Abs(sec));
            else result = string.Format("{0}", Math.Abs(sec));

            return result;

        }
        public static string StringifyTime()
        {
            return StringifyTime(SelectedTimer.ElapsedTime);
        }

    }
}
