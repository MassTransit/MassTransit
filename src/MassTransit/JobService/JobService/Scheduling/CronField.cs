#nullable enable
namespace MassTransit.JobService.Scheduling;

using System.Collections;
using System.Collections.Generic;


public sealed class CronField :
    IEnumerable<int>
{
    bool _hasAllOrNoSpec;

    int? _singleValue;
    SortedSet<int>? _values;

    public CronField()
    {
        Clear();
    }

    public int Count
    {
        get
        {
            if (_singleValue is not null)
                return 1;

            return _values?.Count ?? 0;
        }
    }

    internal int Min
    {
        get
        {
            if (_singleValue is not null)
                return _hasAllOrNoSpec ? 0 : _singleValue.Value;

            if (_values is not null)
                return _hasAllOrNoSpec ? 0 : _values.Min;

            return 0;
        }
    }

    public IEnumerator<int> GetEnumerator()
    {
        if (_singleValue is not null)
        {
            yield return _singleValue.Value;
            yield break;
        }

        if (_values is not null)
        {
            foreach (var value in _values)
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal void Clear()
    {
        _singleValue = null;
        _values = null;
        _hasAllOrNoSpec = false;
    }

    internal bool TryGetMinValueStartingFrom(int start, out int min)
    {
        min = 0;

        if (_singleValue == CronExpressionConstants.AllSpec)
        {
            min = start;
            return true;
        }

        if (_singleValue is not null)
        {
            if (_singleValue >= start)
            {
                min = _singleValue.Value;
                return true;
            }

            // didn't match
            return false;
        }

        SortedSet<int>? set = _values;

        if (set is null)
            return false;

        min = set.Min;

        if (set.Contains(start))
        {
            min = start;
            return true;
        }

        if (set.Count == 0 || set.Max < start)
            return false;

        if (set.Min >= start)
            return true;

        SortedSet<int> view = set.GetViewBetween(start, int.MaxValue);
        if (view.Count > 0)
        {
            min = view.Min;
            return true;
        }

        return false;
    }

    public void Add(int value)
    {
        _hasAllOrNoSpec = value is CronExpressionConstants.AllSpec or CronExpressionConstants.NoSpec;

        if (_singleValue is null)
        {
            if (_values is null)
                _singleValue = value;
            else
                _values.Add(value);
        }
        else if (_singleValue != value)
        {
            _values =
            [
                _singleValue.Value,
                value
            ];
            _singleValue = null;
        }
    }

    public bool Contains(int value)
    {
        if (_singleValue == value
            || (value != CronExpressionConstants.AllSpec && value != CronExpressionConstants.NoSpec && _hasAllOrNoSpec))
            return true;

        return _values is not null && _values.Contains(value);
    }
}
