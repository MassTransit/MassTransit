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
        //     An instantaneous counter that shows the most recently observed value in hexadecimal
        //     format. Used, for example, to maintain a simple count of items or operations.
        NumberOfItemsHEX32 = 0,
        //
        // Summary:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of a very large number of items or operations.
        //     It is the same as NumberOfItemsHEX32 except that it uses larger fields to accommodate
        //     larger values.
        NumberOfItemsHEX64 = 256,
        //
        // Summary:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of items or operations.
        NumberOfItems32 = 65536,
        //
        // Summary:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of a very large number of items or operations.
        //     It is the same as NumberOfItems32 except that it uses larger fields to accommodate
        //     larger values.
        NumberOfItems64 = 65792,
        //
        // Summary:
        //     A difference counter that shows the change in the measured attribute between
        //     the two most recent sample intervals.
        CounterDelta32 = 4195328,
        //
        // Summary:
        //     A difference counter that shows the change in the measured attribute between
        //     the two most recent sample intervals. It is the same as the CounterDelta32 counter
        //     type except that is uses larger fields to accomodate larger values.
        CounterDelta64 = 4195584,
        //
        // Summary:
        //     An average counter that shows the average number of operations completed in one
        //     second. When a counter of this type samples the data, each sampling interrupt
        //     returns one or zero. The counter data is the number of ones that were sampled.
        //     It measures time in units of ticks of the system performance timer.
        SampleCounter = 4260864,
        //
        // Summary:
        //     An average counter designed to monitor the average length of a queue to a resource
        //     over time. It shows the difference between the queue lengths observed during
        //     the last two sample intervals divided by the duration of the interval. This type
        //     of counter is typically used to track the number of items that are queued or
        //     waiting.
        CountPerTimeInterval32 = 4523008,
        //
        // Summary:
        //     An average counter that monitors the average length of a queue to a resource
        //     over time. Counters of this type display the difference between the queue lengths
        //     observed during the last two sample intervals, divided by the duration of the
        //     interval. This counter type is the same as CountPerTimeInterval32 except that
        //     it uses larger fields to accommodate larger values. This type of counter is typically
        //     used to track a high-volume or very large number of items that are queued or
        //     waiting.
        CountPerTimeInterval64 = 4523264,
        //
        // Summary:
        //     A difference counter that shows the average number of operations completed during
        //     each second of the sample interval. Counters of this type measure time in ticks
        //     of the system clock.
        RateOfCountsPerSecond32 = 272696320,
        //
        // Summary:
        //     A difference counter that shows the average number of operations completed during
        //     each second of the sample interval. Counters of this type measure time in ticks
        //     of the system clock. This counter type is the same as the RateOfCountsPerSecond32
        //     type, but it uses larger fields to accommodate larger values to track a high-volume
        //     number of items or operations per second, such as a byte-transmission rate.
        RateOfCountsPerSecond64 = 272696576,
        //
        // Summary:
        //     An instantaneous percentage counter that shows the ratio of a subset to its set
        //     as a percentage. For example, it compares the number of bytes in use on a disk
        //     to the total number of bytes on the disk. Counters of this type display the current
        //     percentage only, not an average over time.
        RawFraction = 537003008,
        //
        // Summary:
        //     A percentage counter that shows the average time that a component is active as
        //     a percentage of the total sample time.
        CounterTimer = 541132032,
        //
        // Summary:
        //     A percentage counter that shows the active time of a component as a percentage
        //     of the total elapsed time of the sample interval. It measures time in units of
        //     100 nanoseconds (ns). Counters of this type are designed to measure the activity
        //     of one component at a time.
        Timer100Ns = 542180608,
        //
        // Summary:
        //     A percentage counter that shows the average ratio of hits to all operations during
        //     the last two sample intervals.
        SampleFraction = 549585920,
        //
        // Summary:
        //     A percentage counter that displays the average percentage of active time observed
        //     during sample interval. The value of these counters is calculated by monitoring
        //     the percentage of time that the service was inactive and then subtracting that
        //     value from 100 percent.
        CounterTimerInverse = 557909248,
        //
        // Summary:
        //     A percentage counter that shows the average percentage of active time observed
        //     during the sample interval.
        Timer100NsInverse = 558957824,
        //
        // Summary:
        //     A percentage counter that displays the active time of one or more components
        //     as a percentage of the total time of the sample interval. Because the numerator
        //     records the active time of components operating simultaneously, the resulting
        //     percentage can exceed 100 percent.
        CounterMultiTimer = 574686464,
        //
        // Summary:
        //     A percentage counter that shows the active time of one or more components as
        //     a percentage of the total time of the sample interval. It measures time in 100
        //     nanosecond (ns) units.
        CounterMultiTimer100Ns = 575735040,
        //
        // Summary:
        //     A percentage counter that shows the active time of one or more components as
        //     a percentage of the total time of the sample interval. It derives the active
        //     time by measuring the time that the components were not active and subtracting
        //     the result from 100 percent by the number of objects monitored.
        CounterMultiTimerInverse = 591463680,
        //
        // Summary:
        //     A percentage counter that shows the active time of one or more components as
        //     a percentage of the total time of the sample interval. Counters of this type
        //     measure time in 100 nanosecond (ns) units. They derive the active time by measuring
        //     the time that the components were not active and subtracting the result from
        //     multiplying 100 percent by the number of objects monitored.
        CounterMultiTimer100NsInverse = 592512256,
        //
        // Summary:
        //     An average counter that measures the time it takes, on average, to complete a
        //     process or operation. Counters of this type display a ratio of the total elapsed
        //     time of the sample interval to the number of processes or operations completed
        //     during that time. This counter type measures time in ticks of the system clock.
        AverageTimer32 = 805438464,
        //
        // Summary:
        //     A difference timer that shows the total time between when the component or process
        //     started and the time when this value is calculated.
        ElapsedTime = 807666944,
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
        //     A base counter that stores the number of sampling interrupts taken and is used
        //     as a denominator in the sampling fraction. The sampling fraction is the number
        //     of samples that were 1 (or true) for a sample interrupt. Check that this value
        //     is greater than zero before using it as the denominator in a calculation of SampleFraction.
        SampleBase = 1073939457,
        //
        // Summary:
        //     A base counter that is used in the calculation of time or count averages, such
        //     as AverageTimer32 and AverageCount64. Stores the denominator for calculating
        //     a counter to present "time per operation" or "count per operation".
        AverageBase = 1073939458,
        //
        // Summary:
        //     A base counter that stores the denominator of a counter that presents a general
        //     arithmetic fraction. Check that this value is greater than zero before using
        //     it as the denominator in a RawFraction value calculation.
        RawBase = 1073939459,
        //
        // Summary:
        //     A base counter that indicates the number of items sampled. It is used as the
        //     denominator in the calculations to get an average among the items sampled when
        //     taking timings of multiple, but similar items. Used with CounterMultiTimer, CounterMultiTimerInverse,
        //     CounterMultiTimer100Ns, and CounterMultiTimer100NsInverse.
        CounterMultiBase = 1107494144
    }
}
