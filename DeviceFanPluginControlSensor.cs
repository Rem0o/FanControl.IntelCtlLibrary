using FanControl.Plugins;
using System;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class DeviceFanPluginControlSensor : IPluginControlSensor
    {
        private readonly Device _device;
        private readonly DeviceFan _fan;

        public DeviceFanPluginControlSensor(Device device, DeviceFan fan)
        {
            _device = device;
            _fan = fan;
        }

        public string Id => $"IntelCtlLibrary/{_device.Index}/control/{_fan.Index}";

        public string Name => $"{_device.Name} - Fan {_fan.Index + 1}";

        public float? Value { get; private set; }

        public void Reset()
        {
            _fan.Reset();
        }

        public void Set(float val)
        {
            var roundedVal = (int)Math.Round(val);

            if (roundedVal == Value)
                return;

            _fan.SetFlatFanSpeedTable(roundedVal);
            Value = roundedVal;
        }

        public void Update()
        {
            //Value = _fan.GetSpeedPercent();
        }
    }
}
