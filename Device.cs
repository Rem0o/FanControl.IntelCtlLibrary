using CtlLibraryBindings;
using System;
using System.Linq;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class Device : IDisposable
    {
        private readonly DeviceFan[] _fans;
        private readonly DeviceTemp[] _temps;
        private readonly CompositeDisposable _disposable;

        public static Device[] GetDevices(SWIGTYPE_p__ctl_api_handle_t apiHandle)
        {
            using (var disposable = new CompositeDisposable())
            {
                var uintPtr = CtlLibrary.new_unsigned_int_Ptr().DisposeWith(disposable, CtlLibrary.delete_unsigned_int_Ptr);
                var _ = CtlLibrary.new_ctl_device_adapter_handle_t_PtrPtr().DisposeWith(disposable, CtlLibrary.delete_ctl_device_adapter_handle_t_PtrPtr);
                CtlLibrary.ctlEnumerateDevices(apiHandle, uintPtr, _).ThrowIfError("Enumerate devices");
                var n = Convert.ToInt32(CtlLibrary.unsigned_int_Ptr_value(uintPtr));
                var array = new DeviceAdapterHandleArray(n).DisposeWith(disposable);
                CtlLibrary.ctlEnumerateDevices(apiHandle, uintPtr, array.cast()).ThrowIfError("Enumerate devices");

                return Enumerable.Range(0, n).Select(array.getitem).Select((x, i) => new Device(x, i)).ToArray(); ;
            }
        }

        private static SWIGTYPE_p__ctl_fan_handle_t[] GetFanHandles(SWIGTYPE_p__ctl_device_adapter_handle_t deviceHandle)
        {
            using (var disposable = new CompositeDisposable())
            {
                var uintPtr = CtlLibrary.new_unsigned_int_Ptr();
                var _ = CtlLibrary.new_ctl_fan_handle_t_PtrPtr().DisposeWith(disposable, CtlLibrary.delete_ctl_fan_handle_t_PtrPtr);
                CtlLibrary.ctlEnumFans(deviceHandle, uintPtr, _).ThrowIfError("Enum fans");
                int n = Convert.ToInt32(CtlLibrary.unsigned_int_Ptr_value(uintPtr));
                var fanArray = new FanHandleArray(n);
                CtlLibrary.ctlEnumFans(deviceHandle, uintPtr, fanArray.cast()).ThrowIfError("Enum fans");

                return Enumerable.Range(0, n).Select(fanArray.getitem).ToArray();
            }
        }

        public static SWIGTYPE_p__ctl_temp_handle_t[] GetTempHandles(SWIGTYPE_p__ctl_device_adapter_handle_t handle)
        {
            using (var disposable = new CompositeDisposable())
            {
                var uintPtr = CtlLibrary.new_unsigned_int_Ptr().DisposeWith(disposable, CtlLibrary.delete_unsigned_int_Ptr);
                var _ = CtlLibrary.new_ctl_temp_handle_t_PtrPtr().DisposeWith(disposable, CtlLibrary.delete_ctl_temp_handle_t_PtrPtr);
                CtlLibrary.ctlEnumTemperatureSensors(handle, uintPtr, _).ThrowIfError("Enumerate temperature sensors");
                int n = Convert.ToInt32(CtlLibrary.unsigned_int_Ptr_value(uintPtr));
                var tempArray = new TempHandleArray(n);
                CtlLibrary.ctlEnumTemperatureSensors(handle, uintPtr, tempArray.cast()).ThrowIfError("Enumerate temperature sensors");

                return Enumerable.Range(0, n).Select(tempArray.getitem).ToArray();
            }
        }


        private Device(SWIGTYPE_p__ctl_device_adapter_handle_t handle, int index)
        {
            _disposable = new CompositeDisposable();
            _fans = GetFanHandles(handle).Select((x, i) => new DeviceFan(x, i)).ToArray();
            _temps = GetTempHandles(handle).Select((x) => new DeviceTemp(x)).ToArray();
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
