using CtlLibraryBindings;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class DeviceTemp : IDisposable
    {
        private readonly SWIGTYPE_p__ctl_temp_handle_t _handle;
        private readonly SWIGTYPE_p_double doublePtr;

        public DeviceTemp(SWIGTYPE_p__ctl_temp_handle_t handle)
        {
            using (var properties = new ctl_temp_properties_t())
            {
                CtlLibrary.ctlTemperatureGetProperties(handle, properties);
                Name = properties.type.ToString();
            }

            _handle = handle;
            doublePtr = CtlLibrary.new_double_Ptr();
        }

        public string Name { get; }

        public double GetValue()
        {
            CtlLibrary.ctlTemperatureGetState(_handle, doublePtr);
            return CtlLibrary.double_Ptr_value(doublePtr);
        }

        public static DeviceTemp[] GetDeviceTemps(SWIGTYPE_p__ctl_device_adapter_handle_t handle)
        {
            using (var disposable = new CompositeDisposable())
            {
                var uintPtr = CtlLibrary.new_unsigned_int_Ptr().DisposeWith(disposable, CtlLibrary.delete_unsigned_int_Ptr);
                var tempArrayPtr = CtlLibrary.new_ctl_temp_handle_t_PtrPtr().DisposeWith(disposable, CtlLibrary.delete_ctl_temp_handle_t_PtrPtr);
                CtlLibrary.ctlEnumTemperatureSensors(handle, uintPtr, tempArrayPtr).ThrowIfError("Enumerate temperature sensors");

                var tempArray = TempHandleArray.frompointer(tempArrayPtr);
                var tempHandles = Enumerable.Range(0, Convert.ToInt32(CtlLibrary.unsigned_int_Ptr_value(uintPtr))).Select(tempArray.getitem).ToArray();
                return tempHandles.Select(x => new DeviceTemp(x)).ToArray();
            }

        }

        public void Dispose()
        {
            CtlLibrary.delete_double_Ptr(doublePtr);
        }
    }
}
