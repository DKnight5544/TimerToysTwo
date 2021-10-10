
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


        [Microsoft.AspNetCore.Components.Parameter]
        public Timer MyTimer { get; set; }

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
                    RenameParms parms = new();
                    parms.TimerKey = MyTimer.TimerKey;
                    parms.NewName = value;
                    var _1 = RenameTimer(parms);
                }
            }
        }

        private bool IsDisabled
        {
            get
            {
                return (MyTimer.IsReadOnly || MyTimer.ChildCount > 0);
            }
        }

        private ElapsedTimeComponent ETC;


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

        private readonly Dictionary<string, Node> MyChildNodes = new();


        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        protected override async Task OnInitializedAsync()
        {
            IsExpanded = false;
            ShowControls = false;
            ETC = new();

            if (MyTimer.ChildCount > 0)
            {
                MyChildTimers = await Http.GetFromJsonAsync<Timer[]>(string.Format("GetChildren/{0}", MyTimer.TimerKey));
                StateHasChanged();
            }
        }

        public int Tick()
        {
            int saveTime = ETC.ElapsedTime;
            int elapsedTime = ETC.ElapsedTime;

            if (MyTimer.IsRunning) elapsedTime++;

            if (MyChildTimers != null)
            {
                elapsedTime = 0;
                foreach (Timer child in MyChildTimers)
                {
                    elapsedTime += MyChildNodes[child.TimerKey].Tick();
                }
            }

            if (elapsedTime != saveTime) ETC.ElapsedTime = elapsedTime;

            return elapsedTime;

            //return 0;

        }

        private async Task ToggleTimer()
        {
            if (!IsDisabled)
            {
                Timer tmr = await Http.GetFromJsonAsync<Timer>(string.Format("ToggleTimer/{0}", MyTimer.TimerKey));
                MyTimer = tmr;
                ETC.ElapsedTime = MyTimer.ElapsedTime;
            }
        }
        private async Task RenameTimer(RenameParms parms)
        {
            var response = await Http.PostAsJsonAsync("RenameTimer", parms);
            MyTimer = await response.Content.ReadFromJsonAsync<Timer>();
            StateHasChanged();
        }

        private async Task AdjustTimer(int seconds)
        {
            if (!IsDisabled)
            {
                AdjustParms parms = new();
                parms.TimerKey = MyTimer.TimerKey;
                parms.SecondsOffset = seconds;
                var response = await Http.PostAsJsonAsync("AdjustTimer", parms);
                MyTimer = await response.Content.ReadFromJsonAsync<Timer>();
                ETC.ElapsedTime = MyTimer.ElapsedTime;
            }

        }
        private async Task ResetTimer()
        {
            if (!IsDisabled)
            {
                Timer tmr = await Http.GetFromJsonAsync<Timer>(string.Format("ResetTimer/{0}", MyTimer.TimerKey));
                MyTimer = tmr;
                ETC.ElapsedTime = MyTimer.ElapsedTime;
            }
        }
    }
}
