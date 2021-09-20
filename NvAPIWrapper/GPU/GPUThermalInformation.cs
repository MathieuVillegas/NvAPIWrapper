using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Holds information regarding the available thermal sensors and current thermal level of a GPU
    /// </summary>
    public class GPUThermalInformation
    {
        internal GPUThermalInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets the current thermal level of the GPU
        /// </summary>
        public int CurrentThermalLevel
        {
            get => (int) GPUApi.GetCurrentThermalLevel(PhysicalGPU.Handle);
        }


        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <summary>
        ///     Gets the list of available thermal sensors
        /// </summary>
        public IEnumerable<GPUThermalSensor> ThermalSensors
        {
            get
            {
                var data = GPUApi.GetThermalSettings(PhysicalGPU.Handle).Sensors
                    .Select((sensor, i) => new GPUThermalSensor(i, sensor));
                List<GPUThermalSensor> list = new List<GPUThermalSensor>();

                try
                {
                    var temp = GPUApi.QueryThermalSensors(PhysicalGPU.Handle, (1u << 1) | (1u << 9));
                    var junction = temp.Temperatures[9];
                    if (junction != 0)
                    {
                        var s = new SimpleThermalSensor();
                        s.Target = ThermalSettingsTarget.MemoryJunction;
                        s.CurrentTemperature = (int)junction;
                        list.Add(new GPUThermalSensor((int)ThermalSettingsTarget.MemoryJunction, s));
                    }
                    var hotspot = temp.Temperatures[1];
                    if (hotspot != 0)
                    {
                        var s = new SimpleThermalSensor();
                        s.Target = ThermalSettingsTarget.Hotspot;
                        s.CurrentTemperature = (int)hotspot;
                        list.Add(new GPUThermalSensor((int)ThermalSettingsTarget.Hotspot, s));
                    }
                }
                catch
                {
                   // nothing to do
                }


               
               
                return Enumerable.Concat(data, list);
            }
        }


        private class SimpleThermalSensor : IThermalSensor
        {

            public ThermalController Controller => new ThermalController();

            public int CurrentTemperature { get; set; }

            public int DefaultMaximumTemperature => Int32.MaxValue;

            public int DefaultMinimumTemperature => Int32.MinValue;

            public ThermalSettingsTarget Target { get; set; }
        }
    }
}