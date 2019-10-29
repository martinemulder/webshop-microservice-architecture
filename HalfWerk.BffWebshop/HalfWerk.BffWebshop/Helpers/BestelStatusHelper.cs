using System;
using HalfWerk.CommonModels.BffWebshop.BestellingService;

namespace HalfWerk.BffWebshop.Helpers
{
    public static class BestelStatusHelper
    {
        public static T CastTo<T>(this BestelStatus status)
        {
            return (T) Enum.ToObject(typeof(T), (int) status);
        }
    }
}