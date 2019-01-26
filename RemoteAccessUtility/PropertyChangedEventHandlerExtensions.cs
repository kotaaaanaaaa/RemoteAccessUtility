using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RemoteAccessUtility
{
    /// <seealso>https://qiita.com/soi/items/d0c83a0cc3a4b23237ef</seealso>
    public static class PropertyChangedEventHandlerExtensions
    {
        public static void Raise<TResult>(this PropertyChangedEventHandler _this, Expression<Func<TResult>> propertyName)
        {
            if (_this == null) return;

            if (!(propertyName.Body is MemberExpression memberEx))
                throw new ArgumentException();

            if (!(memberEx.Expression is ConstantExpression senderExpression))
                throw new ArgumentException();

            var sender = senderExpression.Value;
            _this(sender, new PropertyChangedEventArgs(memberEx.Member.Name));
        }

        public static bool RaiseIfSet<TResult>(this PropertyChangedEventHandler _this, Expression<Func<TResult>> propertyName, ref TResult source, TResult value)
        {
            if (EqualityComparer<TResult>.Default.Equals(source, value))
                return false;

            source = value;
            Raise(_this, propertyName);

            return true;
        }
    }
}
