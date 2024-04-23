using CtlLibraryBindings;
using System;
using System.Linq;

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

        public void Dispose()
        {
            CtlLibrary.delete_double_Ptr(doublePtr);
        }
    }
}
