using FanControl.Plugins;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class DeviceFanPluginSensor : IPluginSensor
    {
        private Device _device;
        private DeviceFan _fan;

        public DeviceFanPluginSensor(Device device, DeviceFan fan)
        {
            _device = device;
            _fan = fan;
        }

        public string Id => $"IntelCtlLibrary/{_device.Index}/fan/{_fan.Index}";

        public string Name => $"{_device.Name} - Fan Control {_fan.Index + 1}";

        public float? Value { get; private set; }

        public void Update()
        {
            Value = _fan.GetSpeedRpm();
        }
    }
}
