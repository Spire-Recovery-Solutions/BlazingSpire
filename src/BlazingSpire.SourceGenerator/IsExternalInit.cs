// Polyfill for netstandard2.0 — enables record types and init-only setters
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
