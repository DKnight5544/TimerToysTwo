﻿using Microsoft.AspNetCore.Components;
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
    public partial class SelectedNode
    {

        [Parameter]
        public EventCallback OnNameChange { get; set; }

        private string SelectedTimerName
        {
            get
            {
                if (Global.SelectedTimer is null) return "";
                else return Global.SelectedTimer.TimerName;
            }
        }

        private string SelectedElapsedTime
        {
            get
            {
                if (Global.SelectedTimer is null) return "";
                else return Global.StringifyTime();
            }
        }

        private string SavedTime = "";

        public void Tick()
        {
            if (SavedTime != SelectedElapsedTime)
            {
                StateHasChanged();
                SavedTime = SelectedElapsedTime;
            }
        }

        public void UpdateDisplay()
        {
            StateHasChanged();
        }

        private void ToggleTimer()
        {
            Global.SelectedTimer.IsRunning = !Global.SelectedTimer.IsRunning;
        }

        private void UpdateName()
        {
            Global.SelectedTimer.TimerName = "Dave Is Cool!";
            StateHasChanged();
            OnNameChange.InvokeAsync();
        }
    }
}
