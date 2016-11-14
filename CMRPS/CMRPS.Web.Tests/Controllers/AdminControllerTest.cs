using System;
using System.Web.Mvc;
using CMRPS.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMRPS.Web.Tests.Controllers
{
    [TestClass]
    public class AdminControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            AdminController controller = new AdminController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
