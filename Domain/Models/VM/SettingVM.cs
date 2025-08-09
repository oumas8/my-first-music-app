using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.VM
{
    public class SettingVM
    {
        public bool playOne { get; set; } = false;
        public bool playInLoop { get; set; } = false;
        public bool maxPlayEnable { get; set; } = false;
        private int _maxPlay;
        public int maxPlay
        {
            get => _maxPlay;
            set => _maxPlay = value < 0 ? 0 : value;
        }

        public bool stopTimeEnable { get; set; } = false;
        private int _stopTime;
        public int stopTime
        {
            get => _stopTime;
            set => _stopTime = value < 0 ? 0 : value;
        }
    }
}
