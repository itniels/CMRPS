using System;
using System.Linq;
using System.Web.Mvc;
using CMRPS.Web.Controllers;
using CMRPS.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CMRPS.Web.Tests.Controllers
{
    [TestClass]
    public class ColorControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            ColorController controller = new ColorController();
            
            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
