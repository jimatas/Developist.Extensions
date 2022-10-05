using Developist.Extensions.Logging.Log4Net.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Extensions.Logging.Log4Net
{
    public sealed class LogScope : IDisposable
    {
        private readonly Stack<IDisposable> disposables = new Stack<IDisposable>();
        private readonly LoggerOptions options;

        public LogScope(object state, LoggerOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            PushOntoThreadContextStack(state);
        }

        public void Dispose()
        {
            while (disposables.Any())
            {
                disposables.Pop().Dispose();
            }
            disposables.Clear();
        }

        private void PushOntoThreadContextStack(object state)
        {
            if (state is null)
            {
                return;
            }

            if (state is string str)
            {
                disposables.Push(log4net.LogicalThreadContext.Stacks[options.DefaultScopeName].Push(str));
            }
            else if (state is IEnumerable states)
            {
                PushOntoThreadContextStack(states);
            }
            else
            {
                disposables.Push(log4net.LogicalThreadContext.Stacks[options.DefaultScopeName].Push(state.ToString()));
            }
        }

        private void PushOntoThreadContextStack(IEnumerable states)
        {
            foreach (var item in states)
            {
                if (item is null)
                {
                    continue;
                }

                var itemType = item.GetType();
                if (itemType.DerivesFromGenericParent(typeof(KeyValuePair<,>)) && itemType.GetGenericArguments().First() == typeof(string))
                {
                    var value = itemType.GetProperty(nameof(KeyValuePair<string, object>.Value)).GetValue(item);
                    if (value is null)
                    {
                        continue;
                    }

                    var key = itemType.GetProperty(nameof(KeyValuePair<string, object>.Key)).GetValue(item).ToString();
                    disposables.Push(log4net.LogicalThreadContext.Stacks[key].Push(value.ToString()));
                }
                else
                {
                    disposables.Push(log4net.LogicalThreadContext.Stacks[options.DefaultScopeName].Push(item.ToString()));
                }
            }
        }
    }
}
