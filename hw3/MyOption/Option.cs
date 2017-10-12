using System;
using System.Collections.Generic;

namespace MyOption
{
    public sealed class Option<T>
    {
        private static Option<T> _emptyOption = new Option<T>();

        private readonly T _value;

        private Option()
        {
        }

        private Option(T value)
        {
            _value = value;
        }

        public static Option<T> Some(T value) => new Option<T>(value);
        public static Option<T> None() => _emptyOption;

        public bool IsSome() => !Equals(_emptyOption);
        public bool IsNone() =>  Equals(_emptyOption);

        public T Value()
        {
            if (IsNone())
                throw new Exception("No element in option");
            return _value;
        }

        public Option<U> Map<U>(Func<T, U> f)
        {
            if (IsNone())
                return Option<U>.None();
            return Option<U>.Some(f(_value));
        }

        public static Option<T> Flatten(Option<Option<T>> a) => a.IsNone() ? None() : a.Value();

        public override bool Equals(object o)
        {
            if (!(o is Option<T> other))
                return false;
            if (this == _emptyOption || other == _emptyOption)
                return this == other;
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            if (IsNone())
                return 0;
            return EqualityComparer<T>.Default.GetHashCode(_value);
        }

        public override string ToString() => IsNone() ? "None" : $"Some({_value.ToString()})";
    }
}