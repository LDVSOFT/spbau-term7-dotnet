using System;
using System.Collections.Generic;

namespace MyOption
{
    public sealed class Option<T>
    {
        private static readonly Option<T> EmptyOption;

        private readonly T _value;

        static Option()
        {
            EmptyOption = new Option<T>();
        }

        private Option()
        {
        }

        private Option(T value)
        {
            _value = value;
        }

        public static Option<T> Some(T value) => new Option<T>(value);
        public static Option<T> None() => EmptyOption;

        public bool IsSome() => !IsNone();
        public bool IsNone() => this == EmptyOption;

        public T Value()
        {
            if (IsNone())
                throw new InvalidOperationException("No element in option");
            return _value;
        }

        public Option<TResult> Map<TResult>(Func<T, TResult> f)
        {
            if (IsNone())
                return Option<TResult>.None();
            return Option<TResult>.Some(f(_value));
        }

        public static Option<T> Flatten(Option<Option<T>> a) => a.IsNone() ? None() : a.Value();

        public override bool Equals(object o)
        {
            if (!(o is Option<T> other))
            {
                return false;
            }

            if (this.IsNone() || other.IsNone())
            {
                return this.IsNone() && other.IsNone();
            }

            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            if (IsNone())
            {
                return 0;
            }

            return EqualityComparer<T>.Default.GetHashCode(_value);
        }

        public override string ToString() => IsNone() ? "None" : $"Some({_value.ToString()})";
    }
}
