﻿using System;
using System.Threading.Tasks;
using KUiPath.Commands;
using KUiPath.Config;
using KUiPath.Manager;
using KUiPath.Models;
using KUiPath.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KUiPathTest.Manager
{
    [TestClass]
    public class HttpClientManagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var contents = new SampleObject()
            {
                password = "dummy",
                usernameOrEmailAddress = "dummy",
                tenancyName = "dummy"
            };
            var result = HttpClientManager.ExecutePostAsync<SampleObject>("https://academy2016.uipath.com/api/account/authenticate", contents).Result;
            Assert.AreEqual("An error has occured",result.ResponceContent);
        }


        public class SampleObject : BaseCommandModel
        {
            public string tenancyName { get; set; }
            public string usernameOrEmailAddress { get; set; }
            public string password { get; set; }
            public string result { get; set; }
            public object targetUrl { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }
    }
}
