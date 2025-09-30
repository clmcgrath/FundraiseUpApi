using System;
using System.Collections.Generic;
using System.Text.Json;
using FundraiseUp.Client.Models;

var expectedResult = new PagedResult<Donation>
{
    Items = new List<Donation>
    {
        new Donation
        {
            Id = "donation-1",
            Amount = 25.00m,
            Currency = "USD",
            Status = DonationStatus.Completed
        },
        new Donation
        {
            Id = "donation-2",
            Amount = 75.00m,
            Currency = "USD",
            Status = DonationStatus.Pending
        }
    },
    TotalCount = 2,
    CurrentPage = 1,
    PageSize = 10
};

var json = JsonSerializer.Serialize(expectedResult);
Console.WriteLine("JSON Output:");
Console.WriteLine(json);

var deserialized = JsonSerializer.Deserialize<PagedResult<Donation>>(json);
Console.WriteLine($"\nDeserialized TotalCount: {deserialized?.TotalCount}");
Console.WriteLine($"Deserialized Items Count: {deserialized?.Items.Count}");