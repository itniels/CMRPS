﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CMRPS.Web;
using CMRPS.Web.Controllers;
using CMRPS.Web.Models;

namespace CMRPS.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UpdateData()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            PartialViewResult result = controller.UpdateData() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
