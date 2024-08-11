using CtlLibraryBindings;
using System;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class DeviceFan : IDisposable
    {
        private readonly SWIGTYPE_p__ctl_fan_handle_t _fanHandle;
        private readonly SWIGTYPE_p_int _speedRequestPtr;
        private readonly ctl_fan_speed_t _speed;
        private readonly ctl_fan_speed_table_t _table;

        public DeviceFan(SWIGTYPE_p__ctl_fan_handle_t fanHandle, int index)
        {
            _fanHandle = fanHandle;
            Index = index;
            _speedRequestPtr = CtlLibrary.new_int_Ptr();
            _speed = new ctl_fan_speed_t
            {
                units = ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_PERCENT,
                // version?
            };

            _table = new ctl_fan_speed_table_t();
        }

        public int Index { get; }

        public void Dispose()
        {
            Reset();
            CtlLibrary.delete_int_Ptr(_speedRequestPtr);
            _table.Dispose();
            _speed.Dispose();
        }

        public void Reset()
        {
            CtlLibrary.ctlFanSetDefaultMode(_fanHandle);
        }

        public int GetSpeedPercent()
        {
            CtlLibrary.ctlFanGetState(_fanHandle, ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_PERCENT, _speedRequestPtr).ThrowIfError("Get fan % speed");
            return CtlLibrary.int_Ptr_value(_speedRequestPtr);
        }

        public int GetSpeedRpm()
        {
            CtlLibrary.ctlFanGetState(_fanHandle, ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_RPM, _speedRequestPtr).ThrowIfError("Get fan RPM speed");
            return CtlLibrary.int_Ptr_value(_speedRequestPtr);
        }

        public void SetFanSpeedPercent(int percent)
        {
            _speed.speed = percent;
            CtlLibrary.ctlFanSetFixedSpeedMode(_fanHandle, _speed).ThrowIfError($"Setting fan speed to {percent}");
        }

        public void SetFlatFanSpeedTable(int percent)
        {
            CtlLibrary.SetFlatFanSpeedTable(_fanHandle, _table, percent).ThrowIfError($"Setting flat table fan speed to {percent}");
        }
    }
}
