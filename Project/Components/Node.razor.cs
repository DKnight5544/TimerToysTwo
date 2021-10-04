
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http; // for HttpClient
using System.Net.Http.Json; // for HttpClientJsonExtensions
using TimerToysTwo.Model;
using TimerToysTwo.Shared;



namespace TimerToysTwo.Components
{
    public partial class Node
    {
        [Microsoft.AspNetCore.Components.Inject]
        HttpClient Http { get; set; }

        private Timer _myTimer;

        [Microsoft.AspNetCore.Components.Parameter]
        public Timer MyTimer {
            get { return _myTimer; }
            set { _myTimer = value; }
        }

        private string ExpandButtonClass
        {
            get
            {
                return MyTimer.ChildCount > 0 ? "expand" : "no-expand";
            }
        }

        private string ToggleIconCode
        {
            get
            {
                string code = (IsExpanded) ? "remove"
                    : (MyTimer.ChildCount > 0) ? "add"
                    : "remove";
                return code;
            }
        }


        private string TimerName
        {
            get { return MyTimer.TimerName; }
            set
            {
                if (MyTimer.TimerName != value && !string.IsNullOrWhiteSpace(value))
                {
                    MyTimer.TimerName = value;
                    RenameParms parms = new RenameParms();
                    parms.TimerKey = MyTimer.TimerKey;
                    parms.NewName = value;
                    RenameTimer(parms);
                }
            }
        }
        private string ElapsedTimeString
        {
            get
            {
                return StringifyTime(ElapsedTime);
            }
        }

        protected int ElapsedTime { get; set; }
        private async Task RenameTimer(RenameParms parms)
        {
            var response = await Http.PostAsJsonAsync("RenameTimer", parms);
            MyTimer = await response.Content.ReadFromJsonAsync<Timer>();
            StateHasChanged();
        }

        private bool IsExpanded { get; set; }
        private string IsExpandedStyle
        {
            get
            {
                return (IsExpanded) ? "block" : "none";
            }
        }

        private bool ShowControls { get; set; }
        private string ShowControlsStyle
        {
            get
            {
                return (ShowControls) ? "block" : "none";
            }
        }
        private void ToggleShowControls()
        {
            ShowControls = !ShowControls;
        }

        private Timer[] MyChildTimers;

        private Dictionary<string, Node> MyChildNodes = new Dictionary<string, Node>();


        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        protected override async Task OnInitializedAsync()
        {
            IsExpanded = false;
            ShowControls = false;
            ElapsedTime = MyTimer.ElapsedTime;

            if (MyTimer.ChildCount > 0)
            {
                MyChildTimers = await Http.GetFromJsonAsync<Timer[]>(string.Format("GetChildren/{0}", MyTimer.TimerKey));
                StateHasChanged();
            }
        }

        public int Tick()
        {
            int saveTime = ElapsedTime;

            if (MyTimer.IsRunning) ElapsedTime++;

            if (MyChildTimers != null)
            {
                ElapsedTime = 0;
                foreach (Timer child in MyChildTimers)
                {
                    ElapsedTime += MyChildNodes[child.TimerKey].Tick();
                }
            }

            if (ElapsedTime != saveTime) StateHasChanged();

            return ElapsedTime;

            //return 0;

        }

        private async Task ToggleTimer()
        {
            if (!MyTimer.IsReadOnly)
            {
                Timer tmr = await Http.GetFromJsonAsync<Timer>(string.Format("ToggleTimer/{0}", MyTimer.TimerKey));
                MyTimer = tmr;
                ElapsedTime = MyTimer.ElapsedTime;
                StateHasChanged();
            }
        }


        private string StringifyTime(int seconds)
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
            else result = string.Format("{0:00}", Math.Abs(sec));

            return result;

        }
    }
}
