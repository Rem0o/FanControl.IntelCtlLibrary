using FanControl.Plugins;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class DeviceTemperaturePluginSensor : IPluginSensor
    {
        private readonly Device _device;
        private readonly DeviceTemp _deviceTemp;

        public DeviceTemperaturePluginSensor(Device device, DeviceTemp deviceTemp)
        {
            _device = device;
            _deviceTemp = deviceTemp;
        }

        public string Id => $"IntelCtlLibrary/{_device.Index}/temp/{_deviceTemp.Name}";

        public string Name => $"{_device.Name} - {_deviceTemp.Name}";

        public float? Value { get; private set; }

        public void Update()
        {
            Value = Convert.ToSingle(_deviceTemp.GetValue());
        }
    }
}
