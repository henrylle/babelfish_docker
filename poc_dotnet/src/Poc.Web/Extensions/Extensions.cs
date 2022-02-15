using System;

namespace Poc.Web.Extensions
{
    public static class Extensions {

        public static Guid ToGuid(this string str) => Guid.Parse(str);
    }
}