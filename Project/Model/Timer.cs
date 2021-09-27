
using System;

namespace TimerToysTwo.Model
{
	public class Timer
	{
		public string TimerKey { get; set; }
		public string ReadOnlyKey { get; set; }
		public string TimerName { get; set; }
		public bool IsRunning { get; set; }
		public int ChildCount { get; set; }
        public DateTime CreationTime { get; set; }
		public int ElapsedTime { get; set; }
        public bool IsReadOnly { get; set; }
	}
}
