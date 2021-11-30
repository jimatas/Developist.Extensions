// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Extensions.Logging.Log4Net
{
    internal class Log4NetScope : DisposableBase
    {
        private const string DefaultStack = "Scope";
        private readonly Stack<IDisposable> disposables = new Stack<IDisposable>();

        public Log4NetScope(object state) => PushOntoThreadContextStack(state);

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
                disposables.Push(log4net.LogicalThreadContext.Stacks[DefaultStack].Push(str));
            }
            else if (state is IEnumerable collection)
            {
                PushOntoThreadContextStack(collection);
            }
            else
            {
                disposables.Push(log4net.LogicalThreadContext.Stacks[DefaultStack].Push(state.ToString()));
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
                if (itemType == typeof(KeyValuePair<,>))
                {
                    var typeArguments = itemType.GetGenericArguments();
                    if (typeArguments.First() != typeof(string))
                    {
                        break;
                    }

                    var value = itemType.GetProperty(nameof(KeyValuePair<string, object>.Value)).GetValue(item);
                    if (value != null)
                    {
                        var key = itemType.GetProperty(nameof(KeyValuePair<string, object>.Key)).GetValue(item).ToString();
                        disposables.Push(log4net.LogicalThreadContext.Stacks[key].Push(value.ToString()));
                    }
                }
                else
                {
                    disposables.Push(log4net.LogicalThreadContext.Stacks[DefaultStack].Push(item.ToString()));
                }
            }
        }
    }
}
