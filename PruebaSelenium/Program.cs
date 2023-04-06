using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Generic;
using PuppeteerSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Drawing.Imaging;

namespace PruebaScrap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize the Chrome Driver
            using (var driver = new ChromeDriver())
            {
                var h3 = string.Empty;
                for (int i = 65466; i < 65490; i++)
                {
                    // Go to the home page
                    driver.Navigate().GoToUrl("http://www.ssn.gob.ar/storage/registros/productores/productoresactivosfiltro.asp");

                    driver.ExecuteScript("document.getElementsByName('form1')[0].target=''");
                    
                    // Get the page elements
                    var NroMatricula = driver.FindElement(By.Id("matricula"));
                    NroMatricula.SendKeys(i.ToString());

                    var FromSubmit = driver.FindElement(By.Name("form1"));
                    FromSubmit.TagName.Replace("target=\"_blank\"", "");

                    var SubmitButton = driver.FindElement(By.Name("Submit"));
                    SubmitButton.Submit();              

                    //// Extract the text and save it into result.txt
                    var listOfElements = driver.FindElements(By.XPath("//h3"));
                    foreach(var element in listOfElements)
                    {
                        h3 += "|" + element.Text;
                    }
                    listOfElements = driver.FindElements(By.XPath("//h5"));
                    foreach (var element in listOfElements)
                    {
                        h3 += "|" + element.Text;
                    }                    
                    h3 += System.Environment.NewLine;
                    driver.GetScreenshot().SaveAsFile($"screen{i}.png", ScreenshotImageFormat.Png);
                }
                File.WriteAllText("result.txt", h3);
            }    
        }


        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }

        private static async void GetMatriculado()
        {
            string fullUrl = "http://www.ssn.gob.ar/storage/registros/productores/productoresactivosfiltro.asp";

            List<string> programmerLinks = new List<string>();

            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };

            var browser = await Puppeteer.LaunchAsync(options, null);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(fullUrl);

            var links = @"Array.from(document.querySelectorAll('li:not([class^=""toc""]) a')).map(a => a.href);";
            var urls = await page.EvaluateExpressionAsync<string[]>(links);

            foreach (string url in urls)
            {
                programmerLinks.Add(url);
            }         
        }
    }

}
