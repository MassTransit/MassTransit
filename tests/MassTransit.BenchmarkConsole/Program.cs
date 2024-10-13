using System.Reflection;
using BenchmarkDotNet.Running;

var currentAssembly = Assembly.GetExecutingAssembly();
BenchmarkSwitcher
    .FromAssembly(currentAssembly)
    .Run(args);
