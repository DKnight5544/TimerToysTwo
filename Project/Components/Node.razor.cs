using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http; // for HttpClient
using System.Net.Http.Json; // for HttpClientJsonExtensions
using System.Threading.Tasks;
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

        [Microsoft.AspNetCore.Components.Parameter]
        public EventCallback OnSelectHandler { get; set; }

        public string HotOrNotClass
        {
            get
            {
                return (MyTimer.ChildCount > 0) ? "hot" : "not";
            }
        }

        public string IsExpandedStyle {
            get
            {
                return (IsExpanded) ? "block" : "none";
            }
        }

        private bool IsExpanded { get; set; }

        private Timer[] MyChildTimers;

        private Dictionary<string, Node> MyChildNodes = new Dictionary<string, Node>();

        public string ElapsedTimeString
        {
            get
            {
                //int et = GetElapsedTime();
                return Global.StringifyTime(MyTimer.ElapsedTime);
            }
        }

        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }

        protected override async Task OnInitializedAsync()
        {
            IsExpanded = true;
            if (MyTimer.ChildCount > 0)
            {
                MyChildTimers = await Http.GetFromJsonAsync<Timer[]>(string.Format("GetChildren/{0}", MyTimer.TimerKey));
                StateHasChanged();
            }
        }

        public int Tick()
        {
            int saveTime = MyTimer.ElapsedTime;

            if (MyTimer.IsRunning) MyTimer.ElapsedTime++;

            if (MyChildTimers != null)
            {
                MyTimer.ElapsedTime = 0;
                foreach (Timer child in MyChildTimers.OrderBy(c => c.CreationTime))
                {
                    bool nodeExists = MyChildNodes.Keys.Contains(child.TimerKey);
                    if (nodeExists) MyTimer.ElapsedTime += MyChildNodes[child.TimerKey].Tick();
                }
            }

            if (MyTimer.ElapsedTime != saveTime) StateHasChanged();

            return MyTimer.ElapsedTime;

        }

        private void OnSelect()
        {
            Global.SelectedTimer = MyTimer;
            OnSelectHandler.InvokeAsync();
        }

        private void Node_onClick()
        {
            IsExpanded = !IsExpanded;
        }



    }
}
