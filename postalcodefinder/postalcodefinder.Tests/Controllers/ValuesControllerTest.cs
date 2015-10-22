using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using postalcodefinder;
using postalcodefinder.Controllers;

namespace postalcodefinder.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {

        [TestMethod]
        public void Post()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Post(null);

            // Assert
        }
    }
}
