﻿using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.Implementations.Environment;
using Vostok.Configuration.Sources.Tests.Commons;

namespace Vostok.Configuration.Sources.Tests
{
    [TestFixture]
    public class EnvironmentVariablesSource_Tests : Sources_Test
    {
        [Test]
        public void Should_return_correct_values()
        {
            var evs = new EnvironmentVariablesSource();
            var res = evs.Get();

            CheckResult(res);
        }

        [Test]
        public void Should_observe_variables()
        {
            new Action(() => ShouldObserveVariablesTest_ReturnsIfValueWasReceived().Should().BeTrue()).ShouldPassIn(1.Seconds());
        }
        private static bool ShouldObserveVariablesTest_ReturnsIfValueWasReceived()
        {
            var val = false;
            var evs = new EnvironmentVariablesSource();
            var sub = evs.Observe().Subscribe(p =>
            {
                val = true;
                CheckResult(p.settings);
            });
            Thread.Sleep(200.Milliseconds());
            sub.Dispose();
            return val;
        }

        private static void CheckResult(ISettingsNode settings)
        {
            var windows = new[] { PlatformID.Win32NT, PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE };
            if (windows.Contains(Environment.OSVersion.Platform))
            {
                settings["pAtH"].Value.Should().NotBeNull();
                settings["APPdata"].Value.Should().NotBeNull();
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                settings["pAtH"].Value.Should().NotBeNull();
                settings["sheLL"].Value.Should().NotBeNull();
            }
            else
                throw new NotImplementedException();
        }
    }
}