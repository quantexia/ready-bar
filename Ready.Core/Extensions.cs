using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ready.Core
{
    public static class Extensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> input, Action<T> action)
        {
            foreach (T item in input)
                action(item);

            return input;
        }
    }
}
