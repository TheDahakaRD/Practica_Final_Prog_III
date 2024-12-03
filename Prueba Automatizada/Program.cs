using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace Consoleprueba
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configuración del reporte
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "TestReport.html");
            var htmlReporter = new ExtentSparkReporter(reportPath);
            var extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);

            // Crear casos de prueba en el reporte
            var testSuccess = extent.CreateTest("Prueba de Tareas - Caso de Éxito");
            var testFailure = extent.CreateTest("Prueba de Tareas - Caso de Fracaso");

            IWebDriver driver = null!;

            try
            {
                // Configuración del Edge WebDriver
                var options = new EdgeOptions();
                driver = new EdgeDriver(options);

                #region Iniciar sesión de manera incorrecta
                // Prueba de inicio de sesión fallido
                testFailure.Log(AventStack.ExtentReports.Status.Info, "Navegando a la página de login");
                driver.Navigate().GoToUrl("http://localhost:5257");
                driver.Manage().Window.Maximize();
                System.Threading.Thread.Sleep(2000);
                // Capturar pantalla inicial
                string screenshotPath = CaptureScreenshot(driver, "LoginPage_Failure");
                testFailure.AddScreenCaptureFromPath(screenshotPath, "Página de login");

                // Automatizar el formulario de inicio de sesión con credenciales incorrectas
                driver.FindElement(By.Id("email")).SendKeys("UsuarioInexistente");
                driver.FindElement(By.Id("password")).SendKeys("ClaveIncorrecta");
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // Capturar pantalla después del intento de inicio de sesión
                screenshotPath = CaptureScreenshot(driver, "FailedLoginAttempt");
                testFailure.AddScreenCaptureFromPath(screenshotPath, "Página después del intento de inicio de sesión");

                // Verificar mensaje de error
                var errorMessage = driver.FindElement(By.CssSelector(".text-danger")).Text;
                if (errorMessage.Contains("Correo o contraseña incorrectos"))
                {
                    testFailure.Pass("Inicio de sesión fallido como se esperaba.");
                }
                else
                {
                    testFailure.Fail("El mensaje de error no es el esperado.");
                }

                #endregion

                #region Iniciar sesión de manera correcta
                // Prueba de inicio de sesión exitoso
                testSuccess.Log(AventStack.ExtentReports.Status.Info, "Navegando a la página de login");
                driver.Navigate().GoToUrl("http://localhost:5257");
                driver.Manage().Window.Maximize();

                // Capturar pantalla inicial
                screenshotPath = CaptureScreenshot(driver, "LoginPage_Success");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Página de login");

                // Automatizar el formulario de inicio de sesión con credenciales correctas
                driver.FindElement(By.Id("email")).SendKeys("Joceord@gmail.com");
                driver.FindElement(By.Id("password")).SendKeys("123456");
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                System.Threading.Thread.Sleep(2000);

                // Capturar pantalla después del inicio de sesión
                screenshotPath = CaptureScreenshot(driver, "LoggedInPage");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Página después del inicio de sesión");

                #endregion

                #region Visualizar las tareas pendientes
                testSuccess.Log(AventStack.ExtentReports.Status.Info, "Visualizando tareas pendientes");
                screenshotPath = CaptureScreenshot(driver, "TareasPage");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Página de tareas");

                // Esperar a que las tareas se carguen
                System.Threading.Thread.Sleep(2000);

                #endregion

                #region Crear una tarea
                testSuccess.Log(AventStack.ExtentReports.Status.Info, "Creando nueva tarea");
                driver.Navigate().GoToUrl("http://localhost:5257/agregar-tarea");
                System.Threading.Thread.Sleep(2000);

                // Localizar los campos de entrada por ID y establecer los valores
                driver.FindElement(By.Id("title")).SendKeys("Realizar prueba automatizada");
                driver.FindElement(By.Id("description")).SendKeys("Crear y probar prueba automatizada");
                driver.FindElement(By.Id("dueDate")).SendKeys("03-12-2024");
                driver.FindElement(By.Id("priority")).SendKeys("Alta");

                // Hacer clic en el botón de envío
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // Esperar y capturar la pantalla después de crear la tarea
                System.Threading.Thread.Sleep(2000);
                screenshotPath = CaptureScreenshot(driver, "TaskCreated");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Tarea creada");

                #endregion

                #region Buscar una tarea
                driver.Navigate().GoToUrl("http://localhost:5257/buscar-tareas");
                System.Threading.Thread.Sleep(2000);
                testSuccess.Log(AventStack.ExtentReports.Status.Info, "Buscando tarea");
                var searchInput = driver.FindElement(By.CssSelector("input[placeholder='Buscar tarea...']"));
                searchInput.SendKeys("Realizar prueba automatizada");

                // Simular el "Enter" después de escribir el término de búsqueda
                searchInput.SendKeys(Keys.Enter);

                // Esperar y capturar la pantalla después de realizar la búsqueda
                System.Threading.Thread.Sleep(2000);
                screenshotPath = CaptureScreenshot(driver, "SearchTask");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Tarea buscada");

                #endregion


                #region Eliminar una tarea
                driver.Navigate().GoToUrl("http://localhost:5257/eliminar-tareas");
                System.Threading.Thread.Sleep(2000);
                testSuccess.Log(AventStack.ExtentReports.Status.Info, "Eliminando tarea");

                screenshotPath = CaptureScreenshot(driver, "TaskDeletedBF");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Tareas antes de eliminacion");
                // Seleccionar la tarea que se desea eliminar (basado en el texto del título)
                var deleteButton = driver.FindElement(By.XPath("//div[contains(@class, 'task')]//h5[text()='Limpiar habitación']/following-sibling::button[contains(@class, 'btn-danger')]"));
                deleteButton.Click();

                // Esperar a que aparezca el cuadro de confirmación
                System.Threading.Thread.Sleep(1000); // Tiempo de espera para el cuadro de confirmación

                // Hacer clic en el botón "Sí" para confirmar la eliminación
                var confirmDeleteButton = driver.FindElement(By.CssSelector("button.btn-success"));
                confirmDeleteButton.Click();

                // Esperar y capturar la pantalla después de eliminar la tarea
                System.Threading.Thread.Sleep(2000);
                screenshotPath = CaptureScreenshot(driver, "TaskDeleted");
                testSuccess.AddScreenCaptureFromPath(screenshotPath, "Tareas despues de eliminacion");

                #endregion
            }
            catch (Exception ex)
            {
                // Si ocurre un error general
                testFailure.Fail("Error durante la prueba: " + ex.Message);
            }
            finally
            {
                // Cerrar el navegador
                driver?.Quit();
                testSuccess.Log(AventStack.ExtentReports.Status.Info, "Navegador cerrado");
                testFailure.Log(AventStack.ExtentReports.Status.Info, "Navegador cerrado");

                // Generar el reporte
                extent.Flush();
            }
        }

        // Método para capturar capturas de pantalla
        static string CaptureScreenshot(IWebDriver driver, string screenshotName)
        {
            try
            {
                string screenshotsDir = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                Directory.CreateDirectory(screenshotsDir);

                string screenshotPath = Path.Combine(screenshotsDir, $"{screenshotName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);

                return screenshotPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al capturar la pantalla: " + ex.Message);
                return null!;
            }
        }
    }
}
