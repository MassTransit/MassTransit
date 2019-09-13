using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !NETCORE
using System.Diagnostics;
#endif

namespace MassTransit.Monitoring.Performance
{
    public class CounterData
    {
        //
        // Summary:
        //     Gets or sets the name of the custom counter.
        //
        // Returns:
        //     A name for the counter, which is unique in its category.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified value is null.
        //
        //   T:System.ArgumentException:
        //     The specified value is not between 1 and 80 characters long or contains double
        //     quotes, control characters or leading or trailing spaces.
        public string CounterName { get; set; }

        //
        // Summary:
        //     Gets or sets the custom counter's description.
        //
        // Returns:
        //     The text that describes the counter's behavior.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The specified value is null.
        public string CounterDescription { get; set; }

        //
        // Summary:
        //     Gets or sets the performance counter type of the custom counter.
        //
        // Returns:
        //     A System.Diagnostics.PerformanceCounterType that defines the behavior of the
        //     performance counter.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     You have specified a type that is not a member of the System.Diagnostics.PerformanceCounterType
        //     enumeration.
        public CounterType CounterType { get; set; }

#if !NETCORE
        public CounterCreationData ToCounterCreationData()
        {
            return new CounterCreationData(CounterName, CounterDescription, (PerformanceCounterType)CounterType);
        }
#endif
    }

    public enum CounterType
    {
        //
        // Summary:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of a very large number of items or operations.
        //     It is the same as NumberOfItems32 except that it uses larger fields to accommodate
        //     larger values.
        NumberOfItems64 = 65792,
        
        //
        // Summary:
        //     A difference counter that shows the average number of operations completed during
        //     each second of the sample interval. Counters of this type measure time in ticks
        //     of the system clock.
        RateOfCountsPerSecond32 = 272696320,

        //
        // Summary:
        //     An average counter that shows how many items are processed, on average, during
        //     an operation. Counters of this type display a ratio of the items processed to
        //     the number of operations completed. The ratio is calculated by comparing the
        //     number of items processed during the last interval to the number of operations
        //     completed during the last interval.
        AverageCount64 = 1073874176,
      
        //
        // Summary:
        //     A base counter that is used in the calculation of time or count averages, such
        //     as AverageTimer32 and AverageCount64. Stores the denominator for calculating
        //     a counter to present "time per operation" or "count per operation".
        AverageBase = 1073939458
    }
}
