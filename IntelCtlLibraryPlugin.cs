using CtlLibraryBindings;
using FanControl.Plugins;
using System.Linq;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class IntelCtlLibraryPlugin : IPlugin2
    {
        private CompositeDisposable? _disposable;
        private SWIGTYPE_p__ctl_api_handle_t? _apiHandle;

        public string Name => "IntelCtlLibrary";

        public void Close()
        {
            _disposable?.Dispose();
            _disposable = null;
            CtlLibrary.ctlClose(_apiHandle).ThrowIfError("Close");
        }

        public void Initialize()
        {
            _disposable = new CompositeDisposable();
            ctl_init_args_t initArgs = CtlLibrary.create_Init_Args().DisposeWith(_disposable);
            var handlePtr = CtlLibrary.new_ctl_api_handle_t_PtrPtr().DisposeWith(_disposable, CtlLibrary.delete_ctl_api_handle_t_PtrPtr);

            if (CtlLibrary.ctlInit(initArgs, handlePtr) == ctl_result_t.CTL_RESULT_SUCCESS)
            {
                _apiHandle = CtlLibrary.ctl_api_handle_t_PtrPtr_value(handlePtr);
            }

        }

        public void Load(IPluginSensorsContainer _container)
        {
            if (_apiHandle == null)
            {
                return;
            }


            var devices = Device.GetDevices(_apiHandle);

            foreach (var device in devices)
            {
                var controls = device.Fans.Select(f => new DeviceFanPluginControlSensor(device, f));
                var speeds = device.Fans.Select(f => new DeviceFanPluginSensor(device, f));
                var tempSensors = device.Temps.Select(t => new DeviceTemperaturePluginSensor(device, t));

                _container.ControlSensors.AddRange(controls);
                _container.FanSensors.AddRange(speeds);
                _container.TempSensors.AddRange(tempSensors);
            }
        }

        public void Update()
        {
            // empty
        }
    }
}
