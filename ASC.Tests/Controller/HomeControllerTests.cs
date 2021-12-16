using ASC.Tests.TestUtilities;
using Lap1.Controllers;
using Lap1.Web.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ASC.Tests.Controller
{
    
    public class HomeControllerTests
    {
        private readonly Mock<IOptions<ApplicationSettings>> optionsMock;
        private readonly Mock<HttpContext> mockHttpContext;
        public HomeControllerTests()
        {
            optionsMock = new Mock<IOptions<ApplicationSettings>>();

            mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(p => p.Session).Returns(new FakeSession());

            optionsMock.Setup(ap => ap.Value).Returns(new ApplicationSettings
            {
                ApplicationTitle = "ASC"
            });
        }
        [Fact]
        public void HomeController_Index_View_Tests()
        {
            var controller = new HomeController(optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            Assert.IsType(typeof(ViewResult), controller.Index());
        }
        [Fact]
        public void HomeController_Index_NoModel_Tests()
        {
            var controller = new HomeController(optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            Assert.Null((controller.Index() as ViewResult).ViewData.Model);
        }
        [Fact]
        public void HomeController_Index_Validation_Tests()
        {
            var controller = new HomeController(optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            Assert.Equal(0, (controller.Index() as ViewResult).ViewData.ModelState.ErrorCount);
        }
    }
}
