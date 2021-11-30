// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Extensions.Logging.Log4Net
{
    public class Log4NetLogScope : DisposableBase
    {
        private readonly Stack<IDisposable> disposables = new Stack<IDisposable>();
        private readonly Log4NetLoggerOptions options;

        public Log4NetLogScope(object state, Log4NetLoggerOptions options)
        {
            this.options = Ensure.Argument.NotNull(() => options);
            PushOntoThreadContextStack(state);
        }

        protected override void ReleaseManagedResources()
        {
            while (disposables.Any())
            {
                disposables.Pop().Dispose();
            }
            disposables.Clear();

            base.ReleaseManagedResources();
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
            else if (state is IEnumerable collection)
            {
                PushOntoThreadContextStack(collection);
            }
            else
            {
                disposables.Push(log4net.LogicalThreadContext.Stacks[options.DefaultScopeName].Push(state.ToString()));
            }
        }

        private void PushOntoThreadContextStack(IEnumerable collection)
        {
            foreach (var item in collection)
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
