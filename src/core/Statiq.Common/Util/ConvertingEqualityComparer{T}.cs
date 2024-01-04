using System;
using System.Collections.Generic;

namespace Statiq.Common
{
    /// <summary>
    /// Adapts a typed equality comparer to untyped metadata by attempting to convert the
    /// metadata values to the comparer type before running the comparison. If neither type
    /// can be converted to <typeparamref name="T"/>, the comparison fails.
    /// </summary>
    /// <typeparam name="T">The value type to convert to for comparisons.</typeparam>
    public class ConvertingEqualityComparer<T> : IEqualityComparer<object>
    {
        private readonly IEqualityComparer<T> _comparer;

        public ConvertingEqualityComparer(IEqualityComparer<T> comparer)
        {
            _comparer = comparer.ThrowIfNull(nameof(comparer));
        }

        bool IEqualityComparer<object>.Equals(object? x, object? y) =>
            TypeHelper.TryConvert(x, out T xValue)
                && TypeHelper.TryConvert(y, out T yValue)
                && _comparer.Equals(xValue, yValue);

#pragma warning disable CS8607 // A possible null value may not be used for a type marked with [NotNull] or [DisallowNull]
        public int GetHashCode(object obj) =>
            TypeHelper.TryConvert(obj, out T value)
                ? _comparer.GetHashCode(value)
                : 0;
#pragma warning restore CS8607 // A possible null value may not be used for a type marked with [NotNull] or [DisallowNull]
    }
}
