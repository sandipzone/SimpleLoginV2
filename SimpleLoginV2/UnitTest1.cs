using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Security.Policy;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.MarkupUtils;
using NUnit.Framework.Interfaces;
using SimpleLoginV2.utilities;

namespace SimpleLoginV2
{
    public class Tests
    {
        private IWebDriver driver;// = new ChromeDriver();
        private ExtentReports extent;
        private ExtentTest extest;

        public ExtentTest GetExtentTest()
        {
            return extest;
        }


        [OneTimeSetUp]
        public void BeforeAllTest()
        {
            DateTime currentTime = DateTime.Now;
            var htmlReporter = new ExtentSparkReporter(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\reports\\SimpleTestReport_" + currentTime.ToString("yyyy-mm-dd_HH-mm-ss") + ".html");
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard;
            htmlReporter.Config.DocumentTitle = "Simple Login test execution report";
            htmlReporter.Config.ReportName = "Automated test report";
            htmlReporter.Config.Encoding = "utf-8";
            extent = new ExtentReports();

            extent.AttachReporter(htmlReporter);
            extent.AddSystemInfo("Organization: ", "Oritain");
            extent.AddSystemInfo("Project: ", "Login upgrade");
            extent.AddSystemInfo("Build No: ", "1.2.3");

        }


        [SetUp]
        public void BeforeEachTest()
        {
            driver = new ChromeDriver();
            extest = extent.CreateTest(TestContext.CurrentContext.Test.Name); // Creating test for reporting
            //extest = extent.CreateTest(TestContext.CurrentContext.Test.ClassName + " @ Test Case Name : " + TestContext.CurrentContext.Test.Name);
        }

        public static IEnumerable<TestCaseData> GetTestData()
        {
            //yield return new TestCaseData("admin", "admin");
            //yield return new TestCaseData("admin2", "admin2");
            //yield return new TestCaseData("admin", "admin");

            // OR
            var columns = new List<string> { "username", "password" };
            return DataUtil.GetTestDataFromExcel(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\resources\\testdata.xlsx", "logintest", columns);

        }

        [Test, TestCaseSource("GetTestData")]
        public void SimpleLoginTest(string username, string pw)
        {

            //IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://demo.testfire.net/login.jsp");

            driver.FindElement(By.Id("uid")).SendKeys(username);

            driver.FindElement(By.Id("passw")).SendKeys(pw);

            //driver.FindElement(By.Name("btnSubmit")).Submit();


            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
            var loginBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Name("btnSubmit")));

            loginBtn.Submit();
            Assert.That(driver.Url, Is.EqualTo("https://demo.testfire.net/bank/main.jsp"), "Login unsuccessful, navigated to different url / page!");

            Console.WriteLine(driver.FindElement(By.XPath("/html/body/table[2]/tbody/tr/td[2]/div/h1")).Text);
            var loginSuccess = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/table[2]/tbody/tr/td[2]/div/h1")));

            //Assert.Multiple(() =>
            //{
            // Just in case if we want to perform multiple assertions even after one failing.
            Assert.That(loginSuccess.Displayed, "Missing successful login message!");
            Assert.That("Hello Admin User", Is.EqualTo(loginSuccess.Text), "Successful login message differs compared to expected!");
            //});

        }


        [TearDown]
        public void AfterEachTest()
        {
            //Get the test status
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

            if (testStatus == TestStatus.Passed)
            {
                extest.Pass("Test case pass");
                IMarkup markup = MarkupHelper.CreateLabel("PASS", ExtentColor.Green);
                extest.Pass(markup);

            }
            else if (testStatus == TestStatus.Skipped)
            {
                extest.Skip("Test Skipped : " + TestContext.CurrentContext.Result.Message);
                IMarkup markup = MarkupHelper.CreateLabel("SKIP", ExtentColor.Amber);
                extest.Skip(markup);
            }
            else if (testStatus == TestStatus.Failed)
            {
                extest.Fail("Test Failed : " + TestContext.CurrentContext.Result.Message);
                IMarkup markup = MarkupHelper.CreateLabel("FAIL", ExtentColor.Red);
                extest.Fail(markup);
            }
            driver.Close();
            driver.Quit();
        }

        //[Test]
        //public void Test2() 
        //{
        //    //IWebDriver driver = new ChromeDriver();

        //    //driver.Navigate().GoToUrl("https://demo.testfire.net/login.jsp");

        //    //LoginPage loginPage = new LoginPage(driver);

        //    //loginPage.Login("admin", "admin");
        //}

        [OneTimeTearDown]
        public void AfterAllTest()
        {
            //Sign off
            //driver.Navigate().GoToUrl("https://demo.testfire.net/logout.jsp");
            Console.WriteLine("Tear Down script executed");
            extent.Flush();

            //Close browser

            //if (driver != null)
            //{
            //    driver.Close();
            //    driver.Quit();
            //}
        }
    }
}