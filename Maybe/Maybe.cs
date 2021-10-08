﻿using System;

namespace Maybe
{
    /// <summary>
    /// The generic Maybe monad.
    /// </summary>
    public readonly struct Maybe<T>
    {
        /// <value>A Maybe without a value.</value>
        public static readonly Maybe<T> Nothing = new Maybe<T>(default, false);

        private readonly T obj;

        private Maybe(T obj, bool hasValue)
        {
            this.obj = obj;
            HasValue = hasValue;
        }

        internal Maybe(T obj)
        {
            this.obj = obj;
            HasValue = true;
        }

        /// <value>True if there is a value present, otherwise false.</value>
        public bool HasValue { get; }

        /// <value>The encapsulated value.</value>
        /// <exception cref="Exception">Thrown if HasValue is false.</exception>
        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new Exception("No Value is present");
                }

                return obj;
            }
        }

        /// <summary>
        /// Returns the value or a default.
        /// </summary>
        /// <returns>
        /// The value if HasValue is true, otherwise returns defaultValue 
        /// </returns>
        /// <param name="defaultValue"> The default value.</param>
        public T Or(T defaultValue) => HasValue ? obj : defaultValue;

        /// <summary>
        /// Returns the value or a default provided by defaultSupplier.
        /// </summary>
        /// <returns>
        /// The value if HasValue is true, otherwise returns the value provided by defaultSupplier
        /// </returns>
        /// <param name="defaultSupplier"> The default value supplier.</param>
        public T Or(Func<T> defaultSupplier)
        {
            defaultSupplier = defaultSupplier ?? throw new ArgumentNullException(nameof(defaultSupplier));

            return HasValue ? obj : defaultSupplier();
        }
        
        /// <summary>
        /// Returns this object or an alternative.
        /// </summary>
        /// <returns>
        /// This if HasValue is true, otherwise an alternative provided by alternativeSupplier
        /// </returns>
        /// <param name="alternativeSupplier"> The alternative supplier.</param>
        public Maybe<T> OrMaybe(Func<Maybe<T>> alternativeSupplier)
        {
            alternativeSupplier = alternativeSupplier ?? throw new ArgumentNullException(nameof(alternativeSupplier));

            return HasValue ? this : alternativeSupplier();
        }

        /// <summary>
        /// Returns this object or an alternative.
        /// </summary>
        /// <returns>
        /// This if HasValue is true, otherwise an alternative provided
        /// </returns>
        /// <param name="alternative"> The alternative.</param>
        public Maybe<T> OrMaybe(Maybe<T> alternative)
        {
            return HasValue ? this : alternative;
        }

        /// <summary>
        /// Returns this object or an alternative.
        /// </summary>
        /// <returns>
        /// This if HasValue is true, otherwise an alternative provided
        /// </returns>
        /// <param name="alternative"> The alternative.</param>
        public Maybe<T> OrMaybe(T alternative)
        {
            return HasValue ? this : alternative.ToMaybe();
        }
        
        /// <summary>
        /// Returns this object or an alternative.
        /// </summary>
        /// <returns>
        /// This if HasValue is true, otherwise an alternative provided provided by alternativeSupplier
        /// </returns>
        /// <param name="alternativeSupplier"> The alternative supplier.</param>
        public Maybe<T> OrMaybe(Func<T> alternativeSupplier)
        {
            alternativeSupplier = alternativeSupplier ?? throw new ArgumentNullException(nameof(alternativeSupplier));

            return HasValue ? this : alternativeSupplier().ToMaybe();
        }

        /// <summary>
        /// Returns the value if HasValue is true, otherwise throws an exception.
        /// </summary>
        /// <returns>
        /// The value if HasValue is true
        /// </returns>
        /// <param name="errorSupplier"> The error supplier that provides the exception to be thrown.</param>
        /// <exception cref="Exception">Thrown if HasValue is false.</exception>
        public T OrThrow(Func<Exception> errorSupplier)
        {
            errorSupplier = errorSupplier ?? throw new ArgumentNullException(nameof(errorSupplier));

            if (HasValue)
            {
                return obj;
            }
            throw errorSupplier();
        }

        /// <summary>
        /// Applies an action to the value if it's present.
        /// </summary>
        /// <param name="consumer"> The action to be applied to the value.</param>
        public void Consume(Action<T> consumer)
        {
            consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));

            if (HasValue)
            {
                consumer(obj);
            }
        }

        /// <summary>
        /// Determines if this instance is equals to another maybe instance.
        /// </summary>
        /// <returns>
        /// True if HasValue is false for both maybes or if HasValue is true for both and their values match using Equals.
        /// False otherwise
        /// </returns>
        /// <param name="other"> The other maybe.</param>
        public bool Equals(Maybe<T> other)
        {
            if (HasValue)
            {
                return other.HasValue && Value.Equals(other.Value);
            }
            else
            {
                return !other.HasValue;
            }
        }

        /// <summary>
        /// Determines if this instance is equals to another obj instance.
        /// </summary>
        /// <returns>
        /// True if obj is an instance of Maybe&lt;<typeparamref name="T"/>&gt; 
        /// and it matches the current instance using Equals(Maybe&lt;T&gt; other).
        /// False otherwise
        /// </returns>
        /// <param name="obj"> The other obj.</param>
        public override bool Equals(object obj) => (obj is Maybe<T> other) && Equals(other);

        /// <summary>
        /// Calculates the HashCode.
        /// </summary>
        /// <returns>
        /// 0 if HasValue is false, otherwise returns the value's HashCode.
        /// </returns>
        public override int GetHashCode() => HasValue ? Value.GetHashCode() : 0;

        /// <summary>
        /// The string representation of this instance.
        /// </summary>
        /// <returns>
        /// The string representation of Value if HasValue is true, otherwise string.Empty.
        /// </returns>
        public override string ToString() => HasValue ? Value.ToString() : string.Empty;

        /// <summary>
        /// Determines if the encapsulated value matches another value using Equals.
        /// </summary>
        /// <returns>
        /// True if HasValue is true and the Value matches the other value using Equals.
        /// False otherwise
        /// </returns>
        /// <param name="other"> The other value.</param>
        public bool Is(T other) => HasValue && Value.Equals(other);
                
        /// <summary>
        /// Determines if the encapsulated value matches a predicate.
        /// </summary>
        /// <returns>
        /// True if HasValue is true and the Value matches the predicate.
        /// False otherwise
        /// </returns>
        /// <param name="predicate"> The predicate.</param>
        public bool Is(Func<T, bool> predicate)
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            return HasValue && predicate(Value);
        }

        /// <summary>
        /// Filters the value using a predicate. Analogous to Linq's Where.
        /// </summary>
        /// <returns>
        /// The calling maybe if the encapsulated value matches the predicate,
        /// Maybe&lt;<typeparamref name="T"/>&gt;.Nothing otherwise
        /// </returns>
        /// <param name="predicate"> The predicate.</param>
        public Maybe<T> Where(Func<T, bool> predicate)
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            return !Is(predicate) ? Nothing : this;
        }
    }
}
