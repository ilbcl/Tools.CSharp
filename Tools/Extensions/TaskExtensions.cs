using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Tools.Extensions
{
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ConfiguredTaskAwaitable<T> CAF<T>(this Task<T> self) => self.ConfigureAwait(false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ConfiguredTaskAwaitable CAF(this Task self) => self.ConfigureAwait(false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static T GAGR<T>(this Task<T> self) => self.GetAwaiter().GetResult();
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void GAGR(this Task self) => self.GetAwaiter().GetResult();
    }
}
