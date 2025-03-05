using CtlLibraryBindings;
using CtlLibraryWrapper;
using System;
using System.Linq;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class Device : IDisposable
    {
        private readonly DeviceFan[] _fans;
        private readonly DeviceTemp[] _temps;
        private readonly CompositeDisposable _disposable;

        public static Device[] GetDevices(SWIGTYPE_p__ctl_api_handle_t apiHandle, CompositeDisposable disposable)
        {
            return CtlLibraryHelpers.GetDevices(apiHandle)
                .DisposeWith(disposable)
                .GetItems((x, i) => new Device(x.getitem(i), i))
                .ToArray();
        }

        private static SWIGTYPE_p__ctl_fan_handle_t[] GetFanHandles(SWIGTYPE_p__ctl_device_adapter_handle_t handle, CompositeDisposable disposable)
        {
            return CtlLibraryHelpers.GetFanHandles(handle)
                .DisposeWith(disposable)
                .GetItems((x, i) => x.getitem(i))
                .ToArray();
        }

        private static SWIGTYPE_p__ctl_temp_handle_t[] GetTempHandles(SWIGTYPE_p__ctl_device_adapter_handle_t handle, CompositeDisposable disposable)
        {
            return CtlLibraryHelpers.GetTempHandles(handle)
                .DisposeWith(disposable)
                .GetItems((x, i) => x.getitem(i))
                .ToArray();
        }

        private Device(SWIGTYPE_p__ctl_device_adapter_handle_t handle, int index)
        {
            _disposable = new CompositeDisposable();
            _fans = GetFanHandles(handle, _disposable).Select((x, i) => new DeviceFan(x, i)).ToArray();
            _temps = GetTempHandles(handle, _disposable).Select((x) => new DeviceTemp(x)).ToArray();
            ctl_device_adapter_properties_t properties = new ctl_device_adapter_properties_t().DisposeWith(_disposable);
            CtlLibrary.ctlGetDeviceProperties(handle, properties).ThrowIfError("Get device properties");
            Name = properties.name;
            Index = index;
        }

        public string Name { get; }

        public DeviceFan[] Fans => _fans;

        public DeviceTemp[] Temps => _temps;

        public int Index { get; }

        public void Dispose()
        {
            foreach (var fan in _fans)
            {
                fan.Dispose();
            }

            foreach (var temp in _temps)
            {
                temp.Dispose();
            }

            _disposable.Dispose();
        }
    }
}
