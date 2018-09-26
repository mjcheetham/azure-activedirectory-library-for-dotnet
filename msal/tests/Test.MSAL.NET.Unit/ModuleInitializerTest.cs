﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Test.MSAL.NET.Unit
{
    [TestClass]
    public class ModuleInitializerTest
    {
        [TestMethod]
        public void InitializesExceptionsAndLogs()
        {
            // EnsureModuleInitilizerIsCleared
            var field = typeof(ModuleInitializer).GetField("isInitialized", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, false);

            // Act
            ModuleInitializer.EnsureModuleInitialized();

            // Assert
            MsalExceptionFactory factory = InternalCoreExceptionFactory.GetCoreExceptionFactory() as MsalExceptionFactory;
            MsalLogger logger = CoreLoggerBase.Default as MsalLogger;
            Telemetry telemetry = CoreTelemetryService.GetInstance() as Telemetry;
            Assert.IsNotNull(factory);
            Assert.IsNotNull(logger);
            Assert.IsNotNull(telemetry);

            // Act
            ModuleInitializer.EnsureModuleInitialized();

            // Assert
            Assert.AreEqual(factory, InternalCoreExceptionFactory.GetCoreExceptionFactory(), "Initialization should have happened only once");
            Assert.AreEqual(logger, CoreLoggerBase.Default, "Initialization should have happened only once");
            Assert.AreEqual(telemetry, CoreTelemetryService.GetInstance() as Telemetry, "Initialization should have happened only once");
        }
    }
}
