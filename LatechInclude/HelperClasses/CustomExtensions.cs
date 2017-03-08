using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace LatechInclude.HelperClasses
{
    /// <summary>
    /// CustomExtensions class
    /// </summary>
    public static class CustomExtensions
    {
        /// <summary>
        /// Extension method SubArray
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Array</param>
        /// <param name="index">index to use</param>
        /// <param name="length">length to use</param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
