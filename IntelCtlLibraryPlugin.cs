using CtlLibraryBindings;
using FanControl.Plugins;
using System.Linq;

namespace FanControl.IntelCtlLibraryPlugin
{
    public class IntelCtlLibraryPlugin : IPlugin2
    {
        private CompositeDisposable? _disposable;
        private SWIGTYPE_p__ctl_api_handle_t? _apiHandle;
        private readonly IPluginLogger _logger;

        public string Name => "IntelCtlLibrary";

        public IntelCtlLibraryPlugin(IPluginLogger logger)
        {
            _logger = logger;
        }

        public void Close()
        {
            if (_apiHandle != null)
            {
                CtlLibrary.ctlClose(_apiHandle).ThrowIfError("Close");
                _apiHandle = null;
            }

            _disposable?.Dispose();
            _disposable = null;
        }

        public void Initialize()
        {
            _disposable = new CompositeDisposable();
            ctl_init_args_t initArgs = CtlLibrary.create_Init_Args().DisposeWith(_disposable);
            var handlePtr = CtlLibrary.new_ctl_api_handle_t_Ptr().DisposeWith(_disposable, CtlLibrary.delete_ctl_api_handle_t_Ptr);

            ctl_result_t initResult = CtlLibrary.ctlInit(initArgs, handlePtr);
            if (initResult == ctl_result_t.CTL_RESULT_SUCCESS)
            {
                _apiHandle = CtlLibrary.ctl_api_handle_t_Ptr_value(handlePtr);
            }
            else
            {
                _logger.Log($"Could not init CtlLibrary: {initResult}");
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
