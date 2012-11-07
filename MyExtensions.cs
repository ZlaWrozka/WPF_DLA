using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DLA
{
    public static class MyExtensions
    {
        public static IEnumerable<T> FindDescendants<T>(this DependencyObject parent, Func<T, bool> predicate, bool deepSearch = false) where T : DependencyObject
        {
            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>().ToList();

            foreach (var child in children)
            {
                var typedChild = child as T;
                if ((typedChild != null) && (predicate == null || predicate.Invoke(typedChild)))
                {
                    yield return typedChild;
                    if (deepSearch) foreach (var foundDescendant in FindDescendants(child, predicate, true)) yield return foundDescendant;
                }
                else
                {
                    foreach (var foundDescendant in FindDescendants(child, predicate, deepSearch)) yield return foundDescendant;
                }
            }

            yield break;
        }
    }
}
