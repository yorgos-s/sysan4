﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using webapi.Services;  // Add this to use IEmailService
using webapi.Services.cs;
using webapi.Models; // Ensure this is included


namespace webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IPortfolioService _portfolioService;
        private string[]? SelectedPortfolios;
        private readonly IPortfolioProcessingService _portfolioProcessingService;

        private string filePath = "H:/_BKP/myData3.csv"; // Adjust the path as needed.

        // Example data to be written to the CSV file
        private List<string> data = new List<string>
            {
                "Header1,Header2,Header3",
                "Value1,Value2,Value3",
                "Value4,Value5,Value6"
            };



        // Constructor to inject the spesific Services
        public PortfoliosController(IEmailService emailService, IPortfolioService portfolioService, IPortfolioProcessingService portfolioProcessingService)
        {
            _emailService = emailService;
            _portfolioService = portfolioService;
            _portfolioProcessingService = portfolioProcessingService;

            //CsvService.CreateCsvFromList(data, filePath);
        }


        //Send the initial portfolio names
        [HttpGet("getteststring")]
        public ActionResult<string> GetTestString()
        {
            return Ok(_portfolioService.GetPortfolioNames());
        }

        //test CSV service
        [HttpGet]
        public IActionResult CreateCsv()
        {
            
            return Ok("CSV file created successfully");
        }



        
        [HttpPost("sendportfolio")]
        public IActionResult SendPortfolio([FromBody] PortfolioModel input)
        {

            // Define the file path where the CSV will be saved


            // Call the service to create the CSV file


            // Return a response indicating success
            // You might want to return the file path or a message to the client
            //return Ok("maybe ok");

            CsvService.CreateCsvFromList(data, filePath);

            try
            {
                //request from the portfolioProcessingService the data associated w/ portfolio input 
                var allRows = _portfolioProcessingService.SendPortfolio(input);
                // Assuming successful execution, return the data.
                return Ok(new { data = allRows });
                //return Ok(new { data = "fake data" });

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during processing
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //email must be a string
        public class EmailData
        {
            public string? PortfolioTable { get; set; }
        }


        //hey API, POST the data of the selected portfolio  (to create the body of the email)
        [HttpPost("PostPortfolio")]
        public IActionResult PostPortfolio([FromBody] EmailData model)
        {
            // This line is just for debugging purposes, you can remove it if not needed
            System.Diagnostics.Debug.WriteLine(model.PortfolioTable);

            // Simplified email body to just the string "TEST"
            string? emailBody = model.PortfolioTable;

            // Send an email notification with the simplified content
            _emailService.SendEmail("giorgosstefatos@gmail.com", "New Data Received", emailBody);

            return Ok(new { message = "Data processed successfully" });
        }


        //hey API, POST the portfolios number positions from the selected rows in the menu
        public class portfoliosNumbers
        {
            public int[]? portfoliosNumbersArray { get; set; }
        }


        [HttpPost("PortfolioStringsArray")]
        public IActionResult PostPortfolios([FromBody] portfoliosNumbers model)
        {
            var portfolioNamesList = _portfolioService.GetPortfolioNames().ToList();
            var portfoliosNumbersArray = model.portfoliosNumbersArray;

            foreach (int i in portfoliosNumbersArray)
            {
                System.Diagnostics.Debug.WriteLine(portfolioNamesList[i]);
            }
            //System.Diagnostics.Debug.WriteLine(model.portfoliosNumbersArray);
            return Ok(new { someName = "SuccessMessage" });
        }

    }
}

