using AirlineTicketingApi.DTOs;

namespace AirlineTicketingApi.Services.Parsers
{
    public class CsvFlightParser : IFlightFileParser
    {
        public IEnumerable<AddFlightRequestDto> Parse(Stream fileStream)
        {
            var flights = new List<AddFlightRequestDto>();
            using var reader = new StreamReader(fileStream);
            
           
            var header = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Split the CSV line by commas
                var values = line.Split(',');

                // Map the array values to the DTO
                flights.Add(new AddFlightRequestDto 
                {
                    FlightNumber = values[0].Trim(),
                    DateFrom = DateTime.Parse(values[1].Trim()),
                    DateTo = DateTime.Parse(values[2].Trim()),
                    AirportFrom = values[3].Trim(),
                    AirportTo = values[4].Trim(),
                    Duration = int.Parse(values[5].Trim()),
                    Capacity = int.Parse(values[6].Trim())
                });
            }

            return flights;
        }
    }
}