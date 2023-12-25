using Scraper.Model;
using System;
using System.Collections.Generic;

namespace Scraper.Comparer
{
    internal class ProductComparer : IEqualityComparer<Product>
    {
        public bool Equals(Product a, Product b)
        {
            return StringComparer.InvariantCultureIgnoreCase
                                 .Equals(a.Model, b.Model);
        }

        public int GetHashCode(Product item)
        {
            return StringComparer.InvariantCultureIgnoreCase
                                 .GetHashCode(item.Model);
        }
    }
}
